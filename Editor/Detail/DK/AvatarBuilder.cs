/*
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Components;
using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.DK
{
    internal class AvatarBuilder
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;

        public const string LogLabel = "AvatarBuilder";

        public static class MessageCode
        {
            // Error
            public const string PassHasErrors = "detail.dk.avatarBuilder.msgCode.error.passHasErrors";
            public const string UnresolvedOrCyclicDependencies = "detail.dk.avatarBuilder.msgCode.error.unresolvedOrCyclicDependencies";
        }

        public DKNativeContext Context { get; private set; }

        private PluginManager _plugMgr;

        public AvatarBuilder(GameObject rootGameObject)
        {
            if (!AssetDatabase.IsValidFolder(DKNativeContext.GeneratedAssetsPath))
            {
                AssetDatabase.CreateFolder("Assets", DKNativeContext.GeneratedAssetsFolderName);
            }

            _plugMgr = new PluginManager();
            Context = new DKNativeContext(rootGameObject);
        }

        private void SetUp()
        {
            Context.OnEnable();
        }

        private void TearDown()
        {
            RemapAnimations();
            DispatchAnimationStore();
            Context.OnDisable();

            // remove all DT components
            var dtComps = Context.AvatarGameObject.GetComponentsInChildren<DKBaseComponent>();
            foreach (var comp in dtComps)
            {
                Object.DestroyImmediate(comp);
            }

            EditorUtility.SetDirty(Context.AssetContainer);
            AssetDatabase.SaveAssets();
        }

        private void DispatchAnimationStore()
        {
            var store = Context.Feature<AnimationStore>();
            store.Dispatch();
        }

        private void RemapAnimations()
        {
            var store = Context.Feature<AnimationStore>();
            var remapper = Context.Feature<PathRemapper>();

            foreach (var clipContainer in store.Clips)
            {
                var oldClip = clipContainer.originalClip;
                var remapped = false;

                var newClip = new AnimationClip()
                {
                    name = oldClip.name,
                    legacy = oldClip.legacy,
                    frameRate = oldClip.frameRate,
                    localBounds = oldClip.localBounds,
                    wrapMode = oldClip.wrapMode
                };
                AnimationUtility.SetAnimationClipSettings(newClip, AnimationUtility.GetAnimationClipSettings(oldClip));

                var curveBindings = AnimationUtility.GetCurveBindings(oldClip);
                foreach (var curveBinding in curveBindings)
                {
                    var avoidContainerBones = curveBinding.type == typeof(Transform);
                    var newPath = remapper.Remap(curveBinding.path, avoidContainerBones);

                    remapped |= newPath != curveBinding.path;

                    newClip.SetCurve(newPath, curveBinding.type, curveBinding.propertyName, AnimationUtility.GetEditorCurve(oldClip, curveBinding));
                }

                var objRefBindings = AnimationUtility.GetObjectReferenceCurveBindings(oldClip);
                foreach (var objRefBinding in objRefBindings)
                {
                    var newPath = remapper.Remap(objRefBinding.path, false);

                    remapped |= newPath != objRefBinding.path;

                    var newObjRefBinding = objRefBinding;
                    newObjRefBinding.path = newPath;
                    AnimationUtility.SetObjectReferenceCurve(newClip, newObjRefBinding, AnimationUtility.GetObjectReferenceCurve(oldClip, objRefBinding));
                }

                if (remapped)
                {
                    clipContainer.newClip = newClip;
                }
            }
        }

        private bool RunPassesAtStage(BuildStage stage)
        {
            var passes = _plugMgr.GetSortedBuildPassesAtStage(stage);

            if (passes == null)
            {
                Context.Report.LogErrorLocalized(t, LogLabel, MessageCode.UnresolvedOrCyclicDependencies);
                return false;
            }

            foreach (var pass in passes)
            {
                var result = pass.Invoke(Context);

                if (!result)
                {
                    Context.Report.LogErrorLocalized(t, LogLabel, MessageCode.PassHasErrors, pass.Identifier);
                    return false;
                }
            }

            return true;
        }

        public void RunStages(BuildStage beginStage = BuildStage.Pre, BuildStage endStage = BuildStage.Post)
        {
            for (var stage = beginStage; stage <= endStage; stage++)
            {
                switch (stage)
                {
                    case BuildStage.Pre:
                        SetUp();
                        if (!RunPassesAtStage(BuildStage.Pre)) return;
                        break;
                    case BuildStage.Preparation:
                    case BuildStage.Generation:
                    case BuildStage.Transpose:
                    case BuildStage.Optimization:
                        if (!RunPassesAtStage(stage)) return;
                        break;
                    case BuildStage.Post:
                        if (!RunPassesAtStage(BuildStage.Post)) return;
                        TearDown();
                        break;
                }
            }
        }
    }
}
