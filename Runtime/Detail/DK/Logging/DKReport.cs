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
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Serialization;
using Newtonsoft.Json;

namespace Chocopoi.DressingFramework.Detail.DK.Logging
{
    /// <summary>
    /// Report
    /// </summary>
    [Serializable]
    internal class DKReport : Report
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

        public override bool HasLogType(LogType type)
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

        public override void Log(LogType type, string label, string message, string code = null)
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
    }
}
