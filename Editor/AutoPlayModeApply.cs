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
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.UI;
using UnityEditor;

namespace Chocopoi.DressingFramework
{
    [InitializeOnLoad]
    [ExcludeFromCodeCoverage]
    internal static class AutoPlayModeApply
    {
        private static DKRuntimeUtils.LifecycleStage s_applyLifeCycle = DKRuntimeUtils.LifecycleStage.Awake;

        static AutoPlayModeApply()
        {
            DKRuntimeUtils.OnCabinetLifecycle = OnCabinetLifecycle;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnCabinetLifecycle(DKRuntimeUtils.LifecycleStage stage, ICabinet cabinet)
        {
            if (s_applyLifeCycle == stage)
            {
                var report = new DKReport();
                new CabinetApplier(report, cabinet).RunStages();
                if (report.HasLogType(LogType.Error))
                {
                    ReportWindow.ShowWindow(report);
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    s_applyLifeCycle = DKRuntimeUtils.LifecycleStage.Start;
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    // remove previous generated files
                    AssetDatabase.DeleteAsset(ApplyCabinetContext.GeneratedAssetsPath);
                    s_applyLifeCycle = DKRuntimeUtils.LifecycleStage.Awake;
                    break;
            }
        }
    }
}
