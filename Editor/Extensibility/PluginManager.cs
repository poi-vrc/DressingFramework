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

namespace Chocopoi.DressingFramework.Extensibility
{
    /// <summary>
    /// Plugin manager
    /// </summary>
    internal class PluginManager
    {
        private static HashSet<Type> s_pluginTypesCache;
        private readonly Dictionary<string, Plugin> _plugins;

        public PluginManager()
        {
            _plugins = new Dictionary<string, Plugin>();
            ScanPlugins();
            InitPlugins();
        }

        private void ScanPlugins()
        {
            if (s_pluginTypesCache == null)
            {
                s_pluginTypesCache = new HashSet<Type>();

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(Plugin)))
                        {
                            s_pluginTypesCache.Add(type);
                        }
                    }
                }
            }
        }

        private void InitPlugins()
        {
            // TODO: customizable enable/disable plugins, now we just enable them all
            foreach (var pluginType in s_pluginTypesCache)
            {
                var plugin = (Plugin)Activator.CreateInstance(pluginType);
                _plugins[plugin.Identifier] = plugin;
            }

            // TODO: dependency graph via topological sort
            foreach (var plugin in _plugins.Values)
            {
                plugin.OnEnable();
            }
        }

        public List<BuildPass> GetBuildPassesAtStage(BuildRuntime buildRuntime, BuildStage stage)
        {
            var stageHooks = new List<BuildPass>();

            foreach (var plugin in _plugins.Values)
            {
                stageHooks.AddRange(plugin.GetBuildPassesAtStage(buildRuntime, stage));
            }

            return stageHooks;
        }

        public List<BuildPass> GetSortedBuildPassesAtStage(BuildRuntime buildRuntime, BuildStage stage)
        {
            return SortBuildPassesByDependencies(GetBuildPassesAtStage(buildRuntime, stage));
        }

        public static List<BuildPass> SortBuildPassesByDependencies(List<BuildPass> hooks)
        {
            var graph = new DependencyGraph<string>();

            foreach (var hook in hooks)
            {
                graph.Add(hook.Identifier, hook.Constraint);
            }

            if (!graph.IsResolved())
            {
                return null;
            }

            var topOrder = graph.Sort();
            if (topOrder == null)
            {
                return null;
            }

            var output = new List<BuildPass>();

            foreach (var identifier in topOrder)
            {
                foreach (var hook in hooks)
                {
                    if (hook.Identifier == identifier)
                    {
                        output.Add(hook);
                    }
                }
            }

            return output;
        }
    }
}
