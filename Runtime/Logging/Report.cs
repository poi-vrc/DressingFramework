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
using Chocopoi.DressingFramework.Localization;

namespace Chocopoi.DressingFramework.Logging
{
    /// <summary>
    /// Abstract report base class
    /// </summary>
    [Serializable]
    internal abstract class Report : IReport
    {
        public abstract bool HasLogType(LogType type);
        public abstract void Log(LogType type, string label, string message, string code = null);

        public void LogException(string label, Exception exception, string extraMessage = null, string extraCode = null)
        {
            LogError(label, extraMessage != null ? string.Format("{0}: {1}", extraMessage, exception.ToString()) : exception.ToString(), extraCode);
        }

        public void LogError(string label, string message, string code = null)
        {
            Log(LogType.Error, label, message, code);
        }

        public void LogInfo(string label, string message, string code = null)
        {
            Log(LogType.Info, label, message, code);
        }

        public void LogWarn(string label, string message, string code = null)
        {
            Log(LogType.Warning, label, message, code);
        }

        public void LogDebug(string label, string message, string code = null)
        {
            Log(LogType.Debug, label, message, code);
        }

        public void LogTrace(string label, string message, string code = null)
        {
            Log(LogType.Trace, label, message, code);
        }

        public void LogLocalized(I18nTranslator t, LogType type, string label, string code, params object[] args)
        {
            Log(type, label, t._(code, args), code);
        }

        public void LogExceptionLocalized(I18nTranslator t, string label, Exception exception, string extraCode = null, params object[] args)
        {
            LogException(label, exception, extraCode != null ? t._(extraCode, args) : null, extraCode);
        }

        public void LogErrorLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Error, label, code, args);
        }

        public void LogInfoLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Info, label, code, args);
        }

        public void LogWarnLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Warning, label, code, args);
        }

        public void LogDebugLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Debug, label, code, args);
        }

        public void LogTraceLocalized(I18nTranslator t, string label, string code, params object[] args)
        {
            LogLocalized(t, LogType.Trace, label, code, args);
        }
    }
}
