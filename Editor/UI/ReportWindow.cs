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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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

        public static readonly Dictionary<string, DKReport> Reports = new Dictionary<string, DKReport>();

        private string _selectedReportName;

        private Dictionary<LogType, List<LogEntry>> _logEntries;

        private Vector2 scrollPos;

        [MenuItem("Tools/chocopoi/DK Report Window", false, 0)]
        public static void ShowWindow()
        {
            var window = (ReportWindow)GetWindow(typeof(ReportWindow));
            window.titleContent = new GUIContent(t._("report.editor.title"));
            window.Show();
        }

        public static void Reset()
        {
            Reports.Clear();
        }

        public static void AddReport(string reportName, DKReport report)
        {
            Reports.Add($"{reportName} ({System.DateTime.Now:yyyy-MM-dd HH:mm:ss.fff})", report);
        }

        public void OnGUI()
        {
            if (Reports.Count != 0)
            {
                if (_selectedReportName == null || !Reports.ContainsKey(_selectedReportName))
                {
                    _logEntries = null;
                    _selectedReportName = Reports.Keys.First();
                }

                var report = Reports[_selectedReportName];

                if (_logEntries == null)
                {
                    _logEntries = report.GetLogEntriesAsDictionary();
                }

                var keys = Reports.Keys.ToArray();
                _selectedReportName = keys[GUILayout.SelectionGrid(Array.IndexOf(keys, _selectedReportName), keys, 1)];

                EditorGUILayout.Separator();

                if (GUILayout.Button(t._("report.editor.saveReportToFile")))
                {
                    var dateTime = DateTime.Parse(report.GeneratedTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    var path = EditorUtility.SaveFilePanel(t._("framework.name"), "", $"dk-report-{dateTime:yyyy-MM-dd-HH-mm-ss-fff}", "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            File.WriteAllText(path, report.Serialize());
                        }
                        catch (IOException e)
                        {
                            Debug.LogException(e);
                            EditorUtility.DisplayDialog(t._("framework.name"), e.Message, t._("common.dialog.btn.ok"));
                        }
                    }
                }

                //Result

                if (report.HasLogType(LogType.Error))
                {
                    EditorGUILayout.HelpBox(t._("report.editor.helpbox.resultError"), MessageType.Error);
                }
                else if (report.HasLogType(LogType.Warning))
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
