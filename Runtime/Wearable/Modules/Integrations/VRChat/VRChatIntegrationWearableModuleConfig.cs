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

#if VRC_SDK_VRCSDK3
using Chocopoi.DressingFramework.Serialization;

namespace Chocopoi.DressingFramework.Integration.VRChat.Modules
{
    /// <summary>
    /// VRChat integration module config
    /// </summary>
    public class VRChatIntegrationWearableModuleConfig : IModuleConfig
    {
        /// <summary>
        /// Module identifier
        /// </summary>
        public const string ModuleIdentifier = "com.chocopoi.dressingtools.integrations.vrchat.wearable";

        /// <summary>
        /// Custom cabinet toggle name. This overrides the original wearable info name.
        /// </summary>
        public string customCabinetToggleName;

        /// <summary>
        /// Enable cabinet thumbnails to use in toggle icons
        /// </summary>
        public bool cabinetThumbnails;

        /// <summary>
        /// Constructs new VRChat integration module config
        /// </summary>
        public VRChatIntegrationWearableModuleConfig()
        {
            customCabinetToggleName = null;
            cabinetThumbnails = true;
        }
    }
}
#endif
