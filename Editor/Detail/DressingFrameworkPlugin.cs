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

using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Detail
{
    internal class DressingFrameworkPlugin : Plugin
    {
        public const string PluginIdentifier = "com.chocopoi.vrc.dressingframework.plugin";

        public override string Identifier => PluginIdentifier;
        public override string FriendlyName => "DressingFramework";
        public override PluginConstraint Constraint => PluginConstraint.Empty;

        public override void OnEnable()
        {
            RegisterInternalPasses();
        }

        private void RegisterInternalPasses()
        {
            RegisterBuildPass(new DK.Passes.FlushMenuStorePass());
#if DK_VRCSDK3A
            RegisterBuildPass(new DK.Passes.VRChat.CloneVRCAnimLayersPass());
            RegisterBuildPass(new DK.Passes.VRChat.CloneVRCExMenuAndParamsPass());
            RegisterBuildPass(new DK.Passes.VRChat.ScanVRCAnimationsPass());
            RegisterBuildPass(new DK.Passes.VRChat.ApplyVRCExParamsPass());
#endif
        }

        public override void OnDisable()
        {
        }
    }
}
