﻿/*
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
    /// Menu repository interface. Defines basic CRUD operations on the menu.
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    public interface IMenuRepository : IEnumerable, IEnumerable<MenuItem>
    {
        void Add(MenuItem menuItem);

        void Insert(int index, MenuItem menuItem);

        int Count();

        void Remove(int index);

        void Clear();

        MenuItem this[int index] { get; set; }
    }
}
