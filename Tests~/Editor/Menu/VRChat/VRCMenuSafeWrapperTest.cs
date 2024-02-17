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

using System.Collections.Generic;
using Chocopoi.DressingFramework.Detail.DK;
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using NUnit.Framework;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Tests.Menu.VRChat
{
    public class VRCMenuSafeWrapperTest : EditorTestBase
    {
        private static void CreateItem(VRCExpressionsMenu menu, string name = null)
        {
            menu.controls.Add(new VRCExpressionsMenu.Control()
            {
                name = name,
                icon = null,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter() { name = "" },
                value = 1f,
                style = VRCExpressionsMenu.Control.Style.Style1,
                subMenu = null,
                subParameters = new VRCExpressionsMenu.Control.Parameter[0],
                labels = new VRCExpressionsMenu.Control.Label[0]
            });
        }

        [Test]
        public void TopMenuExtendTest()
        {
            // create some dummies to fill up the menu to MAX_CONTROLS
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            for (var i = 0; i < VRCExpressionsMenu.MAX_CONTROLS; i++)
            {
                CreateItem(exMenu, i.ToString());
            }
            Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, exMenu.controls.Count);

            // create wrapper
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);

            // expect to have no operations at all and keep it max controls
            Assert.AreEqual(1, wrapper.GetContainingMenus().Count);
            Assert.AreEqual("7", exMenu.controls[VRCExpressionsMenu.MAX_CONTROLS - 1].name);

            // add a dummy to trigger extend
            wrapper.Add(new ToggleItem() { Name = "101" });

            // two menus should be created
            Assert.AreEqual(2, wrapper.GetContainingMenus().Count);

            var menu1 = wrapper.GetContainingMenus()[0];
            Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu1.controls.Count);
            Assert.AreEqual("6", menu1.controls[VRCExpressionsMenu.MAX_CONTROLS - 2].name);
            Assert.AreEqual("Next Page", menu1.controls[VRCExpressionsMenu.MAX_CONTROLS - 1].name);

            var menu2 = wrapper.GetContainingMenus()[1];
            Assert.AreEqual(2, menu2.controls.Count);
            Assert.AreEqual("7", menu2.controls[0].name);
            Assert.AreEqual("101", menu2.controls[1].name);
        }

        [Test]
        public void TopMenuNoExtendTest()
        {
            // no extend needed for less than MAX_CONTROLS
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            for (var i = 0; i < 7; i++)
            {
                CreateItem(exMenu, i.ToString());
            }
            Assert.AreEqual(7, exMenu.controls.Count);

            // create wrapper
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);

            Assert.AreEqual(1, wrapper.GetContainingMenus().Count);
            Assert.AreEqual("6", exMenu.controls[VRCMenuSafeWrapper.MaxUsableControls - 1].name);

            // add a dummy to trigger create new page item
            wrapper.Add(new ToggleItem() { Name = "101" });

            // two menus should be created
            Assert.AreEqual(2, wrapper.GetContainingMenus().Count);

            var menu1 = wrapper.GetContainingMenus()[0];
            Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu1.controls.Count);
            Assert.AreEqual("6", menu1.controls[VRCExpressionsMenu.MAX_CONTROLS - 2].name);
            Assert.AreEqual("Next Page", menu1.controls[VRCExpressionsMenu.MAX_CONTROLS - 1].name);

            var menu2 = wrapper.GetContainingMenus()[1];
            Assert.AreEqual(1, menu2.controls.Count);
            Assert.AreEqual("101", menu2.controls[0].name);
        }

        [Test]
        public void AddTest()
        {
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            for (var i = 0; i < 7; i++)
            {
                CreateItem(exMenu, i.ToString());
            }
            Assert.AreEqual(7, exMenu.controls.Count);

            // create wrapper
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);

            var newPages = 2;
            var times = VRCMenuSafeWrapper.MaxUsableControls * newPages;

            var count = 0;
            for (var i = 0; i < times; i++)
            {
                wrapper.Add(new ToggleItem() { Name = count++.ToString() });
            }

            Assert.AreEqual(newPages + 1, wrapper.GetContainingMenus().Count);
            count = 0;
            for (var i = 1; i <= newPages; i++)
            {
                var menu = wrapper.GetContainingMenus()[i];
                if (i == newPages)
                {
                    // last page has only 7
                    Assert.AreEqual(VRCMenuSafeWrapper.MaxUsableControls, menu.controls.Count);
                }
                else
                {
                    Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu.controls.Count);
                }

                // assert menu content
                for (var j = 0; j < VRCMenuSafeWrapper.MaxUsableControls; j++)
                {
                    Assert.AreEqual(count++.ToString(), menu.controls[j].name, $"Menu item name mismatch at page {i} and item {j}");
                }

                // check for next page item
                if (i != newPages)
                {
                    Assert.AreEqual("Next Page", menu.controls[menu.controls.Count - 1].name);
                }
            }
        }

        private static void AddPages(VRCMenuSafeWrapper wrapper, int expectedPages)
        {
            var times = VRCMenuSafeWrapper.MaxUsableControls * expectedPages;
            for (var i = 0; i < times; i++)
            {
                wrapper.Add(new ToggleItem() { Name = i.ToString() });
            }

            Assert.AreEqual(expectedPages, wrapper.GetContainingMenus().Count);
        }

        [Test]
        public void ClearTest()
        {
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            Assert.AreEqual(0, exMenu.controls.Count);
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);
            AddPages(wrapper, 3);

            // collect the pointers
            var copy = new List<VRCExpressionsMenu>(wrapper.GetContainingMenus());

            wrapper.Clear();

            Assert.AreEqual(0, wrapper.Count());
            Assert.AreEqual(1, wrapper.GetContainingMenus().Count);
        }

        [Test]
        public void CountTest()
        {
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            Assert.AreEqual(0, exMenu.controls.Count);
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);

            for (var i = 0; i < 3; i++)
            {
                wrapper.Add(new ToggleItem() { Name = i.ToString() });
            }
            Assert.AreEqual(3, wrapper.Count());

            for (var i = 0; i < 7; i++)
            {
                wrapper.Add(new ToggleItem() { Name = i.ToString() });
            }
            Assert.AreEqual(10, wrapper.Count());

            for (var i = 0; i < 7; i++)
            {
                wrapper.Add(new ToggleItem() { Name = i.ToString() });
            }
            Assert.AreEqual(17, wrapper.Count());
        }
        
        [Test]
        public void EnumerationTest()
        {
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            Assert.AreEqual(0, exMenu.controls.Count);
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);
            AddPages(wrapper, 3);

            var count = 0;
            foreach(var item in wrapper) {
                Assert.AreEqual(count++.ToString(), item.Name);
            }
            Assert.AreEqual(3 * VRCMenuSafeWrapper.MaxUsableControls, count);
        }

        [Test]
        public void SetDirtyTest() {
            var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            Assert.AreEqual(0, exMenu.controls.Count);
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);
            AddPages(wrapper, 3);

            // just for coverage
            wrapper.SetAllDirty();
        }

        // [Test]
        // public void InsertTest1()
        // {
        //     var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
        //     Assert.AreEqual(0, exMenu.controls.Count);
        //     var ctx = new DKNativeContext(CreateGameObject("abc"));
        //     var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);
        //     AddPages(wrapper, 2);

        //     // 2nd page first item
        //     wrapper.Insert(7, new ToggleItem() { Name = "101" });
        //     // should grow into 3
        //     Assert.AreEqual(3, wrapper.GetContainingMenus().Count);

        //     // 2nd menu should be like 101,7,8,9,10,11,12,Next Page
        //     var menu2 = wrapper.GetContainingMenus()[1];
        //     Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu2.controls.Count);
        //     Assert.AreEqual("101", menu2.controls[0].name);
        //     for (var i = 7; i <= 12; i++)
        //     {
        //         Assert.AreEqual(i.ToString(), menu2.controls[i - 6].name);
        //     }
        //     Assert.AreEqual("Next Page", menu2.controls[menu2.controls.Count - 1].name);

        //     // 3rd menu should be like: 13
        //     var menu3 = wrapper.GetContainingMenus()[2];
        //     Assert.AreEqual(1, menu3.controls.Count);
        //     Assert.AreEqual("13", menu3.controls[0].name);
        // }

        // [Test]
        // public void InsertTest2()
        // {
        //     var exMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
        //     Assert.AreEqual(0, exMenu.controls.Count);
        //     var ctx = new DKNativeContext(CreateGameObject("abc"));
        //     var wrapper = new VRCMenuSafeWrapper(exMenu, ctx);
        //     AddPages(wrapper, 2);

        //     // 1st page forth item
        //     wrapper.Insert(3, new ToggleItem() { Name = "101" });
        //     // should grow into 3
        //     Assert.AreEqual(3, wrapper.GetContainingMenus().Count);

        //     // 1st menu should be like 0,1,2,101,3,4,5,Next Page
        //     var menu1 = wrapper.GetContainingMenus()[0];
        //     Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu1.controls.Count);
        //     for (var i = 0; i <= 2; i++)
        //     {
        //         Assert.AreEqual(i.ToString(), menu1.controls[i].name);
        //     }
        //     Assert.AreEqual("101", menu1.controls[3].name);
        //     for (var i = 3; i <= 5; i++)
        //     {
        //         Assert.AreEqual(i.ToString(), menu1.controls[i + 1].name);
        //     }
        //     Assert.AreEqual("Next Page", menu1.controls[menu1.controls.Count - 1].name);

        //     // 2nd menu should be like 6,7,8,9,10,11,12,Next Page
        //     var menu2 = wrapper.GetContainingMenus()[1];
        //     Assert.AreEqual(VRCExpressionsMenu.MAX_CONTROLS, menu2.controls.Count);
        //     for (var i = 6; i <= 12; i++)
        //     {
        //         Assert.AreEqual(i.ToString(), menu2.controls[i - 6].name);
        //     }
        //     Assert.AreEqual("Next Page", menu2.controls[menu2.controls.Count - 1].name);

        //     // 3rd menu should be like: 13
        //     var menu3 = wrapper.GetContainingMenus()[2];
        //     Assert.AreEqual(1, menu3.controls.Count);
        //     Assert.AreEqual("13", menu3.controls[0].name);
        // }
    }
}
#endif
