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
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using NUnit.Framework;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Tests.Menu.VRChat
{
    public class VRCMenuUtilsTest : EditorTestBase
    {
        [Test]
        public void GetDefaultAssetsTest()
        {
            Assert.NotNull(VRCMenuUtils.GetDefaultExpressionsParameters());
            Assert.NotNull(VRCMenuUtils.GetDefaultExpressionsMenu());
        }

        [Test]
        public void GetExpressionsParametersTest()
        {
            var obj = CreateGameObject("TestAvatarObject");
            var avatarDesc = obj.AddComponent<VRCAvatarDescriptor>();
            var defaultParams = VRCMenuUtils.GetDefaultExpressionsParameters();
            Assert.NotNull(defaultParams);

            avatarDesc.expressionParameters = null;
            Assert.AreEqual(defaultParams, VRCMenuUtils.GetExpressionsParameters(avatarDesc));

            var exParams = ScriptableObject.CreateInstance<VRCExpressionParameters>();
            avatarDesc.expressionParameters = exParams;
            Assert.AreEqual(exParams, VRCMenuUtils.GetExpressionsParameters(avatarDesc));
        }

        [Test]
        public void GetExpressionsMenuTest()
        {
            var obj = CreateGameObject("TestAvatarObject");
            var avatarDesc = obj.AddComponent<VRCAvatarDescriptor>();
            var defaultMenu = VRCMenuUtils.GetDefaultExpressionsMenu();
            Assert.NotNull(defaultMenu);

            avatarDesc.expressionsMenu = null;
            Assert.AreEqual(defaultMenu, VRCMenuUtils.GetExpressionsMenu(avatarDesc));

            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            avatarDesc.expressionsMenu = exMenu;
            Assert.AreEqual(exMenu, VRCMenuUtils.GetExpressionsMenu(avatarDesc));
        }

        private static VRCExpressionsMenu.Control.Parameter[] GenerateVRCParameters(int len)
        {
            var output = new VRCExpressionsMenu.Control.Parameter[len];
            for (var i = 0; i < len; i++)
            {
                output[i] = new VRCExpressionsMenu.Control.Parameter()
                {
                    name = i.ToString()
                };
            }
            return output;
        }

        private static VRCExpressionsMenu.Control.Label[] GenerateVRCLabels(int len)
        {
            var output = new VRCExpressionsMenu.Control.Label[len];
            for (var i = 0; i < len; i++)
            {
                output[i] = new VRCExpressionsMenu.Control.Label()
                {
                    name = i.ToString(),
                    icon = null
                };
            }
            return output;
        }

        private static void TestVRCControlToSingleControllerItem<T>(VRCExpressionsMenu.Control.Parameter param, VRCExpressionsMenu.Control.ControlType type) where T : SingleControllerItem
        {
            var control = new VRCExpressionsMenu.Control()
            {
                name = "ctrl",
                icon = null,
                type = type,
                parameter = param,
                value = 0.75f,
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = null
            };

            var result = VRCMenuUtils.ControlToMenuItem(control);
            Assert.IsInstanceOf<T>(result);
            var item = (T)result;
            Assert.AreEqual(control.name, item.Name);
            Assert.NotNull(item.Controller);

            Assert.IsInstanceOf<AnimatorParameterController>(item.Controller);
            var ctrl = (AnimatorParameterController)item.Controller;
            Assert.AreEqual(control.parameter.name, ctrl.ParameterName);
            Assert.AreEqual(control.value, ctrl.ParameterValue);
        }

        private static void TestVRCControlToControllerOnOpenItem<T>(VRCExpressionsMenu.Control.Parameter param, VRCExpressionsMenu.Control.ControlType type, int paramsLen, int labelsLen, Action<VRCExpressionsMenu.Control, VRCExpressionsMenu.Control.Parameter[], VRCExpressionsMenu.Control.Label[], T> func) where T : ControllerOnOpenItem
        {
            var parameters = GenerateVRCParameters(paramsLen);
            var labels = GenerateVRCLabels(labelsLen);

            var control = new VRCExpressionsMenu.Control()
            {
                name = "ctrl",
                icon = null,
                type = type,
                parameter = param,
                value = 0.75f,
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = null,
                subParameters = parameters,
                labels = labels
            };

            var result = VRCMenuUtils.ControlToMenuItem(control);
            Assert.IsInstanceOf<T>(result);
            var item = (T)result;
            Assert.AreEqual(control.name, item.Name);
            Assert.NotNull(item.ControllerOnOpen);

            Assert.IsInstanceOf<AnimatorParameterController>(item.ControllerOnOpen);
            var ctrl = (AnimatorParameterController)item.ControllerOnOpen;
            Assert.AreEqual(control.parameter.name, ctrl.ParameterName);
            Assert.AreEqual(control.value, ctrl.ParameterValue);

            func(control, parameters, labels, item);
        }

        [Test]
        public void ControlToMenuItemTest()
        {
            var singleVrcParam = new VRCExpressionsMenu.Control.Parameter()
            {
                name = "someVrcParam"
            };

            TestVRCControlToSingleControllerItem<ButtonItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.Button);
            TestVRCControlToSingleControllerItem<ToggleItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.Toggle);

            TestVRCControlToControllerOnOpenItem<VRCSubMenuItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.SubMenu, 0, 0, (control, parameters, labels, item) => { });

            TestVRCControlToControllerOnOpenItem<TwoAxisItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.TwoAxisPuppet, 2, 4, (control, parameters, labels, item) =>
            {
                Assert.NotNull(item.HorizontalController);
                Assert.NotNull(item.VerticalController);

                Assert.IsInstanceOf<AnimatorParameterController>(item.HorizontalController);
                Assert.IsInstanceOf<AnimatorParameterController>(item.VerticalController);

                var horiCtrl = (AnimatorParameterController)item.HorizontalController;
                var vertCtrl = (AnimatorParameterController)item.VerticalController;

                Assert.AreEqual(control.subParameters[0].name, horiCtrl.ParameterName);
                Assert.AreEqual(control.subParameters[1].name, vertCtrl.ParameterName);

                Assert.AreEqual(control.labels[0].name, item.UpLabel.Name);
                Assert.AreEqual(control.labels[1].name, item.RightLabel.Name);
                Assert.AreEqual(control.labels[2].name, item.DownLabel.Name);
                Assert.AreEqual(control.labels[3].name, item.LeftLabel.Name);
            });

            TestVRCControlToControllerOnOpenItem<FourAxisItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.FourAxisPuppet, 4, 4, (control, parameters, labels, item) =>
            {
                Assert.NotNull(item.UpController);
                Assert.NotNull(item.RightController);
                Assert.NotNull(item.DownController);
                Assert.NotNull(item.LeftController);

                Assert.IsInstanceOf<AnimatorParameterController>(item.UpController);
                Assert.IsInstanceOf<AnimatorParameterController>(item.RightController);
                Assert.IsInstanceOf<AnimatorParameterController>(item.DownController);
                Assert.IsInstanceOf<AnimatorParameterController>(item.LeftController);

                var upCtrl = (AnimatorParameterController)item.UpController;
                var rightCtrl = (AnimatorParameterController)item.RightController;
                var downCtrl = (AnimatorParameterController)item.DownController;
                var leftCtrl = (AnimatorParameterController)item.LeftController;

                Assert.AreEqual(control.subParameters[0].name, upCtrl.ParameterName);
                Assert.AreEqual(control.subParameters[1].name, rightCtrl.ParameterName);
                Assert.AreEqual(control.subParameters[2].name, downCtrl.ParameterName);
                Assert.AreEqual(control.subParameters[3].name, leftCtrl.ParameterName);

                Assert.AreEqual(control.labels[0].name, item.UpLabel.Name);
                Assert.AreEqual(control.labels[1].name, item.RightLabel.Name);
                Assert.AreEqual(control.labels[2].name, item.DownLabel.Name);
                Assert.AreEqual(control.labels[3].name, item.LeftLabel.Name);
            });

            TestVRCControlToControllerOnOpenItem<RadialItem>(singleVrcParam, VRCExpressionsMenu.Control.ControlType.RadialPuppet, 1, 0, (control, parameters, labels, item) =>
            {
                Assert.NotNull(item.RadialController);

                Assert.IsInstanceOf<AnimatorParameterController>(item.RadialController);

                var radCtrl = (AnimatorParameterController)item.RadialController;

                Assert.AreEqual(control.subParameters[0].name, radCtrl.ParameterName);
            });
        }

        private static void AssertAbstractItemToVRCControl(MenuItem item, VRCExpressionsMenu.Control control)
        {
            Assert.AreEqual(item.Name, control.name);
            Assert.AreEqual(item.Icon, control.icon);
        }

        private static void AssertSingleControllerItemToVRCControl(SingleControllerItem item, VRCExpressionsMenu.Control control)
        {
            Assert.IsInstanceOf<AnimatorParameterController>(item.Controller);
            var apCtrl = (AnimatorParameterController)item.Controller;
            Assert.AreEqual(apCtrl.ParameterName, control.parameter.name);
            Assert.AreEqual(apCtrl.ParameterValue, control.value);
        }

        private static void AssertControllerOnOpenItemToVRCControl(ControllerOnOpenItem item, VRCExpressionsMenu.Control control)
        {
            Assert.IsInstanceOf<AnimatorParameterController>(item.ControllerOnOpen);
            var apCtrl = (AnimatorParameterController)item.ControllerOnOpen;
            Assert.AreEqual(apCtrl.ParameterName, control.parameter.name);
            Assert.AreEqual(apCtrl.ParameterValue, control.value);
        }

        private static void TestSingleControllerItemToControl(SingleControllerItem item)
        {
            item.Name = "item";
            item.Icon = null;
            item.Controller = new AnimatorParameterController()
            {
                ParameterName = "someParam",
                ParameterValue = 0.75f
            };

            // TODO: ctx
            var ctrl = VRCMenuUtils.MenuItemToControl(item);

            AssertAbstractItemToVRCControl(item, ctrl);
            AssertSingleControllerItemToVRCControl(item, ctrl);
        }

        private static void TestControllerOnOpenItemToControl<T>(T item, Action<T, VRCExpressionsMenu.Control> func) where T : ControllerOnOpenItem
        {
            item.Name = "item";
            item.Icon = null;
            item.ControllerOnOpen = new AnimatorParameterController()
            {
                ParameterName = "someParam",
                ParameterValue = 0.75f
            };

            // TODO: ctx
            var ctrl = VRCMenuUtils.MenuItemToControl(item);

            AssertAbstractItemToVRCControl(item, ctrl);
            AssertControllerOnOpenItemToVRCControl(item, ctrl);

            func(item, ctrl);
        }

        [Test]
        public void MenuItemToControlTest()
        {
            TestSingleControllerItemToControl(new ButtonItem());
            TestSingleControllerItemToControl(new ToggleItem());

            TestControllerOnOpenItemToControl(new SubMenuItem()
            {
                SubMenu = new MenuGroup() { new ToggleItem() }
            }, (item, ctrl) =>
            {
                Assert.NotNull(ctrl.subMenu);
                Assert.AreEqual(1, ctrl.subMenu.controls.Count);
                Assert.AreEqual(VRCExpressionsMenu.Control.ControlType.Toggle, ctrl.subMenu.controls[0].type);
            });

            TestControllerOnOpenItemToControl(new TwoAxisItem()
            {
                HorizontalController = new AnimatorParameterController() { ParameterName = "1" },
                VerticalController = new AnimatorParameterController() { ParameterName = "2" },
                UpLabel = new MenuItem.Label() { Name = "a" },
                RightLabel = new MenuItem.Label() { Name = "b" },
                DownLabel = new MenuItem.Label() { Name = "c" },
                LeftLabel = new MenuItem.Label() { Name = "d" }
            }, (item, ctrl) =>
            {
                Assert.AreEqual(2, ctrl.subParameters.Length);
                Assert.AreEqual("1", ctrl.subParameters[0].name);
                Assert.AreEqual("2", ctrl.subParameters[1].name);

                Assert.AreEqual(4, ctrl.labels.Length);
                Assert.AreEqual("a", ctrl.labels[0].name);
                Assert.AreEqual("b", ctrl.labels[1].name);
                Assert.AreEqual("c", ctrl.labels[2].name);
                Assert.AreEqual("d", ctrl.labels[3].name);
            });

            TestControllerOnOpenItemToControl(new FourAxisItem()
            {
                UpController = new AnimatorParameterController() { ParameterName = "1" },
                RightController = new AnimatorParameterController() { ParameterName = "2" },
                DownController = new AnimatorParameterController() { ParameterName = "3" },
                LeftController = new AnimatorParameterController() { ParameterName = "4" },
                UpLabel = new MenuItem.Label() { Name = "a" },
                RightLabel = new MenuItem.Label() { Name = "b" },
                DownLabel = new MenuItem.Label() { Name = "c" },
                LeftLabel = new MenuItem.Label() { Name = "d" }
            }, (item, ctrl) =>
            {
                Assert.AreEqual(4, ctrl.subParameters.Length);
                Assert.AreEqual("1", ctrl.subParameters[0].name);
                Assert.AreEqual("2", ctrl.subParameters[1].name);
                Assert.AreEqual("3", ctrl.subParameters[2].name);
                Assert.AreEqual("4", ctrl.subParameters[3].name);

                Assert.AreEqual(4, ctrl.labels.Length);
                Assert.AreEqual("a", ctrl.labels[0].name);
                Assert.AreEqual("b", ctrl.labels[1].name);
                Assert.AreEqual("c", ctrl.labels[2].name);
                Assert.AreEqual("d", ctrl.labels[3].name);
            });

            TestControllerOnOpenItemToControl(new RadialItem()
            {
                RadialController = new AnimatorParameterController() { ParameterName = "1" },
            }, (item, ctrl) =>
            {
                Assert.AreEqual(1, ctrl.subParameters.Length);
                Assert.AreEqual("1", ctrl.subParameters[0].name);
            });
        }
    }
}
#endif
