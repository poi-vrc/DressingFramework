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

using System.Diagnostics.CodeAnalysis;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using UnityEditor;
using UnityEngine;
using LogType = Chocopoi.DressingFramework.Logging.LogType;

namespace Chocopoi.DressingFramework.Detail.DK.Triggers.PlayMode
{
    [InitializeOnLoad]
    [ExcludeFromCodeCoverage]
    internal static class AutoPlayModeBuild
    {
        private static PlayModeBuildTriggerSpawner.Lifecycle s_lifeCycle = PlayModeBuildTriggerSpawner.Lifecycle.Awake;

        static AutoPlayModeBuild()
        {
            PlayModeBuildTriggerSpawner.OnAvatarLifecycle = OnAvatarLifecycle;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnAvatarLifecycle(PlayModeBuildTriggerSpawner.Lifecycle stage, GameObject avatarGameObject)
        {
            if (s_lifeCycle == stage)
            {
                var ab = new AvatarBuilder(avatarGameObject);
                var dkReport = (DKReport)ab.Context.Report;

                DKEditorUtils.TrySafeRun("BuildAvatarCallback", dkReport, () => ab.RunStages());
                ReportWindow.AddReport(avatarGameObject.name, dkReport);
                if (dkReport.HasLogType(LogType.Error))
                {
                    ReportWindow.ShowWindow();
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingEditMode:
                    ReportWindow.Reset();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    s_lifeCycle = PlayModeBuildTriggerSpawner.Lifecycle.Start;
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    // remove previous generated files
                    AssetDatabase.DeleteAsset(DKNativeContext.GeneratedAssetsPath);
                    s_lifeCycle = PlayModeBuildTriggerSpawner.Lifecycle.Awake;
                    break;
            }
        }
    }
}
