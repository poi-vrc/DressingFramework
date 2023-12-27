/*
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingTools.
 * 
 * DressingTools is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingTools is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingTools. If not, see <https://www.gnu.org/licenses/>.
 */

#if DK_MA && VRC_SDK_VRCSDK3
using System.Collections.Generic;
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Detail.MA
{
    internal class MAMenuStore : MenuStore
    {
        public class MenuAppendEntry
        {
            public string name;
            // public Texture2D icon;
            public VRCExpressionsMenu menu;
        }

        internal readonly List<MenuAppendEntry> RootMenusToAppend;
        internal readonly Dictionary<VRCExpressionsMenu, List<MenuAppendEntry>> OtherMenusToAppend;

        private readonly Context _ctx;

        public MAMenuStore(Context ctx)
        {
            RootMenusToAppend = new List<MenuAppendEntry>();
            OtherMenusToAppend = new Dictionary<VRCExpressionsMenu, List<MenuAppendEntry>>();
            _ctx = ctx;
        }

        private void AddOtherAppendMenu(VRCExpressionsMenu targetVrcMenu, string name, MenuGroup menuGroup)
        {
            if (!OtherMenusToAppend.TryGetValue(targetVrcMenu, out var menuAppends))
            {
                OtherMenusToAppend[targetVrcMenu] = menuAppends = new List<MenuAppendEntry>();
            }
            Debug.Log("Add menu append: " + name);
            menuAppends.Add(new MenuAppendEntry()
            {
                name = name,
                menu = VRCMenuUtils.MenuGroupToVRCMenu(menuGroup, _ctx)
            });
        }

        public override void AppendMenu(string name, Texture2D icon, MenuGroup menuGroup, IMenuRepository targetMenuRepo = null)
        {
            Debug.Log("Add menu append2: " + name);
            if (targetMenuRepo == null)
            {
                // MA doesn't support icons
                RootMenusToAppend.Add(new MenuAppendEntry()
                {
                    name = name,
                    menu = VRCMenuUtils.MenuGroupToVRCMenu(menuGroup, _ctx)
                });
            }
            else if (targetMenuRepo is VRCMenuWrapper menu)
            {
                // attempt to not to do this ourselves, but throw to MA to manage it
                AddOtherAppendMenu(menu.GetContainingMenu(), name, menuGroup);
            }
            else
            {
                targetMenuRepo.Add(new MenuItem()
                {
                    name = name,
                    icon = icon,
                    subMenu = menuGroup
                });
            }
        }

        internal override void OnEnable() { }

        internal override void OnDisable() { }
    }
}
#endif
