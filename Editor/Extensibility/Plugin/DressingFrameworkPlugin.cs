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

using Chocopoi.DressingFramework.Cabinet.Hooks;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Wearable.Hooks;

namespace Chocopoi.DressingFramework.Plugin
{
    internal class DressingFrameworkPlugin : PluginBase
    {
        public const string PluginIdentifier = "com.chocopoi.vrc.dressingframework.plugin";

        public override string Identifier => PluginIdentifier;
        public override string FriendlyName => "DressingFramework";
        public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

        public override void OnEnable()
        {
            RegisterCabinetHook(new RemapAnimationCabinetHook());
            RegisterWearableHook(new GroupDynamicsWearableHook());
        }

        public override void OnDisable()
        {
        }
    }
}
