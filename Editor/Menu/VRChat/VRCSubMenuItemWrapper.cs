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

#if DK_VRCSDK3A
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal class VRCSubMenuItemWrapper : VRCSubMenuItem
    {
        public override string Name { get => _control.name; set => _control.name = value; }
        public override Texture2D Icon { get => _control.icon; set => _control.icon = value; }
        public override MenuItemController ControllerOnOpen { get => _controllerOnOpenWrapper; set => VRCMenuUtils.WriteMenuItemController(value, _controllerOnOpenWrapper); }
        public override VRCExpressionsMenu SubMenu { get => _control.subMenu; set => _control.subMenu = value; }

        private readonly VRCExpressionsMenu.Control _control;
        private readonly VRCAnimatorParameterControllerWrapper _controllerOnOpenWrapper;

        public VRCSubMenuItemWrapper(VRCExpressionsMenu.Control control)
        {
            _control = control;
            _controllerOnOpenWrapper = new VRCAnimatorParameterControllerWrapper(control.parameter, () => control.value, val => control.value = val);
        }
    }
}
#endif
