/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingTools.
 * 
 * DressingTools is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingTools is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingTools. If not, see <https://www.gnu.org/licenses/>.
 */

#if DK_MA && DK_VRCSDK3A
using System.Collections.Generic;
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Detail.DK
{
    /// <summary>
    /// This might sound a bit confusing, but DKMAMenuStore is more associated to DK instead of MA/NDMF.
    /// This store only appears during DK builds instead of NDMF builds. It will convert DK menus into
    /// proper form and pass them to MA, in order to have good compatibility with MA menus, especially with
    /// the Next Page button feature.
    /// </summary>
    internal class DKMAMenuStore : MenuStore
    {
        private readonly Context _ctx;
        private readonly Dictionary<string, List<MenuItem>> _buffer;
        private readonly HashSet<VRCExpressionsMenu> _clonedVrcMenus;

        public DKMAMenuStore(Context ctx)
        {
            _ctx = ctx;
            _buffer = new Dictionary<string, List<MenuItem>>();
            _clonedVrcMenus = new HashSet<VRCExpressionsMenu>();
        }

        public override void Append(MenuItem menuItem, string path = null)
        {
            if (path == null)
            {
                path = "";
            }
            path = path.Trim();

            if (!_buffer.TryGetValue(path, out var menuItems))
            {
                menuItems = _buffer[path] = new List<MenuItem>();
            }
            menuItems.Add(menuItem);
        }

        private static VRCExpressionsMenu.Control MakeSubMenuControl(string name, VRCExpressionsMenu subMenu)
        {
            return new VRCExpressionsMenu.Control()
            {
                name = name,
                icon = null,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" },
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = subMenu,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0]
            };
        }

        private VRCExpressionsMenu MakeDownwardsMenuGroups(string[] paths, int index)
        {
            var menu = Object.Instantiate(VRCMenuUtils.GetDefaultExpressionsMenu());
            _ctx.CreateUniqueAsset(menu, string.Join("_", paths, 0, index));
            if (index < paths.Length)
            {
                var newMenuItem = MakeSubMenuControl(paths[index], MakeDownwardsMenuGroups(paths, index + 1));
                menu.controls.Add(newMenuItem);
            }
            return menu;
        }

        private VRCExpressionsMenu FindInstallTarget(VRCExpressionsMenu parent, string[] paths, int index)
        {
            if (index >= paths.Length)
            {
                return parent;
            }

            foreach (var item in parent.controls)
            {
                if (item.name != paths[index])
                {
                    continue;
                }

                if (item.type == VRCExpressionsMenu.Control.ControlType.SubMenu)
                {
                    if (item.subMenu == null)
                    {
                        var newVrcMenu = Object.Instantiate(VRCMenuUtils.GetDefaultExpressionsMenu());
                        _ctx.CreateUniqueAsset(newVrcMenu, string.Join("_", paths, 0, index + 1));
                        item.subMenu = newVrcMenu;

                        _clonedVrcMenus.Add(item.subMenu);
                    }
                    else if (!_clonedVrcMenus.Contains(item.subMenu))
                    {
                        var menuCopy = Object.Instantiate(item.subMenu);
                        _ctx.CreateUniqueAsset(menuCopy, string.Join("_", paths, 0, index + 1));
                        item.subMenu = menuCopy;

                        _clonedVrcMenus.Add(item.subMenu);
                    }

                    return FindInstallTarget(item.subMenu, paths, index + 1);
                }
            }

            // if not found, we create empty menu groups recursively downwards
            var newMenuItem = MakeSubMenuControl(paths[index], MakeDownwardsMenuGroups(paths, index + 1));
            parent.controls.Add(newMenuItem);

            // find again
            return FindInstallTarget(parent, paths, index);
        }

        public override void Flush()
        {
            if (!_ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // nothing to do if it's not a VRC avatar
                return;
            }

            var dkMaRootObj = new GameObject("DKMA");
            dkMaRootObj.transform.SetParent(_ctx.AvatarGameObject.transform);

            foreach (var kvp in _buffer)
            {
                var path = kvp.Key;
                var items = kvp.Value;

                // find and create the install target and pass to MA
                string menuObjName;
                VRCExpressionsMenu installTarget;
                if (string.IsNullOrEmpty(path))
                {
                    menuObjName = "Root";
                    installTarget = avatarDesc.expressionsMenu;
                }
                else
                {
                    var paths = path.Trim().Split('/');
                    menuObjName = string.Join("_", paths);
                    installTarget = FindInstallTarget(avatarDesc.expressionsMenu, paths, 0);
                }

                var menuObj = new GameObject(menuObjName);
                menuObj.transform.SetParent(dkMaRootObj.transform);

                // add installer
                var maInstaller = menuObj.AddComponent<ModularAvatarMenuInstaller>();
                maInstaller.installTargetMenu = installTarget;

                // add menu group
                var maGroup = menuObj.AddComponent<ModularAvatarMenuGroup>();
                maGroup.targetObject = menuObj;

                // add menu items
                foreach (var item in items)
                {
                    var maItemObj = new GameObject(item.Name);
                    maItemObj.transform.SetParent(maGroup.transform);

                    var maItem = maItemObj.AddComponent<ModularAvatarMenuItem>();
                    // TODO
                }
            }
        }

        internal override void OnEnable() { }

        internal override void OnDisable() { }
    }
}
#endif
