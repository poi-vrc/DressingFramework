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

using System.Collections.Generic;
using Chocopoi.DressingFramework.Menu;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Menu
{
    public class MenuGroupTest : EditorTestBase
    {
        [Test]
        public void CrudTest()
        {
            var gp = new MenuGroup();
            var item1 = new ToggleItem();
            var item2 = new ToggleItem();

            Assert.AreEqual(0, gp.Count());

            gp.Add(item1);
            gp.Add(item2);
            Assert.AreEqual(2, gp.Count());
            Assert.AreEqual(item1, gp[0]);
            Assert.AreEqual(item2, gp[1]);

            gp.Remove(0);
            Assert.AreEqual(item2, gp[0]);

            gp.Insert(0, item1);
            Assert.AreEqual(item1, gp[0]);
            Assert.AreEqual(item2, gp[1]);

            var seq = new List<MenuItem>();
            foreach (var item in gp)
            {
                seq.Add(item);
            }
            Assert.AreEqual(item1, seq[0]);
            Assert.AreEqual(item2, seq[1]);

            gp.Clear();
            Assert.AreEqual(0, gp.Count());
        }
    }
}
