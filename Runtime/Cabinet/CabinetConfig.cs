﻿/*
 * File: CabinetConfig.cs
 * Project: DressingFramework
 * Created Date: Saturday, Aug 24th 2023, 05:08:11 pm
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

using System.Collections.Generic;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Serialization;

namespace Chocopoi.DressingFramework.Cabinet
{
    /// <summary>
    /// Cabinet config
    /// </summary>
    public class CabinetConfig
    {
        /// <summary>
        /// Current config version
        /// </summary>
        public static readonly SerializationVersion CurrentConfigVersion = new SerializationVersion(1, 0, 0);

        /// <summary>
        /// Config version
        /// </summary>
        public SerializationVersion version;

        /// <summary>
        /// Avatar armature name
        /// </summary>
        public string avatarArmatureName;

        /// <summary>
        /// Group dynamics
        /// </summary>
        public bool groupDynamics;

        /// <summary>
        /// Group dynamics and separate into different GameObjects
        /// </summary>
        public bool groupDynamicsSeparateGameObjects;

        /// <summary>
        /// Animation write defaults. Animation-related modules should respect this option
        /// </summary>
        public bool animationWriteDefaults;

        /// <summary>
        /// Cabinet modules
        /// </summary>
        public List<CabinetModule> modules;

        /// <summary>
        /// Initialize a new cabinet configuration
        /// </summary>
        public CabinetConfig()
        {
            version = CurrentConfigVersion;
            avatarArmatureName = "Armature";
            groupDynamics = true;
            groupDynamicsSeparateGameObjects = true;
            animationWriteDefaults = true;
            modules = new List<CabinetModule>();
        }

        public T FindModuleConfig<T>() where T : IModuleConfig
        {
            var list = new List<T>();
            foreach (var module in modules)
            {
                if (module.config is T moduleConfig)
                {
                    return moduleConfig;
                }
            }
            return default;
        }

        public List<T> FindModuleConfigs<T>() where T : IModuleConfig
        {
            var list = new List<T>();
            foreach (var module in modules)
            {
                if (module.config is T moduleConfig)
                {
                    list.Add(moduleConfig);
                }
            }
            return list;
        }

        public CabinetModule FindModule(string moduleName)
        {
            foreach (var module in modules)
            {
                if (module.moduleName == moduleName)
                {
                    return module;
                }
            }
            return null;
        }

        public List<CabinetModule> FindModules(string moduleName)
        {
            var list = new List<CabinetModule>();
            foreach (var module in modules)
            {
                if (module.moduleName == moduleName)
                {
                    list.Add(module);
                }
            }
            return list;
        }
    }
}
