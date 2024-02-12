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

using System.Collections;
using System.Collections.Generic;

namespace Chocopoi.DressingFramework.Menu
{
    /// <summary>
    /// DK Menu Group.
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    ///
    public class MenuGroup : IMenuRepository
    {
        public List<MenuItem> Items { get; private set; }

        public MenuItem this[int index] { get => Items[index]; set => Items[index] = value; }

        public MenuGroup()
        {
            Items = new List<MenuItem>();
        }

        public void Add(MenuItem menuItem) => Items.Add(menuItem);
        public void Clear() => Items.Clear();
        public int Count() => Items.Count;
        public IEnumerator<MenuItem> GetEnumerator() => Items.GetEnumerator();
        public void Insert(int index, MenuItem menuItem) => Items.Insert(index, menuItem);
        public void Remove(int index) => Items.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }
}
