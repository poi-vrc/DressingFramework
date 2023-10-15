/*
 * File: DTReport.cs
 * Project: DressingFramework
 * Created Date: Thursday, August 10th 2023, 11:42:41 pm
 * Author: chocopoi (poi@chocopoi.com)
 * -----
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

using System;
using System.Collections.Generic;
using System.Globalization;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Serialization;
using Newtonsoft.Json;

namespace Chocopoi.DressingFramework.Logging
{
    /// <summary>
    /// Log type
    /// </summary>
    [Serializable]
    public enum LogType
    {
        Error = -1,
        Info = 0,
        Warning = 1,
        Debug = 2,
        Trace = 3
    }

    /// <summary>
    /// Log entry
    /// </summary>
    [Serializable]
    public class LogEntry
    {
        /// <summary>
        /// Log type
        /// </summary>
        public LogType type;

        /// <summary>
        /// Log label
        /// </summary>
        public string label;

        /// <summary>
        /// Human-readable message. Preferably a localized string.
        /// </summary>
        public string message;

        /// <summary>
        /// Message code. Used for machines to identify this message.
        /// </summary>
        public string code;
    }

    /// <summary>
    /// Report
    /// </summary>
    [Serializable]
    public class DKReport
    {
        private static readonly SerializationVersion CurrentReportVersion = new SerializationVersion(1, 0, 0);

        /// <summary>
        /// Serialization version of this DKReport
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public SerializationVersion Version { get; private set; }

        /// <summary>
        /// Generated time
        /// </summary>
        [JsonProperty(PropertyName = "generatedTime")]
        public string GeneratedTime { get; private set; }

        /// <summary>
        /// Log entries
        /// </summary>
        [JsonProperty(PropertyName = "logEntries")]
        public List<LogEntry> LogEntries { get; private set; }

        /// <summary>
        /// Constructs a new report
        /// </summary>
        public DKReport()
        {
            Version = CurrentReportVersion;
            GeneratedTime = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            LogEntries = new List<LogEntry>();
        }

        /// <summary>
        /// Serialize this into JSON
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Get log entries grouped by log type in a dictionary
        /// </summary>
        /// <returns>Dictionary</returns>
        public Dictionary<LogType, List<LogEntry>> GetLogEntriesAsDictionary()
        {
            var dict = new Dictionary<LogType, List<LogEntry>>();
            foreach (var entry in LogEntries)
            {
                if (!dict.ContainsKey(entry.type))
                {
                    dict.Add(entry.type, new List<LogEntry>());
                }
                dict[entry.type].Add(entry);
            }
            return dict;
        }

        /// <summary>
        /// Return whether there exists a log entry with the code
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns>Exists</returns>
        public bool HasLogCode(string code)
        {
            foreach (var entry in LogEntries)
            {
                if (entry.code == code)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return whether there exists a log entry with the code and type
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="code">Code</param>
        /// <returns>Exists</returns>
        public bool HasLogCodeByType(LogType type, string code)
        {
            foreach (var entry in LogEntries)
            {
                if (entry.type == type && entry.code == code)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return whether there exists a log entry with the type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Exists</returns>
        public bool HasLogType(LogType type)
        {
            foreach (var entry in LogEntries)
            {
                if (entry.type == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void Log(LogType type, string label, string message, string code = null)
        {
#if DK_PRINT_REPORT_LOG
            // TODO: do not output debug, trace unless specified in settings
            if (code != null)
            {
                Debug.Log(string.Format("[DressingFramework] [{0}] [{1}] ({2}) {3}", label, type, code, message));
            }
            else
            {
                Debug.Log(string.Format("[DressingFramework] [{0}] [{1}] {2}", label, type, message));
            }
#endif
            LogEntries.Add(new LogEntry() { type = type, label = label, code = code, message = message });
        }

        /// <summary>
        /// Append a report to this report
        /// </summary>
        /// <param name="report"></param>
        public void AppendReport(DKReport report)
        {
            LogEntries.AddRange(new List<LogEntry>(report.LogEntries));
        }

        /// <summary>
        /// Log an exception to this report
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="exception">Exception</param>
        /// <param name="extraMessage">Extra human-readable message. Preferably localized.</param>
        /// <param name="extraCode">Extra message code for machines</param>
        public void LogException(string label, Exception exception, string extraMessage = null, string extraCode = null)
        {
            LogError(label, extraMessage != null ? string.Format("{0}: {1}", extraMessage, exception.ToString()) : exception.ToString(), extraCode);
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void LogError(string label, string message, string code = null)
        {
            Log(LogType.Error, label, message, code);
        }

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void LogInfo(string label, string message, string code = null)
        {
            Log(LogType.Info, label, message, code);
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void LogWarn(string label, string message, string code = null)
        {
            Log(LogType.Warning, label, message, code);
        }

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void LogDebug(string label, string message, string code = null)
        {
            Log(LogType.Debug, label, message, code);
        }

        /// <summary>
        /// Log trace
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        public void LogTrace(string label, string message, string code = null)
        {
            Log(LogType.Trace, label, message, code);
        }

        /// <summary>
        /// Log localized
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="type">Type</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogLocalized(I18nTranslator t, LogType type, string label, string code, params object[] args)
        {
            Log(type, label, t._(code, args), code);
        }

        /// <summary>
        /// Log exception localized
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="exception">Exception</param>
        /// <param name="extraCode">Translation key for extra message, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogExceptionLocalized(I18nTranslator t, string label, Exception exception, string extraCode = null, params object[] args)
        {
            LogException(label, exception, extraCode != null ? t._(extraCode, args) : null, extraCode);
        }

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogErrorLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Error, label, code, args);
        }

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogInfoLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Info, label, code, args);
        }

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogWarnLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Warning, label, code, args);
        }

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogDebugLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Debug, label, code, args);
        }

        /// <summary>
        /// Log trace
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        public void LogTraceLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Trace, label, code, args);
        }
    }
}
