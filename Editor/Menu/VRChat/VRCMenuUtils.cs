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
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal static class VRCMenuUtils
    {
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

        private static MenuItemController[] ParametersToItemControllers(float value, VRCExpressionsMenu.Control.Parameter[] parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            var output = new MenuItemController[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                output[i] = new AnimatorParameterController()
                {
                    ParameterName = parameters[i].name,
                    ParameterValue = value
                };
            }
            return output;
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
                    Name = labels[i].name,
                    Icon = labels[i].icon
                };
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

        private static T TryGetOrDefault<T>(T[] array, int index)
        {
            return array.Length > index ? array[index] : default;
        }

        public static MenuItem ControlToMenuItem(VRCExpressionsMenu.Control ctrl)
        {
            var controller = new AnimatorParameterController()
            {
                ParameterName = ctrl.parameter.name,
                ParameterValue = ctrl.value
            };
            var subControllers = ParametersToItemControllers(ctrl.value, ctrl.subParameters);
            var subLabels = VRCLabelsToDKLabels(ctrl.labels);

            MenuItem item;
            switch (ctrl.type)
            {
                case VRCExpressionsMenu.Control.ControlType.Button:
                    item = new ButtonItem() { Controller = controller };
                    break;
                case VRCExpressionsMenu.Control.ControlType.Toggle:
                    item = new ToggleItem() { Controller = controller };
                    break;
                case VRCExpressionsMenu.Control.ControlType.SubMenu:
                    item = new VRCSubMenuItem()
                    {
                        ControllerOnOpen = controller,
                        SubMenu = ctrl.subMenu
                    };
                    break;
                case VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet:
                    item = new TwoAxisItem()
                    {
                        ControllerOnOpen = controller,
                        HorizontalController = TryGetOrDefault(subControllers, 0),
                        VerticalController = TryGetOrDefault(subControllers, 1),
                        UpLabel = TryGetOrDefault(subLabels, 0),
                        RightLabel = TryGetOrDefault(subLabels, 1),
                        DownLabel = TryGetOrDefault(subLabels, 2),
                        LeftLabel = TryGetOrDefault(subLabels, 3),
                    };
                    break;
                case VRCExpressionsMenu.Control.ControlType.FourAxisPuppet:
                    item = new FourAxisItem()
                    {
                        ControllerOnOpen = controller,
                        UpController = TryGetOrDefault(subControllers, 0),
                        RightController = TryGetOrDefault(subControllers, 1),
                        DownController = TryGetOrDefault(subControllers, 2),
                        LeftController = TryGetOrDefault(subControllers, 3),
                        UpLabel = TryGetOrDefault(subLabels, 0),
                        RightLabel = TryGetOrDefault(subLabels, 1),
                        DownLabel = TryGetOrDefault(subLabels, 2),
                        LeftLabel = TryGetOrDefault(subLabels, 3),
                    };
                    break;
                case VRCExpressionsMenu.Control.ControlType.RadialPuppet:
                    item = new RadialItem()
                    {
                        ControllerOnOpen = controller,
                        RadialController = TryGetOrDefault(subControllers, 0)
                    };
                    break;
                default:
                    return null;
            }

            item.Name = ctrl.name;
            item.Icon = ctrl.icon;

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
                        ctx?.CreateUniqueAsset(subMenu, $"{item.Name}_SubMenu_{DKEditorUtils.RandomString(6)}.asset");
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
            foreach (var menuItem in menuGroup.Items)
            {
                vrcMenu.controls.Add(MenuItemToControl(menuItem, ctx));
            }
            return vrcMenu;
        }
    }
}
#endif
