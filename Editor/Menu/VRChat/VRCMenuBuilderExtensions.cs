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

#if DK_VRCSDK3A
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    public static class VRCMenuBuilderExtensions
    {
        public static MenuRepositoryBuilder AddSubMenu(this MenuRepositoryBuilder builder, string name, VRCExpressionsMenu vrcMenu, Texture2D icon = null)
        {
            // TODO: parameter on open
            return builder.AddMenuItem(new VRCSubMenuItem()
            {
                Name = name,
                Icon = icon,
                SubMenu = vrcMenu
            });
        }
    }
}
#endif
