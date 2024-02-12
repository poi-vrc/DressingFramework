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

namespace Chocopoi.DressingFramework.Menu
{
    /// <summary>
    /// Abstract menu store base class
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    public abstract class MenuStore : ContextFeature, IMenuStore
    {
        private MenuGroup _buffer;

        public MenuStore()
        {
            _buffer = new MenuGroup();
        }

        public void Append(MenuItem menuItem)
        {
            _buffer.Add(menuItem);
        }

        public void Flush()
        {
            var rootMenu = GetRootMenu();
            foreach (var menuItem in _buffer)
            {
                rootMenu.Add(menuItem);
            }
            _buffer.Clear();
        }

        public abstract IMenuRepository GetRootMenu();
    }
}
