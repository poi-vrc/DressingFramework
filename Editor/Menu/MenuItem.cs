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

using System;
using UnityEngine;

namespace Chocopoi.DressingFramework.Menu
{
    /// <summary>
    /// DK Menu Item.
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    [Serializable]
    public class MenuItem
    {
        public enum ItemType
        {
            Button = 0,
            Toggle = 1,
            SubMenu = 2,
            TwoAxis = 3,
            FourAxis = 4,
            Radial = 5
        }

        [Serializable]
        public struct Label
        {
            public string name;

            public Texture2D icon;
        }

        [Serializable]
        public struct ItemController
        {
            public enum ControllerType
            {
                AnimatorParameter = 0
            }

            public ControllerType type;

            public string animatorParameterName;

            public float animatorParameterValue;
        }

        public string name;

        public Texture2D icon;

        public ItemType type;

        public ItemController controller;

        public MenuGroup subMenu;

        public ItemController[] subControllers;

        public Label[] subLabels;
    }
}
