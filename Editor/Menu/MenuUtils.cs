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

using System;

namespace Chocopoi.DressingFramework.Menu
{
    internal static class MenuUtils
    {
        private static MenuGroup MakeDownwardsMenuGroups(string[] paths, int index)
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

        internal static IMenuRepository GenericFindInstallTarget(IMenuRepository parent, string[] paths, int index, Func<MenuItem, int, IMenuRepository> extraFindFunc = null)
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
                    return GenericFindInstallTarget(subMenuItem.SubMenu, paths, index + 1, extraFindFunc);
                }
                else if (extraFindFunc != null)
                {
                    var repo = extraFindFunc(item, index);
                    if (repo != null)
                    {
                        return repo;
                    }
                }
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
            return GenericFindInstallTarget(parent, paths, index, extraFindFunc);
        }

        public static IMenuRepository FindInstallTarget(IMenuRepository rootMenu, string path)
        {
            var installTarget = rootMenu;
            if (!string.IsNullOrEmpty(path))
            {
                var paths = path.Trim().Split('/');
                installTarget = GenericFindInstallTarget(rootMenu, paths, 0);
            }
            return installTarget;
        }
    }
}
