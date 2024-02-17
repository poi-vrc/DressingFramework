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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    /// <summary>
    /// Overflow-safe VRC menu wrapper. It has a limitation that the eight-th menu item
    /// must be the next page item.
    /// </summary>
    internal class VRCMenuSafeWrapper : IMenuRepository
    {
        // we want to hide the Next Page button to abstraction layer, so we subtract one
        public const int MaxUsableControls = VRCExpressionsMenu.MAX_CONTROLS - 1;

        public MenuItem this[int index]
        {
            get
            {
                GetMenuAndItemIndex(index, out var menuIndex, out var itemIndex);
                var controls = _vrcMenus[menuIndex].controls;
                return VRCMenuUtils.ControlToMenuItem(controls[itemIndex]);
            }
            set
            {
                GetMenuAndItemIndex(index, out var menuIndex, out var itemIndex);
                var controls = _vrcMenus[menuIndex].controls;
                controls[itemIndex] = VRCMenuUtils.MenuItemToControl(value, _ctx);
            }
        }

        private readonly List<VRCExpressionsMenu> _vrcMenus;
        private readonly Context _ctx;
        private bool _topMenuExtended;

        public VRCMenuSafeWrapper(VRCExpressionsMenu vrcMenu, Context ctx = null) : this(new List<VRCExpressionsMenu>() { vrcMenu }, ctx)
        {
            if (vrcMenu.controls.Count > VRCExpressionsMenu.MAX_CONTROLS)
            {
                throw new ArgumentException($"The provided VRC menu is invalid with too many controls: {vrcMenu.controls.Count} > {VRCExpressionsMenu.MAX_CONTROLS}");
            }

            // if count is less or equal to max usable controls, we don't have to do anything
            // otherwise, we extend and move the last menu item on the first write op
            _topMenuExtended = vrcMenu.controls.Count <= MaxUsableControls;
        }

        private VRCMenuSafeWrapper(List<VRCExpressionsMenu> vrcMenus, Context ctx)
        {
            if (vrcMenus.Count > 1)
            {
                // if more than one specified, we assume it's already extended
                _topMenuExtended = true;
            }
            _vrcMenus = vrcMenus;
            _ctx = ctx;
        }

        public List<VRCExpressionsMenu> GetContainingMenus() => _vrcMenus;

        private void ExtendTopMenu()
        {
            var menu = _vrcMenus[0];
            var newMenu = ExtendNextPage(menu, 0);

            var lastMenuItem = menu.controls[VRCExpressionsMenu.MAX_CONTROLS - 1];

            menu.controls.Remove(lastMenuItem);
            newMenu.controls.Add(lastMenuItem);

            _topMenuExtended = true;
        }

        public void Add(MenuItem menuItem)
        {
            if (!_topMenuExtended)
            {
                ExtendTopMenu();
            }

            for (var i = 0; i < _vrcMenus.Count; i++)
            {
                var menu = _vrcMenus[i];
                if (SafeAddControl(menu, i, VRCMenuUtils.MenuItemToControl(menuItem, _ctx)))
                {
                    return;
                }
            }
        }

        private static void CreateNextPageItem(List<VRCExpressionsMenu.Control> controls, VRCExpressionsMenu subMenu)
        {
            controls.Add(new VRCExpressionsMenu.Control()
            {
                name = "Next Page", // TODO: localization
                icon = null, // TODO: customizable
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" },
                value = 1f,
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = subMenu,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0]
            });
        }

        private VRCExpressionsMenu ExtendNextPage(VRCExpressionsMenu menu, int i)
        {
            var newMenu = Object.Instantiate(VRCMenuUtils.GetDefaultExpressionsMenu());
            _ctx?.CreateUniqueAsset(newMenu, $"{menu.name}_{i + 2}");
            CreateNextPageItem(menu.controls, newMenu);
            _vrcMenus.Add(newMenu);
            return newMenu;
        }

        private bool SafeAddControl(VRCExpressionsMenu menu, int i, VRCExpressionsMenu.Control control)
        {
            // requires the menu to not already contain a next page button
            if (menu.controls.Count >= VRCExpressionsMenu.MAX_CONTROLS)
            {
                return false;
            }

            if (menu.controls.Count == MaxUsableControls)
            {
                // create next page
                menu = ExtendNextPage(menu, i);
            }

            menu.controls.Add(control);
            return true;
        }

        public void Clear()
        {
            var tmp = _vrcMenus[0];
            tmp.controls.Clear();
            for (var i = 1; i < _vrcMenus.Count; i++)
            {
                _vrcMenus[i].name = "_DeletedMenuPlzIgnore";
                // TODO: delete asset
            }
            _vrcMenus.Clear();
            _vrcMenus.Add(tmp);
        }

        public int Count() => (_vrcMenus.Count - 1) * MaxUsableControls + _vrcMenus[_vrcMenus.Count - 1].controls.Count;

        // private void PrintItems()
        // {
        //     var str = "";
        //     foreach (var menu in _vrcMenus)
        //     {
        //         foreach (var ctrl in menu.controls)
        //         {
        //             str += $"{ctrl.name} ";
        //         }
        //         str += $" ({menu.controls.Count}) |";
        //     }
        //     Debug.Log(str);
        // }

        public void Insert(int index, MenuItem menuItem)
        {
            // if (!_topMenuExtended)
            // {
            //     ExtendTopMenu();
            // }

            // GetMenuAndItemIndex(index, out var menuIndex, out var itemIndex);
            // var targetMenu = _vrcMenus[menuIndex];

            // TODO: implement insert
            throw new NotImplementedException();
        }

        public void Remove(int index)
        {
            // if (!_topMenuExtended)
            // {
            //     ExtendTopMenu();
            // }

            // GetMenuAndItemIndex(index, out var menuIndex, out var itemIndex);
            // var targetMenu = _vrcMenus[menuIndex];

            // TODO: implement remove
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<MenuItem> GetEnumerator()
        {
            foreach (var menu in _vrcMenus)
            {
                var len = Math.Min(menu.controls.Count, MaxUsableControls);
                for (var i = 0; i < len; i++)
                {
                    yield return VRCMenuUtils.ControlToMenuItem(menu.controls[i]);
                }
            }
        }

        private void GetMenuAndItemIndex(int index, out int menuIndex, out int itemIndex)
        {
            if (index < 0 || index >= _vrcMenus.Count * MaxUsableControls)
            {
                throw new IndexOutOfRangeException();
            }
            menuIndex = index / MaxUsableControls;
            itemIndex = index % MaxUsableControls;
        }

        public void SetAllDirty()
        {
            foreach (var menu in _vrcMenus)
            {
                EditorUtility.SetDirty(menu);
            }
        }
    }
}
#endif
