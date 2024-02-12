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

using System;
using Chocopoi.DressingFramework.Localization;

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
    /// Report
    /// </summary>
    internal interface IReport
    {
        /// <summary>
        /// Return whether there exists a log entry with the type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Exists</returns>
        bool HasLogType(LogType type);

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void Log(LogType type, string label, string message, string code = null);

        /// <summary>
        /// Log an exception to this report
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="exception">Exception</param>
        /// <param name="extraMessage">Extra human-readable message. Preferably localized.</param>
        /// <param name="extraCode">Extra message code for machines</param>
        void LogException(string label, Exception exception, string extraMessage = null, string extraCode = null);

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void LogError(string label, string message, string code = null);

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void LogInfo(string label, string message, string code = null);

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void LogWarn(string label, string message, string code = null);

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void LogDebug(string label, string message, string code = null);

        /// <summary>
        /// Log trace
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="message">Human-readable message. Preferably localized.</param>
        /// <param name="code">Message code for machines</param>
        void LogTrace(string label, string message, string code = null);

        /// <summary>
        /// Log localized
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="type">Type</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogLocalized(I18nTranslator t, LogType type, string label, string code, params object[] args);

        /// <summary>
        /// Log exception localized
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="exception">Exception</param>
        /// <param name="extraCode">Translation key for extra message, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogExceptionLocalized(I18nTranslator t, string label, Exception exception, string extraCode = null, params object[] args);

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogErrorLocalized(I18nTranslator t, string label, string code, params object[] args);

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogInfoLocalized(I18nTranslator t, string label, string code, params object[] args);

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogWarnLocalized(I18nTranslator t, string label, string code, params object[] args);

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogDebugLocalized(I18nTranslator t, string label, string code, params object[] args);

        /// <summary>
        /// Log trace
        /// </summary>
        /// <param name="t">I18n translator</param>
        /// <param name="label">Label</param>
        /// <param name="code">Translation key, this is also used as message code for machines</param>
        /// <param name="args">Arguments</param>
        void LogTraceLocalized(I18nTranslator t, string label, string code, params object[] args);
    }
}
