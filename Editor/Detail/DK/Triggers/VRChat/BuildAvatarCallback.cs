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

#if DK_VRCSDK3A
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Localization;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;
using LogType = Chocopoi.DressingFramework.Logging.LogType;

namespace Chocopoi.DressingFramework.Detail.DK.Triggers.VRChat
{
    [ExcludeFromCodeCoverage]
    internal class BuildAvatarCallback : IVRCSDKPreprocessAvatarCallback, IVRCSDKPostprocessAvatarCallback
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;
        private const string LogLabel = "BuildAvatarCallback";

        public int callbackOrder => -12000;

        // for mocking
        internal UI ui = new UnityEditorUI();

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            ReportWindow.Reset();

            var ab = new AvatarBuilder(avatarGameObject);
            var dkReport = (DKReport)ab.Context.Report;

            DKEditorUtils.TrySafeRun("BuildAvatarCallback", dkReport, () => ab.RunStages());
            ReportWindow.AddReport(avatarGameObject.name, dkReport);
            if (dkReport.HasLogType(LogType.Error))
            {
                ReportWindow.ShowWindow();
            }

            return !dkReport.HasLogType(LogType.Error);
        }

        public void OnPostprocessAvatar()
        {
        }

        // UI abstraction layer for mocking
        internal interface UI
        {
            void ShowErrorPreprocessingAvatarDialog();
            void ShowReportWindow();
        }

        // Unity editor UI
        internal class UnityEditorUI : UI
        {
            public void ShowErrorPreprocessingAvatarDialog()
            {
                EditorUtility.DisplayDialog(t._("framework.name"), t._("triggers.vrc.dialog.msg.errorPreprocessingReferReportWindow"), t._("common.dialog.btn.ok"));
            }

            public void ShowReportWindow()
            {
                ReportWindow.ShowWindow();
            }
        }
    }

#region IEditorOnly Workaround
    // Copyright (c) 2022 bd_ under MIT License
    // temporary workaround with VRCSDK to not remove IEditorOnly objects at early stage which causes problems
    // code referenced from MA: https://github.com/bdunderscore/modular-avatar/blob/main/Packages/nadena.dev.modular-avatar/Editor/PreventStripTagObjects.cs
    // https://feedback.vrchat.com/sdk-bug-reports/p/ieditoronly-components-should-be-destroyed-late-in-the-build-process

    [InitializeOnLoad]
    [ExcludeFromCodeCoverage]
    internal static class RemoveOriginalEditorOnlyCallback
    {
        static RemoveOriginalEditorOnlyCallback()
        {
            EditorApplication.delayCall += () =>
            {
                // obtain the private static field via reflection
                var callbackStaticField = typeof(VRCBuildPipelineCallbacks).GetField("_preprocessAvatarCallbacks", BindingFlags.Static | BindingFlags.NonPublic);
                var callbacks = (List<IVRCSDKPreprocessAvatarCallback>)callbackStaticField.GetValue(null);

                // remove RemoveAvatarEditorOnly
                var modifiedCallbacks = callbacks.Where(c => !(c is RemoveAvatarEditorOnly)).ToList();
                callbackStaticField.SetValue(null, modifiedCallbacks);
            };
        }
    }

    [ExcludeFromCodeCoverage]
    internal class ReplacementRemoveAvatarEditorOnly : IVRCSDKPreprocessAvatarCallback
    {
        public int callbackOrder => -1024;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            // iterate the avatar transforms to see if has tag EditorOnly
            foreach (var transform in avatarGameObject.GetComponentsInChildren<Transform>(true))
            {
                // remove if has tag
                if (transform != null && transform.CompareTag("EditorOnly"))
                {
                    Object.DestroyImmediate(transform.gameObject);
                }
            }
            return true;
        }
    }

    [ExcludeFromCodeCoverage]
    internal class ReplacementRemoveIEditorOnly : IVRCSDKPreprocessAvatarCallback
    {
        // execute the callback at a very very late order
        public int callbackOrder => int.MaxValue;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            // iterate all IEditorOnly objects
            foreach (var component in avatarGameObject.GetComponentsInChildren<IEditorOnly>(true))
            {
                Object.DestroyImmediate(component as Object);
            }
            return true;
        }
    }
#endregion IEditorOnly Workaround

}
#endif
