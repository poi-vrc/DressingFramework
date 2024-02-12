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
using System.Collections;
using System.Collections.Generic;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    /// <summary>
    /// VRC menu wrapper on the IMenuRepository CRUD interface
    /// </summary>
    public class VRCMenuWrapper : IMenuRepository
    {
        public MenuItem this[int index] { get => VRCMenuUtils.ControlToMenuItem(_vrcMenu.controls[index]); set => _vrcMenu.controls[index] = VRCMenuUtils.MenuItemToControl(value, _ctx); }

        private readonly VRCExpressionsMenu _vrcMenu;
        private readonly Context _ctx;

        public VRCMenuWrapper(VRCExpressionsMenu vrcMenu, Context ctx = null)
        {
            _vrcMenu = vrcMenu;
            _ctx = ctx;
        }

        public VRCExpressionsMenu GetContainingMenu() => _vrcMenu;

        public void Add(MenuItem menuItem) => _vrcMenu.controls.Add(VRCMenuUtils.MenuItemToControl(menuItem, _ctx));
        public void Clear() => _vrcMenu.controls.Clear();
        public int Count() => _vrcMenu.controls.Count;
        public void Insert(int index, MenuItem menuItem) => _vrcMenu.controls.Insert(index, VRCMenuUtils.MenuItemToControl(menuItem));
        public void Remove(int index) => _vrcMenu.controls.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<MenuItem> GetEnumerator()
        {
            foreach (var control in _vrcMenu.controls)
            {
                yield return VRCMenuUtils.ControlToMenuItem(control);
            }
        }
    }
}
#endif
