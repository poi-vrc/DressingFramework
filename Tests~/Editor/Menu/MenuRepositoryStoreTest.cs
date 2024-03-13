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

using Chocopoi.DressingFramework.Detail.DK;
using Chocopoi.DressingFramework.Menu;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Menu
{
    public class MenuRepositoryStoreTest : EditorTestBase
    {
        private class TestMenuRepositoryStore : MenuRepositoryStore
        {
            private readonly MenuGroup _rootMenu;

            public TestMenuRepositoryStore(Context ctx) : base(ctx)
            {
                _rootMenu = new MenuGroup();
            }

            public override IMenuRepository GetRootMenu() => _rootMenu;

            internal override void OnDisable()
            {
            }

            internal override void OnEnable()
            {
            }
        }

        [Test]
        public void SimpleFlushTest()
        {
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var store = new TestMenuRepositoryStore(ctx);
            var realMenu = store.GetRootMenu();
            var menuItem = new ToggleItem();

            store.Append(menuItem);
            Assert.AreEqual(0, realMenu.Count(), "Should be empty");

            store.Flush();
            Assert.AreEqual(1, realMenu.Count());
            Assert.AreEqual(menuItem, realMenu[0]);
        }

        private void AssertThroughPath(MenuGroup mg, string[] paths, int index, MenuItem expectedItem)
        {
            if (index < paths.Length)
            {
                var found = false;
                foreach (var mi in mg)
                {
                    if (mi is SubMenuItem subMenuItem && subMenuItem.Name == paths[index])
                    {
                        found = true;
                        AssertThroughPath(subMenuItem.SubMenu, paths, index + 1, expectedItem);
                        break;
                    }
                }
                Assert.True(found, $"Sub-menu {paths[index]} not found through {string.Join("/", paths)}");
            }
            else
            {
                AssertHasMenuItem(mg, expectedItem);
            }
        }

        private void AssertHasMenuItem(MenuGroup mg, MenuItem expectedItem)
        {
            var found = false;
            foreach (var mi in mg)
            {
                if (mi == expectedItem)
                {
                    found = true;
                    break;
                }
            }
            Assert.True(found, $"Menu item not found");
        }

        [Test]
        public void InstallToPathNoExistingSubMenuTest()
        {
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var store = new TestMenuRepositoryStore(ctx);
            var realMenu = (MenuGroup)store.GetRootMenu();

            var menuItem1 = new ToggleItem();
            store.Append(menuItem1, "A");

            var menuItem2 = new ToggleItem();
            store.Append(menuItem2, "B/C");

            var menuItem3 = new ToggleItem();
            store.Append(menuItem3, "C/D/E");

            store.Flush();

            AssertThroughPath(realMenu, new string[] { "A" }, 0, menuItem1);
            AssertThroughPath(realMenu, new string[] { "B", "C" }, 0, menuItem2);
            AssertThroughPath(realMenu, new string[] { "C", "D", "E" }, 0, menuItem3);
        }

        [Test]
        public void InstallToPathExistingSubMenuTest()
        {
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var store = new TestMenuRepositoryStore(ctx);
            var realMenu = (MenuGroup)store.GetRootMenu();
            realMenu.Add(new SubMenuItem()
            {
                Name = "A",
                SubMenu = new MenuGroup()
            });
            realMenu.Add(new SubMenuItem()
            {
                Name = "B",
                SubMenu = new MenuGroup()
            });
            realMenu.Add(new SubMenuItem()
            {
                Name = "C",
                SubMenu = null
            });

            var menuItem1 = new ToggleItem();
            store.Append(menuItem1, "A");

            var menuItem2 = new ToggleItem();
            store.Append(menuItem2, "B/C");

            var menuItem3 = new ToggleItem();
            store.Append(menuItem3, "C/D/E");

            store.Flush();

            AssertThroughPath(realMenu, new string[] { "A" }, 0, menuItem1);
            AssertThroughPath(realMenu, new string[] { "B", "C" }, 0, menuItem2);
            AssertThroughPath(realMenu, new string[] { "C", "D", "E" }, 0, menuItem3);
        }
    }
}
