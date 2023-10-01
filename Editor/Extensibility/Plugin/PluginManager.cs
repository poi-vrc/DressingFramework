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
using System.Collections.Generic;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Extensibility.Plugin
{
    /// <summary>
    /// Plugin manager
    /// </summary>
    public class PluginManager
    {
        private static PluginManager s_instance = null;

        /// <summary>
        /// The Plugin manager instance
        /// </summary>
        public static PluginManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new PluginManager();
                }
                return s_instance;
            }
        }

        private static HashSet<Type> s_pluginTypesCache;

        private Dictionary<string, PluginBase> _plugins;

        private PluginManager()
        {
            _plugins = new Dictionary<string, PluginBase>();

            ScanPlugins();
            InitPlugins();
        }

        private void ScanPlugins()
        {
            s_pluginTypesCache = new HashSet<Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(PluginBase)))
                    {
                        s_pluginTypesCache.Add(type);
                    }
                }
            }
        }

        private void InitPlugins()
        {
            // TODO: customizable enable/disable plugins, now we just enable them all
            foreach (var pluginType in s_pluginTypesCache)
            {
                var plugin = (PluginBase)Activator.CreateInstance(pluginType);
                _plugins[plugin.Identifier] = plugin;
            }

            // TODO: dependency graph via topological sort
            foreach (var plugin in _plugins.Values)
            {
                plugin.OnEnable();
            }
        }

        public List<CabinetHookBase> GetCabinetHooksAtStage(CabinetApplyStage stage, CabinetHookStageRunOrder order)
        {
            var list = new List<CabinetHookBase>();
            foreach (var plugin in _plugins.Values)
            {
                var hooks = plugin.GetAllCabinetHooks();
                foreach (var hook in hooks)
                {
                    if (hook.Constraint.stage == stage && hook.Constraint.order == order)
                    {
                        list.Add(hook);
                    }
                }
            }
            return list;
        }

        public List<CabinetModuleProviderBase> GetCabinetModulesAtStage(CabinetApplyStage stage, CabinetHookStageRunOrder order)
        {
            var list = new List<CabinetModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                var hooks = plugin.GetAllCabinetModuleProviders();
                foreach (var hook in hooks)
                {
                    if (hook.Constraint.stage == stage && hook.Constraint.order == order)
                    {
                        list.Add(hook);
                    }
                }
            }
            return list;
        }

        public List<WearableHookBase> GetWearableHooksAtStage(CabinetApplyStage stage)
        {
            var list = new List<WearableHookBase>();
            foreach (var plugin in _plugins.Values)
            {
                var hooks = plugin.GetAllWearableHooks();
                foreach (var hook in hooks)
                {
                    if (hook.Constraint.stage == stage)
                    {
                        list.Add(hook);
                    }
                }
            }
            return list;
        }

        public List<WearableModuleProviderBase> GetWearableModulesAtStage(CabinetApplyStage stage)
        {
            var list = new List<WearableModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                var modules = plugin.GetAllWearableModuleProviders();
                foreach (var module in modules)
                {
                    if (module.Constraint.stage == stage)
                    {
                        list.Add(module);
                    }
                }
            }
            // TODO: handle duplicates
            return list;
        }

        public List<CabinetModuleProviderBase> GetAllCabinetModuleProviders()
        {
            var list = new List<CabinetModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                list.AddRange(plugin.GetAllCabinetModuleProviders());
            }
            // TODO: handle duplicates
            return list;
        }

        public List<WearableModuleProviderBase> GetAllWearableModuleProviders()
        {
            var list = new List<WearableModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                list.AddRange(plugin.GetAllWearableModuleProviders());
            }
            // TODO: handle duplicates
            return list;
        }

        public List<IFrameworkEventHandler> GetAllFrameworkEventHandlers()
        {
            var list = new List<IFrameworkEventHandler>();
            foreach (var plugin in _plugins.Values)
            {
                list.AddRange(plugin.GetAllFrameworkEventHandlers());
            }
            return list;
        }

        public CabinetModuleProviderBase GetCabinetModuleProvider(string moduleName)
        {
            var list = new List<CabinetModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                list.AddRange(plugin.GetAllCabinetModuleProviders());
            }

            // TODO: handle duplicate module providers, now just return first found
            foreach (var provider in list)
            {
                if (provider.Identifier == moduleName)
                {
                    return provider;
                }
            }

            return null;
        }

        public WearableModuleProviderBase GetWearableModuleProvider(string moduleName)
        {
            var list = new List<WearableModuleProviderBase>();
            foreach (var plugin in _plugins.Values)
            {
                list.AddRange(plugin.GetAllWearableModuleProviders());
            }

            // TODO: handle duplicate module providers, now just return first found
            foreach (var provider in list)
            {
                if (provider.Identifier == moduleName)
                {
                    return provider;
                }
            }

            return null;
        }
    }
}
