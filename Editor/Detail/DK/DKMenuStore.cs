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
using Chocopoi.DressingFramework.Menu;
using UnityEngine;
#if DK_VRCSDK3A
using Chocopoi.DressingFramework.Menu.VRChat;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace Chocopoi.DressingFramework.Detail.DK
{
    internal class DKMenuStore : MenuStore
    {
        private Context _ctx;
        private readonly Dictionary<MenuItem, string> _buffer;
#if DK_VRCSDK3A
        private readonly HashSet<VRCExpressionsMenu> _clonedVrcMenus;
#endif

        public DKMenuStore(Context ctx)
        {
            _ctx = ctx;
            _buffer = new Dictionary<MenuItem, string>();
#if DK_VRCSDK3A
            _clonedVrcMenus = new HashSet<VRCExpressionsMenu>();
#endif
        }

        public override void Append(MenuItem menuItem, string path = null)
        {
            if (path == null)
            {
                path = "";
            }
            path = path.Trim();

            _buffer[menuItem] = path;
        }

        private IMenuRepository FindInstallTarget(IMenuRepository parent, string[] paths, int index)
        {
            if (index >= paths.Length)
            {
                return parent;
            }

            foreach (var item in parent)
            {
                if (item.Name != paths[index])
                {
                    continue;
                }

                if (item is SubMenuItem subMenuItem)
                {
                    if (subMenuItem.SubMenu == null)
                    {
                        subMenuItem.SubMenu = new MenuGroup();
                    }
                    return FindInstallTarget(subMenuItem.SubMenu, paths, index + 1);
                }
#if DK_VRCSDK3A
                else if (item is VRCSubMenuItem vrcSubMenuItem)
                {
                    if (vrcSubMenuItem.SubMenu == null)
                    {
                        var newVrcMenu = Object.Instantiate(VRCMenuUtils.GetDefaultExpressionsMenu());
                        _ctx.CreateUniqueAsset(newVrcMenu, string.Join("_", paths, 0, index + 1));
                        vrcSubMenuItem.SubMenu = newVrcMenu;

                        _clonedVrcMenus.Add(vrcSubMenuItem.SubMenu);
                    }
                    else if (!_clonedVrcMenus.Contains(vrcSubMenuItem.SubMenu))
                    {
                        var menuCopy = Object.Instantiate(vrcSubMenuItem.SubMenu);
                        _ctx.CreateUniqueAsset(menuCopy, string.Join("_", paths, 0, index + 1));
                        vrcSubMenuItem.SubMenu = menuCopy;
                        _clonedVrcMenus.Add(vrcSubMenuItem.SubMenu);
                    }

                    return FindInstallTarget(new VRCMenuWrapper(vrcSubMenuItem.SubMenu, _ctx), paths, index + 1);
                }
#endif
            }

            // if not found, we create empty menu groups recursively downwards
            var newMenuItem = new SubMenuItem()
            {
                Name = paths[index],
                Icon = null,
                SubMenu = MakeDownwardsMenuGroups(paths, index + 1)
            };
            parent.Add(newMenuItem);

            // find again, the menu group pointers above cannot be used after the CRUD operation
            return FindInstallTarget(parent, paths, index);
        }

        private MenuGroup MakeDownwardsMenuGroups(string[] paths, int index)
        {
            var mg = new MenuGroup();
            if (index < paths.Length)
            {
                var newMenuItem = new SubMenuItem()
                {
                    Name = paths[index],
                    Icon = null,
                    SubMenu = MakeDownwardsMenuGroups(paths, index + 1)
                };
                mg.Add(newMenuItem);
            }
            return mg;
        }

        private void InstallMenuItem(IMenuRepository rootMenu, MenuItem item, string path)
        {
            var installTarget = rootMenu;
            if (!string.IsNullOrEmpty(path))
            {
                var paths = path.Trim().Split('/');
                installTarget = FindInstallTarget(rootMenu, paths, 0);
            }
            installTarget.Add(item);
        }

        private IMenuRepository GetRootMenu()
        {
            IMenuRepository rootMenu;
#if DK_VRCSDK3A
            {
                if (_ctx.AvatarGameObject.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out var avatarDesc))
                {
                    rootMenu = new VRCMenuSafeWrapper(avatarDesc.expressionsMenu, _ctx);
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
                _ctx.Report.LogInfo("DKMenuStore", "No compatible menu platform detected. An empty DK menu group is created as root menu.");
            }
#endif
            return rootMenu;
        }

        public override void Flush()
        {
            var rootMenu = GetRootMenu();
            foreach (var kvp in _buffer)
            {
                InstallMenuItem(rootMenu, kvp.Key, kvp.Value);
            }
            _buffer.Clear();
        }

        internal override void OnDisable() { }

        internal override void OnEnable() { }
    }
}
