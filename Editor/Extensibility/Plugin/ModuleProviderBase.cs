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

using System.Collections.ObjectModel;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chocopoi.DressingFramework.Extensibility.Plugin
{
    public abstract class ModuleProviderBase : IHook
    {
        public abstract string Identifier { get; }
        public abstract string FriendlyName { get; }

        /// <summary>
        /// Allow multiple module to exist in the same configuration
        /// </summary>
        public abstract bool AllowMultiple { get; }

        /// <summary>
        /// Deserialize module configuration
        /// </summary>
        /// <param name="jObject">JObject</param>
        /// <returns>Module config</returns>
        public abstract IModuleConfig DeserializeModuleConfig(JObject jObject);

        /// <summary>
        /// Serialize module configuration
        /// </summary>
        /// <param name="moduleConfig">Module config</param>
        /// <returns>Serialized JSON string</returns>
        public virtual string SerializeModuleConfig(IModuleConfig moduleConfig) => JsonConvert.SerializeObject(moduleConfig);

        /// <summary>
        /// Create a new module configuration
        /// </summary>
        /// <returns>Empty module config</returns>
        public abstract IModuleConfig NewModuleConfig();
    }

    public abstract class CabinetModuleProviderBase : ModuleProviderBase, ICabinetHook
    {
        public abstract CabinetApplyConstraint Constraint { get; }

        /// <summary>
        /// Shortcut to create a cabinet apply constraint builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        /// <returns>Builder</returns>
        protected CabinetApplyConstraintBuilder ApplyAtStage(CabinetApplyStage stage, CabinetHookStageRunOrder order) => new CabinetApplyConstraintBuilder(stage, order);

        /// <summary>
        /// Invoke this hook
        /// </summary>
        /// <param name="cabCtx">Apply cabinet context</param>
        /// <param name="modules">Associated cabinet modules</param>
        /// <param name="isPreview">Whether this is a preview apply</param>
        /// <returns>Return false to stop continuing execution</returns>
        public abstract bool Invoke(ApplyCabinetContext cabCtx, ReadOnlyCollection<CabinetModule> modules, bool isPreview);
    }

    public abstract class WearableModuleProviderBase : ModuleProviderBase, IWearableHook
    {
        public abstract WearableApplyConstraint Constraint { get; }

        /// <summary>
        /// Shortcut to create a wearable apply constraint builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        /// <returns>Builder</returns>
        protected WearableApplyConstraintBuilder ApplyAtStage(CabinetApplyStage stage) => new WearableApplyConstraintBuilder(stage);

        /// <summary>
        /// Invoke this hook
        /// </summary>
        /// <param name="cabCtx">Apply cabinet context</param>
        /// <param name="wearCtx">Apply wearable context</param>
        /// <param name="modules">Associated wearable modules</param>
        /// <param name="isPreview">Whether this is a preview apply</param>
        /// <returns>Return false to stop continuing execution</returns>
        public abstract bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview);
    }
}
