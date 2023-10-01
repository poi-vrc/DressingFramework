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

#if VRC_SDK_VRCSDK3
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.UI;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;

namespace Chocopoi.DressingFramework.Triggers.VRChat
{
    [InitializeOnLoad]
    public class BuildCabinetCallback : IVRCSDKPreprocessAvatarCallback, IVRCSDKPostprocessAvatarCallback
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;
        private const string LogLabel = "BuildCabinetCallback";

        public int callbackOrder => -10000;

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            var cabinet = DKRuntimeUtils.GetAvatarCabinet(avatarGameObject);
            if (cabinet == null)
            {
                // avatar has no cabinet, skipping
                return true;
            }

            var report = new DKReport();

            try
            {
                // create hook instances
                EditorUtility.DisplayProgressBar(t._("tool.name"), t._("integrations.vrc.progressBar.msg.applyingCabinet"), 0);
                new CabinetApplier(report, cabinet).RunStages();
            }
            catch (System.Exception ex)
            {
                report.LogExceptionLocalized(t, LogLabel, ex, "integrations.vrc.msgCode.error.exceptionProcessAvatar");
            }
            finally
            {
                // show report window if any errors
                if (report.HasLogType(DressingFramework.Logging.LogType.Error))
                {
                    ReportWindow.ShowWindow(report);
                    EditorUtility.DisplayDialog(t._("tool.name"), t._("integrations.vrc.dialog.msg.errorPreprocessingReferReportWindow"), t._("common.dialog.btn.ok"));
                }

                AssetDatabase.SaveAssets();
                // hide the progress bar
                EditorUtility.ClearProgressBar();
            }

            return !report.HasLogType(DressingFramework.Logging.LogType.Error);
        }

        public void OnPostprocessAvatar()
        {
        }
    }

#region IEditorOnly Workaround
    // Copyright (c) 2022 bd_ under MIT License
    // temporary workaround with VRCSDK to not remove IEditorOnly objects at early stage which causes problems
    // code referenced from MA: https://github.com/bdunderscore/modular-avatar/blob/main/Packages/nadena.dev.modular-avatar/Editor/PreventStripTagObjects.cs
    // https://feedback.vrchat.com/sdk-bug-reports/p/ieditoronly-components-should-be-destroyed-late-in-the-build-process

    [InitializeOnLoad]
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
