/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

#if DK_MA && DK_VRCSDK3A
using System;
using System.Collections.Generic;
using System.Linq;
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using nadena.dev.modular_avatar.core;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

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
        private readonly MenuGroup _buffer;
        private readonly Dictionary<VRCExpressionsMenu, Tuple<string, MenuGroup>> _vrcMenuAppends;

        public DKMAMenuStore(Context ctx)
        {
            _ctx = ctx;
            _buffer = new MenuGroup();
            _vrcMenuAppends = new Dictionary<VRCExpressionsMenu, Tuple<string, MenuGroup>>();
        }

        public IMenuRepository FindMAInstallTarget(IMenuRepository rootMenu, string path)
        {
            var installTarget = rootMenu;
            if (!string.IsNullOrEmpty(path))
            {
                var paths = path.Trim().Split('/');
                installTarget = MenuUtils.GenericFindInstallTarget(rootMenu, paths, 0, (item, idx) =>
                {
                    if (item is VRCSubMenuItem vrcSubMenuItem)
                    {
                        var newPath = $"{path}/{item.Name}";
                        if (vrcSubMenuItem.SubMenu == null)
                        {
                            var newVrcMenu = Object.Instantiate(VRCMenuUtils.GetDefaultExpressionsMenu());
                            _ctx.CreateUniqueAsset(newVrcMenu, newPath.Replace('/', '_'));
                            vrcSubMenuItem.SubMenu = newVrcMenu;
                        }
                        if (!_vrcMenuAppends.TryGetValue(vrcSubMenuItem.SubMenu, out var menuGroup))
                        {
                            menuGroup = _vrcMenuAppends[vrcSubMenuItem.SubMenu] = new Tuple<string, MenuGroup>(newPath, new MenuGroup());
                        }
                        return menuGroup.Item2;
                    }
                    return null;
                });
            }
            return installTarget;
        }

        public override void Append(MenuItem menuItem, string path = null)
        {
            // append the item to our buffer
            // in cases the path falls on a VRC menu, we install using MA installer instead
            var target = FindMAInstallTarget(_buffer, path);
            target.Add(menuItem);
        }

        private static VRCExpressionsMenu.Control MakeSubMenuControl(string name, Texture2D icon, VRCExpressionsMenu subMenu)
        {
            return new VRCExpressionsMenu.Control()
            {
                name = name,
                icon = icon,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" },
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = subMenu,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0]
            };
        }

        private VRCExpressionsMenu FindExistingVRCMenu(VRCExpressionsMenu parent, string[] paths, int index)
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
                        _ctx.CreateUniqueAsset(newVrcMenu, $"{string.Join("_", paths, 0, index + 1)}_{item.name}");
                        item.subMenu = newVrcMenu;
                    }
                    return FindExistingVRCMenu(item.subMenu, paths, index + 1);
                }
            }

            // not found
            return null;
        }

        private void DKToMAMenuItem(Transform dkMaRoot, VRCExpressionsMenu avatarRootMenu, Transform parent, MenuItem menuItem, string absolutePathPrefix)
        {
            var newPath = string.IsNullOrEmpty(absolutePathPrefix) ?
                menuItem.Name :
                $"{absolutePathPrefix}/{menuItem.Name}";

            // prefer using MA installer on existing menus for supporting path install
            if (menuItem is SubMenuItem preCheckSubMenuItem)
            {
                var newPaths = newPath.Split('/');
                var vrcMenu = FindExistingVRCMenu(avatarRootMenu, newPaths, 0);
                if (vrcMenu != null)
                {
                    // make a ma installer to that menu
                    var menuObj = new GameObject(string.Join("_", newPaths));
                    menuObj.transform.SetParent(dkMaRoot);
                    var maInstaller = menuObj.AddComponent<ModularAvatarMenuInstaller>();
                    maInstaller.installTargetMenu = vrcMenu;
                    var maGroup = menuObj.AddComponent<ModularAvatarMenuGroup>();
                    maGroup.targetObject = menuObj;
                    DKGroupToMAItems(dkMaRoot, avatarRootMenu, menuObj.transform, preCheckSubMenuItem.SubMenu, newPath);
                    return;
                }
            }

            var maItemObj = new GameObject(menuItem.Name);
            maItemObj.transform.SetParent(parent);

            var maItem = maItemObj.AddComponent<ModularAvatarMenuItem>();

            if (menuItem is SubMenuItem subMenuItem)
            {
                maItem.Control = MakeSubMenuControl(menuItem.Name, menuItem.Icon, null);
                maItem.MenuSource = SubmenuSource.Children;
                if (subMenuItem.SubMenu != null)
                {
                    DKGroupToMAItems(dkMaRoot, avatarRootMenu, maItemObj.transform, subMenuItem.SubMenu, newPath);
                }
            }
            else if (menuItem is VRCSubMenuItem vrcSubMenuItem)
            {
                maItem.Control = MakeSubMenuControl(menuItem.Name, menuItem.Icon, vrcSubMenuItem.SubMenu);
                maItem.MenuSource = SubmenuSource.MenuAsset;
            }
            else
            {
                maItem.Control = VRCMenuUtils.MenuItemToControl(menuItem);
            }
        }

        private void DKGroupToMAItems(Transform dkMaRoot, VRCExpressionsMenu avatarRootMenu, Transform parent, MenuGroup menuGroup, string absolutePath)
        {
            foreach (var item in menuGroup)
            {
                DKToMAMenuItem(dkMaRoot, avatarRootMenu, parent, item, absolutePath);
            }
        }

        public override void Flush()
        {
            if (!_ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // nothing to do if it's not a VRC avatar
                return;
            }

            var dkMaRootObj = new GameObject("DKMAMenu");
            dkMaRootObj.transform.SetParent(_ctx.AvatarGameObject.transform);

            var rootMenuObj = new GameObject("Root");
            rootMenuObj.transform.SetParent(dkMaRootObj.transform);
            var rootMaInstaller = rootMenuObj.AddComponent<ModularAvatarMenuInstaller>();
            rootMaInstaller.installTargetMenu = avatarDesc.expressionsMenu;
            var rootMaGroup = rootMenuObj.AddComponent<ModularAvatarMenuGroup>();
            rootMaGroup.targetObject = rootMenuObj;

            // for paths pointing to existing vrc menus, we use the ma installer
            foreach (var kvp in _vrcMenuAppends)
            {
                var menuObj = new GameObject(kvp.Key.name);
                menuObj.transform.SetParent(dkMaRootObj.transform);
                var maInstaller = menuObj.AddComponent<ModularAvatarMenuInstaller>();
                maInstaller.installTargetMenu = kvp.Key;
                var maGroup = menuObj.AddComponent<ModularAvatarMenuGroup>();
                maGroup.targetObject = menuObj;
                DKGroupToMAItems(dkMaRootObj.transform, avatarDesc.expressionsMenu, menuObj.transform, kvp.Value.Item2, kvp.Value.Item1);
            }

            // normal appends
            DKGroupToMAItems(dkMaRootObj.transform, avatarDesc.expressionsMenu, rootMenuObj.transform, _buffer, "");
        }

        internal override void OnEnable() { }

        internal override void OnDisable() { }
    }
}
#endif
