/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */
#if UNITY_EDITOR
using Chocopoi.DressingFramework.Localization;
using UnityEditor;

namespace Chocopoi.DressingFramework.Detail.NDMF
{
    [InitializeOnLoad]
    public static class UnsupportedVersionChecker
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;

        private static bool IsMAUnsupported()
        {
#if DK_MA_UNSUPPORTED
            return true;
#else
            return false;
#endif
        }

        private static bool IsNDMFUnsupported()
        {
#if DK_NDMF_UNSUPPORTED
            return true;
#else
            return false;
#endif
        }

        static UnsupportedVersionChecker()
        {
            EditorApplication.delayCall += () =>
            {
                if (IsMAUnsupported())
                {
                    EditorUtility.DisplayDialog(t._("framework.name"), t._("detail.ndmf.unsupportedVersionChecker.unsupportedMaVersionDetected"), t._("common.dialog.btn.ok"));
                }

                if (IsNDMFUnsupported())
                {
                    EditorUtility.DisplayDialog(t._("framework.name"), t._("detail.ndmf.unsupportedVersionChecker.unsupportedNdmfVersionDetected"), t._("common.dialog.btn.ok"));
                }
            };
        }
    }
}
#endif
