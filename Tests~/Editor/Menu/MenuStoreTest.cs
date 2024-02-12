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

using Chocopoi.DressingFramework.Menu;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Menu
{
    public class MenuStoreTest : EditorTestBase
    {
        private class TestMenuStore : MenuStore
        {
            private readonly MenuGroup _rootMenu;

            public TestMenuStore()
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
        public void FlushTest()
        {
            var store = new TestMenuStore();
            var realMenu = store.GetRootMenu();
            var menuItem = new ToggleItem();

            store.Append(menuItem);
            Assert.AreEqual(0, realMenu.Count(), "Should be empty");

            store.Flush();
            Assert.AreEqual(1, realMenu.Count());
            Assert.AreEqual(menuItem, realMenu[0]);
        }
    }
}
