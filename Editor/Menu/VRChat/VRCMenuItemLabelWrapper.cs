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

#if DK_VRCSDK3A
using System;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal class VRCMenuItemLabelWrapper : MenuItem.Label
    {
        public override string Name
        {
            get => _readFunc().name;
            set
            {
                var label = _readFunc();
                label.name = value;
                _writeFunc(label);
            }
        }
        public override Texture2D Icon
        {
            get => _readFunc().icon;
            set
            {
                var label = _readFunc();
                label.icon = value;
                _writeFunc(label);
            }
        }

        private readonly Func<VRCExpressionsMenu.Control.Label> _readFunc;
        private readonly Action<VRCExpressionsMenu.Control.Label> _writeFunc;

        public VRCMenuItemLabelWrapper(Func<VRCExpressionsMenu.Control.Label> readFunc, Action<VRCExpressionsMenu.Control.Label> writeFunc)
        {
            // it's a struct! it's a struct! it's a struct!!! :(
            // since struct are copy by value, we can only perform writes through this method
            _readFunc = readFunc;
            _writeFunc = writeFunc;
        }
    }
}
#endif
