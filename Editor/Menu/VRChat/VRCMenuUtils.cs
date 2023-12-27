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

#if VRC_SDK_VRCSDK3
using System;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal class VRCMenuUtils
    {
        private static MenuItem.ItemController[] ParametersToItemControllers(float value, VRCExpressionsMenu.Control.Parameter[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            var output = new MenuItem.ItemController[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                output[i] = new MenuItem.ItemController()
                {
                    type = MenuItem.ItemController.ControllerType.AnimatorParameter,
                    animatorParameterName = parameters[i].name,
                    animatorParameterValue = value
                };
            }
            return output;
        }

        private static VRCExpressionsMenu.Control.Parameter[] ItemControllersToParameters(MenuItem.ItemController[] controllers)
        {
            if (controllers == null)
            {
                return null;
            }
            var output = new VRCExpressionsMenu.Control.Parameter[controllers.Length];
            for (var i = 0; i < controllers.Length; i++)
            {
                output[i] = new VRCExpressionsMenu.Control.Parameter()
                {
                    name = controllers[i].animatorParameterName
                };
            }
            return output;
        }

        private static MenuItem.Label[] VRCLabelsToDKLabels(VRCExpressionsMenu.Control.Label[] labels)
        {
            if (labels == null)
            {
                return null;
            }
            var output = new MenuItem.Label[labels.Length];
            for (var i = 0; i < labels.Length; i++)
            {
                output[i] = new MenuItem.Label()
                {
                    name = labels[i].name,
                    icon = labels[i].icon
                };
            }
            return output;
        }

        private static VRCExpressionsMenu.Control.Label[] DKLabelsToVRCLabels(MenuItem.Label[] labels)
        {
            if (labels == null)
            {
                return null;
            }
            var output = new VRCExpressionsMenu.Control.Label[labels.Length];
            for (var i = 0; i < labels.Length; i++)
            {
                output[i] = new VRCExpressionsMenu.Control.Label()
                {
                    name = labels[i].name,
                    icon = labels[i].icon
                };
            }
            return output;
        }

        public static MenuItem ControlToMenuItem(VRCExpressionsMenu.Control ctrl)
        {
            MenuItem.ItemType type;
            switch (ctrl.type)
            {
                case VRCExpressionsMenu.Control.ControlType.Button:
                    type = MenuItem.ItemType.Button;
                    break;
                case VRCExpressionsMenu.Control.ControlType.Toggle:
                    type = MenuItem.ItemType.Toggle;
                    break;
                case VRCExpressionsMenu.Control.ControlType.SubMenu:
                    type = MenuItem.ItemType.SubMenu;
                    break;
                case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
                    type = MenuItem.ItemType.TwoAxis;
                    break;
                case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
                    type = MenuItem.ItemType.FourAxis;
                    break;
                case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
                    type = MenuItem.ItemType.Radial;
                    break;
                default:
                    return null;
            }

            return new VRCMenuItem()
            {
                name = ctrl.name,
                icon = ctrl.icon,
                type = type,
                controller = new MenuItem.ItemController()
                {
                    type = MenuItem.ItemController.ControllerType.AnimatorParameter,
                    animatorParameterName = ctrl.parameter.name,
                    animatorParameterValue = ctrl.value
                },
                vrcSubMenu = ctrl.subMenu,
                subControllers = ParametersToItemControllers(ctrl.value, ctrl.subParameters),
                subLabels = VRCLabelsToDKLabels(ctrl.labels)
            };
        }

        public static VRCExpressionsMenu.Control MenuItemToControl(MenuItem menuItem, Context ctx = null)
        {
            VRCExpressionsMenu.Control.ControlType type;
            switch (menuItem.type)
            {
                case MenuItem.ItemType.Button:
                    type = VRCExpressionsMenu.Control.ControlType.Button;
                    break;
                case MenuItem.ItemType.Toggle:
                    type = VRCExpressionsMenu.Control.ControlType.Toggle;
                    break;
                case MenuItem.ItemType.SubMenu:
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                    break;
                case MenuItem.ItemType.TwoAxis:
                    type = VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet;
                    break;
                case MenuItem.ItemType.FourAxis:
                    type = VRCExpressionsMenu.Control.ControlType.FourAxisPuppet;
                    break;
                case MenuItem.ItemType.Radial:
                    type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                    break;
                default:
                    return null;
            }

            if (menuItem.controller.type != MenuItem.ItemController.ControllerType.AnimatorParameter)
            {
                throw new NotImplementedException("Non animator parameter item controllers are not implemented yet.");
            }

            VRCExpressionsMenu subMenu = null;

            if (menuItem.type == MenuItem.ItemType.SubMenu)
            {
                if (menuItem is VRCMenuItem item && item.vrcSubMenu != null)
                {
                    subMenu = item.vrcSubMenu;
                }
                else if (menuItem.subMenu != null)
                {
                    subMenu = MenuGroupToVRCMenu(menuItem.subMenu);
                    ctx?.CreateUniqueAsset(subMenu, $"{menuItem.name}_SubMenu_{DKEditorUtils.RandomString(6)}");
                }
            }

            return new VRCExpressionsMenu.Control()
            {
                name = menuItem.name,
                icon = menuItem.icon,
                type = type,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = menuItem.controller.animatorParameterName },
                value = menuItem.controller.animatorParameterValue,
                style = VRCExpressionsMenu.Control.Style.Style1, // what is this for?
                subMenu = subMenu,
                subParameters = ItemControllersToParameters(menuItem.subControllers),
                labels = DKLabelsToVRCLabels(menuItem.subLabels)
            };
        }

        public static MenuGroup VRCMenuToMenuGroup(VRCExpressionsMenu vrcMenu)
        {
            var menuGroup = new MenuGroup();
            foreach (var control in vrcMenu.controls)
            {
                menuGroup.Add(ControlToMenuItem(control));
            }
            return menuGroup;
        }

        public static VRCExpressionsMenu MenuGroupToVRCMenu(MenuGroup menuGroup, Context ctx = null)
        {
            var vrcMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            foreach (var menuItem in menuGroup.Items)
            {
                vrcMenu.controls.Add(MenuItemToControl(menuItem, ctx));
            }
            return vrcMenu;
        }
    }
}
#endif
