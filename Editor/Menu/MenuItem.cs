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

using UnityEngine;

namespace Chocopoi.DressingFramework.Menu
{
    /// <summary>
    /// DK Menu Item.
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    public abstract class MenuItem
    {
        public class Label
        {
            public string Name { get; set; }

            public Texture2D Icon { get; set; }
        }

        public string Name { get; set; }

        public Texture2D Icon { get; set; }
    }
}
