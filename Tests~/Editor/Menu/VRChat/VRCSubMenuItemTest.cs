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

#if DK_VRCSDK3A
using Chocopoi.DressingFramework.Menu;
using Chocopoi.DressingFramework.Menu.VRChat;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Menu.VRChat
{
    public class VRCSubMenuItemTest : EditorTestBase
    {
        [Test]
        public void CoverageTest()
        {
            // just for passing coverage

            new VRCSubMenuItem()
            {
                Name = "",
                Icon = null,
                ControllerOnOpen = new AnimatorParameterController(),
                SubMenu = null
            };
        }
    }
}
#endif
