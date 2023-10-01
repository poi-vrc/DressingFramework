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

using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Extensibility.Plugin
{
    /// <summary>
    /// Plugin entry interface
    /// </summary>
    internal interface IPlugin
    {
        /// <summary>
        /// Plugin identifier
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Human-readable friendly name
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Execution dependency and constraint of this plugin
        /// </summary>
        ExecutionConstraint Constraint { get; }

        /// <summary>
        /// On enable this plugin
        /// </summary>
        void OnEnable();

        /// <summary>
        /// On disable this plugin
        /// </summary>
        void OnDisable();
    }
}
