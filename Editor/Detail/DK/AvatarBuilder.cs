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

using System.Diagnostics;
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Components;
using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            _plugMgr = new PluginManager();
            Context = new DKNativeContext(rootGameObject);
        }

        private void SetUp()
        {
            Context.OnEnable();
        }

        private void TearDown()
        {
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
        }

        private bool RunPassesAtStage(BuildStage stage)
        {
            // Debug.Log($"[DK] =========== {stage} Start ===========");
            var passes = _plugMgr.GetSortedBuildPassesAtStage(BuildRuntime.DK, stage);

            if (passes == null)
            {
                Context.Report.LogErrorLocalized(t, LogLabel, MessageCode.UnresolvedOrCyclicDependencies);
                return false;
            }

            foreach (var pass in passes)
            {
                // Debug.Log($"[DK] -- {pass.Identifier} Start");
                // var sw = new Stopwatch();
                // sw.Start();
                var result = pass.Invoke(Context);
                // sw.Stop();
                // Debug.Log($"[DK] -- {pass.Identifier} End ({sw.Elapsed.TotalSeconds} s)");

                if (!result)
                {
                    Context.Report.LogErrorLocalized(t, LogLabel, MessageCode.PassHasErrors, pass.Identifier);
                    return false;
                }
            }
            // Debug.Log($"[DK] =========== {stage} End ===========");

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
