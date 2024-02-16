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
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Menu.VRChat
{
    internal class VRCAnimatorParameterControllerWrapper : AnimatorParameterController
    {
        public override string ParameterName { get => _parameter.name; set => _parameter.name = value; }
        public override float ParameterValue { get => _readValueFunc(); set => _writeValueFunc(value); }

        private readonly VRCExpressionsMenu.Control.Parameter _parameter;
        private readonly Func<float> _readValueFunc;
        private readonly Action<float> _writeValueFunc;

        public VRCAnimatorParameterControllerWrapper(VRCExpressionsMenu.Control.Parameter parameter, Func<float> readValueFunc, Action<float> writeValueFunc)
        {
            _parameter = parameter;
            _readValueFunc = readValueFunc;
            _writeValueFunc = writeValueFunc;
        }
    }
}
#endif
