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
    /// Menu store interface
    /// </summary>
    internal interface IMenuStore
    {
        /// <summary>
        /// Schedule an append of single menu item.
        /// </summary>
        /// <param name="menuItem">Menu item</param>
        void Append(MenuItem menuItem, string path = null);

        /// <summary>
        /// Flush the changes onto the avatar's root menu
        /// </summary>
        void Flush();
    }
}
