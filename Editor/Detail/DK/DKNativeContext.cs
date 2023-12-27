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
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Menu;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.DK
{
    /// <summary>
    /// DK native implementation context
    /// </summary>
    internal class DKNativeContext : Context
    {
        /// <summary>
        /// Folder of generated assets
        /// </summary>
        public const string GeneratedAssetsFolderName = "_DTGeneratedAssets";
        /// <summary>
        /// Full path of generated assets folder
        /// </summary>
        public const string GeneratedAssetsPath = "Assets/" + GeneratedAssetsFolderName;

        private const string AssetNamePrefix = "cpDT_";

        public override object RuntimeContext => null;
        public override Report Report => _report;

        private readonly DKReport _report;

        public DKNativeContext(GameObject avatarGameObject) : base(avatarGameObject)
        {
            _report = new DKReport();
            AddContextFeature(new AnimationStore(this));
            AddContextFeature(new DKMenuStore(this));
        }

        /// <summary>
        /// Make the specified name unique to this avatar with asset path
        /// Note: Only guarantee uniqueness of the same name against other avatars, not within the same avatar.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Unique name asset path</returns>
        public string MakeUniqueAssetPath(string name)
        {
            return $"{GeneratedAssetsPath}/{AssetNamePrefix}{MakeUniqueName(name)}";
        }

        public override void CreateUniqueAsset(Object obj, string name)
        {
            AssetDatabase.CreateAsset(obj, MakeUniqueAssetPath(name));
        }

        private void RemapAnimations()
        {
            var store = Feature<AnimationStore>();
            var remapper = Feature<PathRemapper>();

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

        internal override void OnDisable()
        {
            RemapAnimations();
            base.OnDisable();
        }
    }
}
