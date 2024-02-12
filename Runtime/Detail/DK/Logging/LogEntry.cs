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
using Chocopoi.DressingFramework.Logging;

namespace Chocopoi.DressingFramework.Detail.DK.Logging
{
    /// <summary>
    /// Log entry
    /// </summary>
    [Serializable]
    internal class LogEntry
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
}
