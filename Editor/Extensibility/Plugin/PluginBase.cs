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

using System.Collections.Generic;
using System.Linq;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Extensibility.Plugin
{
    /// <summary>
    /// Plugin entry base class
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        public virtual string Identifier => GetType().FullName;
        public abstract string FriendlyName { get; }
        public abstract ExecutionConstraint Constraint { get; }

        private Dictionary<string, CabinetHookBase> _cabinetHooks;
        private Dictionary<string, WearableHookBase> _wearableHooks;
        private Dictionary<string, CabinetModuleProviderBase> _cabinetModuleProviders;
        private Dictionary<string, WearableModuleProviderBase> _wearableModuleProviders;
        private List<IFrameworkEventHandler> _dkEventHandlers;

        /// <summary>
        /// Constructs a new plugin. If you override this constructor, make sure you also call the base constructor via `base()`.
        /// </summary>
        public PluginBase()
        {
            _cabinetHooks = new Dictionary<string, CabinetHookBase>();
            _wearableHooks = new Dictionary<string, WearableHookBase>();
            _cabinetModuleProviders = new Dictionary<string, CabinetModuleProviderBase>();
            _wearableModuleProviders = new Dictionary<string, WearableModuleProviderBase>();
            _dkEventHandlers = new List<IFrameworkEventHandler>();
        }

        public abstract void OnEnable();
        public abstract void OnDisable();

        /// <summary>
        /// Register a cabinet hook. Which will be executed on the cabinet apply level.
        /// </summary>
        /// <param name="hook">Hook</param>
        protected void RegisterCabinetHook(CabinetHookBase hook)
        {
            _cabinetHooks[hook.Identifier] = hook;
        }

        /// <summary>
        /// Register a wearable hook. Which will be executed on the wearable apply level.
        /// </summary>
        /// <param name="hook">Hook</param>
        protected void RegisterWearableHook(WearableHookBase hook)
        {
            _wearableHooks[hook.Identifier] = hook;
        }

        /// <summary>
        /// Register a cabinet module provider. Indicates this plugin provides this module.
        /// </summary>
        /// <param name="provider">Module provider</param>
        protected void RegisterCabinetModuleProvider(CabinetModuleProviderBase provider)
        {
            _cabinetModuleProviders[provider.Identifier] = provider;
        }

        /// <summary>
        /// Register a wearable module provider. Indicates this plugin provides this module.
        /// </summary>
        /// <param name="provider">Module provider</param>
        protected void RegisterWearableModuleProvider(WearableModuleProviderBase provider)
        {
            _wearableModuleProviders[provider.Identifier] = provider;
        }

        /// <summary>
        /// Register a framework event handler.
        /// </summary>
        /// <param name="handler">Handler</param>
        protected void RegisterFrameworkEventHandler(IFrameworkEventHandler handler)
        {
            _dkEventHandlers.Add(handler);
        }

        internal List<CabinetHookBase> GetAllCabinetHooks()
        {
            return _cabinetHooks.Values.ToList();
        }

        internal List<WearableHookBase> GetAllWearableHooks()
        {
            return _wearableHooks.Values.ToList();
        }

        internal List<CabinetModuleProviderBase> GetAllCabinetModuleProviders()
        {
            return _cabinetModuleProviders.Values.ToList();
        }

        internal List<WearableModuleProviderBase> GetAllWearableModuleProviders()
        {
            return _wearableModuleProviders.Values.ToList();
        }

        internal List<IFrameworkEventHandler> GetAllFrameworkEventHandlers()
        {
            return _dkEventHandlers;
        }
    }
}
