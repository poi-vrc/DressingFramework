/*
 * File: CabinetModule.cs
 * Project: DressingFramework
 * Created Date: Thursday, August 24th 2023, 7:53:33 pm
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

using Chocopoi.DressingFramework.Serialization;

namespace Chocopoi.DressingFramework.Cabinet.Modules
{
    /// <summary>
    /// Cabinet module container
    /// </summary>
    public class CabinetModule
    {
        /// <summary>
        /// Module name
        /// </summary>
        public string moduleName;

        /// <summary>
        /// Module config deserialized from provider
        /// </summary>
        public IModuleConfig config;
    }
}
