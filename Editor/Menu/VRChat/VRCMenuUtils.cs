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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Object = UnityEngine.Object;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal static class VRCMenuUtils
    {
        private static IMenuRepository ProcessVRCMenuItem(MenuItem item, int index, Context ctx, string[] paths, HashSet<VRCExpressionsMenu> clonedVrcMenus)
        {
            if (item is VRCSubMenuItem vrcSubMenuItem)
            {
                if (vrcSubMenuItem.SubMenu == null)
                {
                    var newVrcMenu = Object.Instantiate(GetDefaultExpressionsMenu());
                    ctx.CreateUniqueAsset(newVrcMenu, string.Join("_", paths, 0, index + 1));
                    vrcSubMenuItem.SubMenu = newVrcMenu;

                    clonedVrcMenus.Add(vrcSubMenuItem.SubMenu);
                }
                else if (!clonedVrcMenus.Contains(vrcSubMenuItem.SubMenu))
                {
                    var menuCopy = Object.Instantiate(vrcSubMenuItem.SubMenu);
                    ctx.CreateUniqueAsset(menuCopy, string.Join("_", paths, 0, index + 1));
                    vrcSubMenuItem.SubMenu = menuCopy;
                    clonedVrcMenus.Add(vrcSubMenuItem.SubMenu);
                }

                return MenuUtils.GenericFindInstallTarget(new VRCMenuWrapper(vrcSubMenuItem.SubMenu, ctx), paths, index + 1, (anotherItem, anotherIndex) =>
                    ProcessVRCMenuItem(anotherItem, anotherIndex, ctx, paths, clonedVrcMenus));
            }
            return null;

        }

        public static IMenuRepository FindInstallTarget(IMenuRepository rootMenu, string path, Context ctx, HashSet<VRCExpressionsMenu> clonedVrcMenus)
        {
            var installTarget = rootMenu;
            if (!string.IsNullOrEmpty(path))
            {
                var paths = path.Trim().Split('/');
                installTarget = MenuUtils.GenericFindInstallTarget(rootMenu, paths, 0, (anotherItem, anotherIndex) =>
                    ProcessVRCMenuItem(anotherItem, anotherIndex, ctx, paths, clonedVrcMenus));
            }
            return installTarget;
        }

        public static VRCExpressionParameters GetDefaultExpressionsParameters()
        {
            var expressionParameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>("Packages/com.vrchat.avatars/Samples/AV3 Demo Assets/Expressions Menu/DefaultExpressionParameters.asset");
            if (expressionParameters == null)
            {
                expressionParameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>("Assets/VRCSDK/Examples3/Animation/Controllers/Expressions Menu/DefaultExpressionParameters.asset");
            }
            return expressionParameters;
        }

        public static VRCExpressionsMenu GetDefaultExpressionsMenu()
        {
            var expressionsMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>("Packages/com.vrchat.avatars/Samples/AV3 Demo Assets/Expressions Menu/DefaultExpressionsMenu.asset");
            if (expressionsMenu == null)
            {
                expressionsMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>("Assets/VRCSDK/Examples3/Animation/Controllers/Expressions Menu/DefaultExpressionsMenu.asset");
            }
            return expressionsMenu;
        }

        public static VRCExpressionParameters GetExpressionsParameters(VRCAvatarDescriptor avatarDescriptor)
        {
            var expressionParameters = avatarDescriptor.expressionParameters;

            if (expressionParameters == null)
            {
                expressionParameters = GetDefaultExpressionsParameters();
                if (expressionParameters == null)
                {
                    // we can't obtain the default asset
                    return null;
                }
            }

            return expressionParameters;
        }

        public static VRCExpressionsMenu GetExpressionsMenu(VRCAvatarDescriptor avatarDescriptor)
        {
            var expressionsMenu = avatarDescriptor.expressionsMenu;

            if (expressionsMenu == null)
            {
                expressionsMenu = GetDefaultExpressionsMenu();
                if (expressionsMenu == null)
                {
                    // we can't obtain the default asset
                    return null;
                }
            }

            return expressionsMenu;
        }

        private static void ItemControllerToAnimatorParameter(MenuItemController ctrl, out VRCExpressionsMenu.Control.Parameter parameter, out float value)
        {
            if (ctrl == null)
            {
                // no controller
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" };
                value = 0.0f;
                return;
            }

            if (!(ctrl is AnimatorParameterController animParamCtrl))
            {
                throw new NotImplementedException("Non animator parameter item controllers are not implemented yet.");
            }

            parameter = new VRCExpressionsMenu.Control.Parameter()
            {
                name = animParamCtrl.ParameterName
            };
            value = animParamCtrl.ParameterValue;
        }

        private static VRCExpressionsMenu.Control.Parameter[] ItemControllersToParameters(params MenuItemController[] controllers)
        {
            if (controllers == null)
            {
                return null;
            }
            var output = new VRCExpressionsMenu.Control.Parameter[controllers.Length];
            for (var i = 0; i < controllers.Length; i++)
            {
                ItemControllerToAnimatorParameter(controllers[i], out var param, out var _);
                output[i] = param;
            }
            return output;
        }

        private static VRCExpressionsMenu.Control.Label[] DKLabelsToVRCLabels(params MenuItem.Label[] labels)
        {
            var output = new VRCExpressionsMenu.Control.Label[labels.Length];
            for (var i = 0; i < labels.Length; i++)
            {
                output[i] = new VRCExpressionsMenu.Control.Label()
                {
                    name = labels[i]?.Name ?? "",
                    icon = labels[i]?.Icon
                };
            }
            return output;
        }

        public static MenuItem ControlToMenuItem(VRCExpressionsMenu.Control ctrl)
        {
            MenuItem item;
            switch (ctrl.type)
            {
                case VRCExpressionsMenu.Control.ControlType.Button:
                    item = new VRCButtonItemWrapper(ctrl);
                    break;
                case VRCExpressionsMenu.Control.ControlType.Toggle:
                    item = new VRCToggleItemWrapper(ctrl);
                    break;
                case VRCExpressionsMenu.Control.ControlType.SubMenu:
                    item = new VRCSubMenuItemWrapper(ctrl);
                    break;
                case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
                    item = new VRCTwoAxisItemWrapper(ctrl);
                    break;
                case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
                    item = new VRCFourAxisItemWrapper(ctrl);
                    break;
                case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
                    item = new VRCRadialItemWrapper(ctrl);
                    break;
                default:
                    return null;
            }
            return item;
        }

        public static VRCExpressionsMenu.Control MenuItemToControl(MenuItem item, Context ctx = null)
        {
            var ctrl = new VRCExpressionsMenu.Control()
            {
                name = item.Name,
                icon = item.Icon,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" },
                value = 1.0f,
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = null,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0]
            };

            if (item is SingleControllerItem singleCtrlItem)
            {
                ItemControllerToAnimatorParameter(singleCtrlItem.Controller, out var param, out var value);
                ctrl.parameter = param;
                ctrl.value = value;

                if (item is ButtonItem btnItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.Button;
                }
                else if (item is ToggleItem toggleItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.Toggle;
                }
            }
            else if (item is ControllerOnOpenItem ctrlOnOpenItem)
            {
                ItemControllerToAnimatorParameter(ctrlOnOpenItem.ControllerOnOpen, out var param, out var value);
                ctrl.parameter = param;
                ctrl.value = value;

                if (item is SubMenuItem subMenuItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.SubMenu;

                    if (subMenuItem.SubMenu != null)
                    {
                        var subMenu = MenuGroupToVRCMenu(subMenuItem.SubMenu, ctx);
                        ctx?.CreateUniqueAsset(subMenu, $"{item.Name}_SubMenu");
                        ctrl.subMenu = subMenu;
                    }
                }
                else if (item is VRCSubMenuItem vrcSubMenuItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.SubMenu;
                    ctrl.subMenu = vrcSubMenuItem.SubMenu;
                }
                else if (item is TwoAxisItem twoAxisItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet;
                    ctrl.subParameters = ItemControllersToParameters(twoAxisItem.HorizontalController, twoAxisItem.VerticalController);
                    ctrl.labels = DKLabelsToVRCLabels(twoAxisItem.UpLabel, twoAxisItem.RightLabel, twoAxisItem.DownLabel, twoAxisItem.LeftLabel);
                }
                else if (item is FourAxisItem fourAxisItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.FourAxisPuppet;
                    ctrl.subParameters = ItemControllersToParameters(fourAxisItem.UpController, fourAxisItem.RightController, fourAxisItem.DownController, fourAxisItem.LeftController);
                    ctrl.labels = DKLabelsToVRCLabels(fourAxisItem.UpLabel, fourAxisItem.RightLabel, fourAxisItem.DownLabel, fourAxisItem.LeftLabel);
                }
                else if (item is RadialItem radialItem)
                {
                    ctrl.type = VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                    ctrl.subParameters = ItemControllersToParameters(radialItem.RadialController);
                }
            }

            return ctrl;
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
            var vrcMenu = UnityEngine.Object.Instantiate(GetDefaultExpressionsMenu());
            var wrapper = new VRCMenuSafeWrapper(vrcMenu, ctx);
            foreach (var menuItem in menuGroup.Items)
            {
                wrapper.Add(menuItem);
            }
            return vrcMenu;
        }

        public static void WriteMenuItemController(MenuItemController from, VRCAnimatorParameterControllerWrapper to)
        {
            if (from is AnimatorParameterController val)
            {
                to.ParameterName = val.ParameterName;
                to.ParameterValue = val.ParameterValue;
            }
        }

        public static void WriteLabel(MenuItem.Label from, VRCMenuItemLabelWrapper to)
        {
            to.Name = from.Name;
            to.Icon = from.Icon;
        }

        public static int CalculateParametersCost(IEnumerable<VRCExpressionParameters.Parameter> parameters)
        {
            var cost = 0;

            foreach (var p in parameters)
            {
                if (p.networkSynced)
                {
                    cost += VRCExpressionParameters.TypeCost(p.valueType);
                }
            }

            return cost;
        }
    }
}
#endif
