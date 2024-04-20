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
using UnityEngine;
#if DK_VRCSDK3A
using Chocopoi.DressingFramework.Menu.VRChat;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace Chocopoi.DressingFramework.Menu
{
    internal abstract class MenuRepositoryStore : MenuStore
    {
        private readonly Context _ctx;
        private readonly Dictionary<MenuItem, string> _buffer;
#if DK_VRCSDK3A
        private readonly HashSet<VRCExpressionsMenu> _clonedVrcMenus;
#endif

        public MenuRepositoryStore(Context ctx)
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

        private void InstallMenuItem(IMenuRepository rootMenu, MenuItem item, string path)
        {
#if DK_VRCSDK3A
            var installTarget = VRCMenuUtils.FindInstallTarget(rootMenu, path, _ctx, _clonedVrcMenus);
#else
            var installTarget = MenuUtils.FindInstallTarget(rootMenu, path);
#endif
            installTarget.Add(item);
        }

        public abstract IMenuRepository GetRootMenu();

        public override void Flush()
        {
            var rootMenu = GetRootMenu();
            foreach (var kvp in _buffer)
            {
                InstallMenuItem(rootMenu, kvp.Key, kvp.Value);
            }
            _buffer.Clear();
        }
    }
}
