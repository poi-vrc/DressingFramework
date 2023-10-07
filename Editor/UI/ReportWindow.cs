/*
 * File: ReportWindow.cs
 * Project: DressingTools
 * Created Date: Thursday, August 10th 2023, 11:42:41 pm
 * Author: chocopoi (poi@chocopoi.com)
 * -----
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingTools.
 * 
 * DressingTools is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingTools is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingTools. If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Logging;
using UnityEditor;
using UnityEngine;
using LogType = Chocopoi.DressingFramework.Logging.LogType;

namespace Chocopoi.DressingFramework.UI
{
    [ExcludeFromCodeCoverage]
    public class ReportWindow : EditorWindow
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;

        private DKReport _report;

        private Dictionary<LogType, List<LogEntry>> _logEntries;

        private Vector2 scrollPos;
        public static void ShowWindow(DKReport report)
        {
            var window = (ReportWindow)GetWindow(typeof(ReportWindow));
            window.titleContent = new GUIContent(t._("report.editor.title"));
            window.Show();

            window._report = report;
        }

        public void ResetReport()
        {
            _report = null;
            _logEntries = null;
        }

        public void OnGUI()
        {
            if (_report != null)
            {
                if (_logEntries == null)
                {
                    _logEntries = _report.GetLogEntriesAsDictionary();
                }
                //Result

                if (_report.HasLogType(LogType.Error))
                {
                    EditorGUILayout.HelpBox(t._("report.editor.helpbox.resultError"), MessageType.Error);
                }
                else if (_report.HasLogType(LogType.Warning))
                {
                    EditorGUILayout.HelpBox(t._("report.editor.helpbox.resultWarn"), MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox(t._("report.editor.helpbox.resultSuccess"), MessageType.Info);
                }

                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(t._("report.editor.label.errors", _logEntries.ContainsKey(LogType.Error) ? _logEntries[LogType.Error].Count : 0));
                    GUILayout.Label(t._("report.editor.label.warnings", _logEntries.ContainsKey(LogType.Warning) ? _logEntries[LogType.Warning].Count : 0));
                    GUILayout.Label(t._("report.editor.label.infos", _logEntries.ContainsKey(LogType.Info) ? _logEntries[LogType.Info].Count : 0));
                }
                EditorGUILayout.EndHorizontal();

                // show logs
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                {
                    if (_logEntries.ContainsKey(LogType.Error))
                    {
                        foreach (var logEntry in _logEntries[LogType.Error])
                        {
                            EditorGUILayout.HelpBox(logEntry.message, MessageType.Error);
                        }
                    }

                    if (_logEntries.ContainsKey(LogType.Warning))
                    {

                        foreach (var logEntry in _logEntries[LogType.Warning])
                        {
                            EditorGUILayout.HelpBox(logEntry.message, MessageType.Warning);
                        }
                    }

                    if (_logEntries.ContainsKey(LogType.Info))
                    {
                        foreach (var logEntry in _logEntries[LogType.Info])
                        {
                            EditorGUILayout.HelpBox(logEntry.message, MessageType.Info);
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox(t._("report.helpbox.noReport"), MessageType.Warning);
            }
        }
    }
}
