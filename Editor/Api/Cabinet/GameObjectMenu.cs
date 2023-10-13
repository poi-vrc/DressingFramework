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
using Chocopoi.DressingFramework;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.UI;
using Chocopoi.DressingTools.Api.Cabinet;
using UnityEditor;
using UnityEngine;
using LogType = Chocopoi.DressingFramework.Logging.LogType;

namespace Chocopoi.DressingTools.Api
{
    [ExcludeFromCodeCoverage]
    internal static class GameObjectMenu
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;

        // note that in order for a menu item in "GameObject/" to be propagated to the
        // hierarchy Create dropdown and hierarchy context menu, it must be grouped with
        // the other GameObject creation menu items. This can be achieved by setting its priority to 10 
        private const int MenuItemPriority = 21;

        [MenuItem("GameObject/DressingTools/Apply cabinet as copy", false, MenuItemPriority)]
        public static void ApplyCabinetAsCopy(MenuCommand menuCommand)
        {
            if (!(menuCommand.context is GameObject))
            {
                return;
            }

            var obj = (GameObject)menuCommand.context;

            // traverse up until the cabinet is found
            var p = obj.transform;
            DTCabinet originalCabinet = null;
            while (p != null)
            {
                if (p.TryGetComponent(out originalCabinet))
                {
                    break;
                }
                p = p.parent;
            }

            if (originalCabinet == null)
            {
                EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.noCabinetFoundInSelection"), t._("common.dialog.btn.ok"));
                return;
            }

            var avatarCopy = Object.Instantiate(originalCabinet.AvatarGameObject);
            avatarCopy.name = $"{originalCabinet.AvatarGameObject.name}_{System.DateTime.Now:yyyyMMddHHmmss}";
            var cabinetCopy = avatarCopy.GetComponent<DTCabinet>();

            var report = new DKReport();
            new CabinetApplier(report, cabinetCopy).RunStages();
            if (report.HasLogType(LogType.Error))
            {
                ReportWindow.ShowWindow(report);
            }
        }
    }
}
