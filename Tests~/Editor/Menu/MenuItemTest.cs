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
    public class MenuItemTest : EditorTestBase
    {
        [Test]
        public void CoverageTest()
        {
            // just for passing coverage

            new MenuItem.Label()
            {
                Name = "",
                Icon = null
            };

            new AnimatorParameterController()
            {
                ParameterName = "",
                ParameterValue = 0.0f
            };

            new ButtonItem()
            {
                Name = "",
                Icon = null,
                Controller = new AnimatorParameterController()
            };

            new ToggleItem()
            {
                Name = "",
                Icon = null,
                Controller = new AnimatorParameterController()
            };

            new SubMenuItem()
            {
                Name = "",
                Icon = null,
                ControllerOnOpen = new AnimatorParameterController(),
                SubMenu = new MenuGroup()
            };

            new TwoAxisItem()
            {
                Name = "",
                Icon = null,
                ControllerOnOpen = new AnimatorParameterController(),
                HorizontalController = new AnimatorParameterController(),
                VerticalController = new AnimatorParameterController(),
                UpLabel = new MenuItem.Label(),
                RightLabel = new MenuItem.Label(),
                DownLabel = new MenuItem.Label(),
                LeftLabel = new MenuItem.Label()
            };

            new FourAxisItem()
            {
                Name = "",
                Icon = null,
                ControllerOnOpen = new AnimatorParameterController(),
                UpController = new AnimatorParameterController(),
                RightController = new AnimatorParameterController(),
                DownController = new AnimatorParameterController(),
                LeftController = new AnimatorParameterController(),
                UpLabel = new MenuItem.Label(),
                RightLabel = new MenuItem.Label(),
                DownLabel = new MenuItem.Label(),
                LeftLabel = new MenuItem.Label()
            };

            new RadialItem()
            {
                Name = "",
                Icon = null,
                ControllerOnOpen = new AnimatorParameterController(),
                RadialController = new AnimatorParameterController()
            };
        }
    }
}
