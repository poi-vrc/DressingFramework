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
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.NDMF
{
    [InitializeOnLoad]
    public static class ScriptRecompiler
    {
        private const string NDMFPackageName = "nadena.dev.ndmf";
        private const string MAPackageName = "nadena.dev.modular-avatar";
        private const string TempFileName = "Temp/com.chocopoi.vrc.dressingframework.ScriptRecompiler.lastRecompileTime.txt";

        private static ListRequest s_listReq = null;

        static ScriptRecompiler()
        {
            s_listReq = Client.List();
            EditorApplication.update += OnEditorUpdate;
        }

        private static bool CheckNDMFInconsistency(PackageCollection pkgs)
        {
#if DK_NDMF
            return !pkgs.Any(p => p.name == NDMFPackageName);
#else
            return pkgs.Any(p => p.name == NDMFPackageName);
#endif
        }

        private static bool CheckMAInconsistency(PackageCollection pkgs)
        {
#if DK_MA
            return !pkgs.Any(p => p.name == MAPackageName);
#else
            return pkgs.Any(p => p.name == MAPackageName);
#endif
        }

        private static void WriteLastRecompileTime()
        {
            var writer = new StreamWriter(TempFileName);
            writer.WriteLine(DateTime.Now.ToString());
            writer.Close();
        }

        private static bool IsLastRecompileMinuteAgo()
        {
            if (!File.Exists(TempFileName))
            {
                return false;
            }

            var reader = new StreamReader(TempFileName);
            var dateTimeLine = reader.ReadLine();
            reader.Close();
            try
            {
                var dateTime = DateTime.Parse(dateTimeLine);
                if ((DateTime.Now - dateTime).TotalMinutes <= 1)
                {
                    return true;
                }
            }
            catch
            {
                Debug.Log("Unable to parse data time from last recompile time file.");
                File.Delete(TempFileName);
                return false;
            }
            return false;
        }

        private static void OnEditorUpdate()
        {
            if (!s_listReq.IsCompleted)
            {
                return;
            }

            if (CheckNDMFInconsistency(s_listReq.Result) || CheckMAInconsistency(s_listReq.Result))
            {
                if (IsLastRecompileMinuteAgo())
                {
                    Debug.LogError("[DressingFramework] Version defines and package presence inconsistency detected, but last recompile was just a minute ago. Probably there is a compilation error? Recompilation is not requested to prevent endless loop.");
                }
                else
                {
                    Debug.Log("[DressingFramework] Version defines and package presence inconsistency detected, requesting recompilation");
                    WriteLastRecompileTime();
                    CompilationPipeline.RequestScriptCompilation();
                }
            }

            EditorApplication.update -= OnEditorUpdate;
        }
    }
}
#endif
