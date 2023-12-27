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

using System.Collections.Generic;
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using UnityEditor;
using UnityEngine;
using MenuItem = Chocopoi.DressingFramework.Menu.MenuItem;

namespace Chocopoi.DressingFramework.Detail.DK
{
    internal class DKMenuStore : MenuStore
    {
        private IMenuRepository _rootMenu;
        private Context _ctx;

        public DKMenuStore(Context ctx)
        {
            _ctx = ctx;
            _rootMenu = ObtainRootMenu(ctx);
        }

        private static IMenuRepository ObtainRootMenu(Context ctx)
        {
            IMenuRepository rootMenu;
#if VRC_SDK_VRCSDK3
            {
                if (ctx.AvatarGameObject.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out var avatarDesc))
                {
                    var vrcMenu = ScriptableObject.CreateInstance<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu>();

                    if (avatarDesc.expressionsMenu != null)
                    {
                        EditorUtility.CopySerialized(avatarDesc.expressionsMenu, vrcMenu);
                    }

                    avatarDesc.expressionsMenu = vrcMenu;
                    ctx.CreateUniqueAsset(vrcMenu, "RootMenu");
                    rootMenu = new VRCMenuWrapper(vrcMenu, ctx);
                }
                else
                {
                    // not a VRC avatar
                    rootMenu = new MenuGroup();
                }
            }
#else
            {
                // nothing that we could do, just an empty menu
                rootMenu = new MenuGroup();
                ctx.Report.LogInfo("DKMenuStore", "No compatible menu platform detected. An empty DK menu group is created as root menu.");
            }
#endif
            return rootMenu;
        }

        public override void AppendMenu(string name, Texture2D icon, MenuGroup menuGroup, IMenuRepository targetMenuRepo = null)
        {
            var menu = targetMenuRepo ?? _rootMenu;
            menu.Add(new MenuItem()
            {
                name = name,
                icon = icon,
                type = MenuItem.ItemType.SubMenu,
                subMenu = menuGroup
            });
        }

        internal override void OnDisable() { }

        internal override void OnEnable() { }
    }
}
