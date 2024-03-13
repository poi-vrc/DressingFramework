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

#if DK_MA && DK_VRCSDK3A
using Chocopoi.DressingFramework.Detail.DK;
using Chocopoi.DressingFramework.Menu;
using nadena.dev.modular_avatar.core;
using NUnit.Framework;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Tests.Detail.DK
{
    public class DKMAMenuStoreTest : EditorTestBase
    {
        [Test]
        public void SimpleFlushTest()
        {
            var avatar = CreateGameObject("Avatar");
            var avatarDesc = avatar.AddComponent<VRCAvatarDescriptor>();
            var dummyMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            avatarDesc.expressionsMenu = dummyMenu;

            var ctx = new DKNativeContext(avatar);
            var store = new DKMAMenuStore(ctx);

            var magicName = "Item";
            var magicParam = "SomeParam";
            var magicFloat = 0.75f;
            var menuItem = new ToggleItem()
            {
                Name = magicName,
                Icon = null,
                Controller = new AnimatorParameterController()
                {
                    ParameterName = magicParam,
                    ParameterValue = magicFloat
                }
            };
            store.Append(menuItem);
            store.Flush();

            var menuRoot = avatar.transform.Find("DKMAMenu/Root");
            Assert.NotNull(menuRoot);

            Assert.True(menuRoot.TryGetComponent<ModularAvatarMenuInstaller>(out var installer));
            Assert.AreEqual(dummyMenu, installer.installTargetMenu);
            Assert.True(menuRoot.TryGetComponent<ModularAvatarMenuGroup>(out _));
            Assert.AreEqual(1, menuRoot.transform.childCount);

            var child = menuRoot.transform.GetChild(0);
            Assert.True(child.TryGetComponent<ModularAvatarMenuItem>(out var item));
            Assert.NotNull(item.Control);
            Assert.AreEqual(magicName, item.Control.name);
            Assert.AreEqual(magicParam, item.Control.parameter.name);
            Assert.AreEqual(magicFloat, item.Control.value);
        }

        private static VRCExpressionsMenu FindMenuThroughPath(VRCExpressionsMenu menu, string[] paths, int index)
        {
            if (index < paths.Length)
            {
                foreach (var ctrl in menu.controls)
                {
                    if (ctrl.type == VRCExpressionsMenu.Control.ControlType.SubMenu && ctrl.name == paths[index])
                    {
                        Assert.NotNull(ctrl.subMenu, $"Submenu null at {paths[index]} through {string.Join("/", paths)}");
                        return FindMenuThroughPath(ctrl.subMenu, paths, index + 1);
                    }
                }
                Assert.Fail($"Sub-menu {paths[index]} not found through {string.Join("/", paths)}");
                return null;
            }
            else
            {
                return menu;
            }
        }

        private static void AssertMAItems(GameObject avatar, VRCExpressionsMenu rootMenu, MenuItem dkItem1, MenuItem dkItem2, MenuItem dkItem3)
        {
            var vrcMenu1 = FindMenuThroughPath(rootMenu, new string[] { "A" }, 0);
            var vrcMenu2 = FindMenuThroughPath(rootMenu, new string[] { "B", "C" }, 0);
            var vrcMenu3 = FindMenuThroughPath(rootMenu, new string[] { "C", "D", "E" }, 0);

            var menu1 = avatar.transform.Find("DKMAMenu/A");
            Assert.NotNull(menu1);
            var menu2 = avatar.transform.Find("DKMAMenu/B_C");
            Assert.NotNull(menu2);
            var menu3 = avatar.transform.Find("DKMAMenu/C_D_E");
            Assert.NotNull(menu3);

            Assert.True(menu1.TryGetComponent<ModularAvatarMenuInstaller>(out var installer1));
            Assert.AreEqual(vrcMenu1, installer1.installTargetMenu);
            Assert.True(menu2.TryGetComponent<ModularAvatarMenuInstaller>(out var installer2));
            Assert.AreEqual(vrcMenu2, installer2.installTargetMenu);
            Assert.True(menu3.TryGetComponent<ModularAvatarMenuInstaller>(out var installer3));
            Assert.AreEqual(vrcMenu3, installer3.installTargetMenu);

            Assert.AreEqual(1, menu1.childCount);
            Assert.True(menu1.GetChild(0).TryGetComponent<ModularAvatarMenuItem>(out var maItem1));
            Assert.NotNull(maItem1.Control);
            Assert.AreEqual(dkItem1.Name, maItem1.Control.name);
            Assert.AreEqual(VRCExpressionsMenu.Control.ControlType.Toggle, maItem1.Control.type);

            Assert.AreEqual(1, menu2.childCount);
            Assert.True(menu2.GetChild(0).TryGetComponent<ModularAvatarMenuItem>(out var maItem2));
            Assert.NotNull(maItem2.Control);
            Assert.AreEqual(dkItem2.Name, maItem2.Control.name);
            Assert.AreEqual(VRCExpressionsMenu.Control.ControlType.Toggle, maItem2.Control.type);

            Assert.AreEqual(1, menu3.childCount);
            Assert.True(menu3.GetChild(0).TryGetComponent<ModularAvatarMenuItem>(out var maItem3));
            Assert.NotNull(maItem3.Control);
            Assert.AreEqual(dkItem3.Name, maItem3.Control.name);
            Assert.AreEqual(VRCExpressionsMenu.Control.ControlType.Toggle, maItem3.Control.type);
        }

        [Test]
        public void InstallToPathNoExistingSubMenuTest()
        {
            var avatar = CreateGameObject("Avatar");
            var avatarDesc = avatar.AddComponent<VRCAvatarDescriptor>();
            var rootMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            avatarDesc.expressionsMenu = rootMenu;

            var ctx = new DKNativeContext(avatar);
            var store = new DKMAMenuStore(ctx);

            var dkItem1 = new ToggleItem() { Name = "1" };
            store.Append(dkItem1, "A");

            var dkItem2 = new ToggleItem() { Name = "2" };
            store.Append(dkItem2, "B/C");

            var dkItem3 = new ToggleItem() { Name = "3" };
            store.Append(dkItem3, "C/D/E");

            store.Flush();

            AssertMAItems(avatar, rootMenu, dkItem1, dkItem2, dkItem3);
        }

        private static void AddMenusThroughPath(VRCExpressionsMenu menu, string[] paths, int index)
        {
            if (index < paths.Length)
            {
                var newMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
                menu.controls.Add(new VRCExpressionsMenu.Control()
                {
                    name = paths[index],
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = newMenu
                });
                AddMenusThroughPath(newMenu, paths, index + 1);
            }
        }

        [Test]
        public void InstallToPathExistingSubMenuTest()
        {
            var avatar = CreateGameObject("Avatar");
            var avatarDesc = avatar.AddComponent<VRCAvatarDescriptor>();
            var rootMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            avatarDesc.expressionsMenu = rootMenu;

            AddMenusThroughPath(rootMenu, new string[] { "A" }, 0);
            AddMenusThroughPath(rootMenu, new string[] { "B", "C" }, 0);
            AddMenusThroughPath(rootMenu, new string[] { "C", "D", "E" }, 0);

            var ctx = new DKNativeContext(avatar);
            var store = new DKMAMenuStore(ctx);

            var dkItem1 = new ToggleItem() { Name = "1" };
            store.Append(dkItem1, "A");

            var dkItem2 = new ToggleItem() { Name = "2" };
            store.Append(dkItem2, "B/C");

            var dkItem3 = new ToggleItem() { Name = "3" };
            store.Append(dkItem3, "C/D/E");

            store.Flush();

            // the store will clone a copy of it, so the original cannot be used for asserts
            AssertMAItems(avatar, rootMenu, dkItem1, dkItem2, dkItem3);
        }
    }
}
#endif
