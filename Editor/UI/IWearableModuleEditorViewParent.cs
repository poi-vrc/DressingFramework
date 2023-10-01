﻿/*
 * File: IModuleEditorViewParent.cs
 * Project: DressingFramework
 * Created Date: Saturday, August 5th 2023, 11:44:30 am
 * Author: chocopoi (poi@chocopoi.com)
 * -----
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

namespace Chocopoi.DressingFramework.UI
{
    /// <summary>
    /// Wearable module editor view parent interface. This defines what events, properties and methods that a module editor parent should have.
    /// </summary>
    public interface IWearableModuleEditorViewParent : IEditorView
    {
        /// <summary>
        /// Target avatar or wearable change event
        /// </summary>
        event Action TargetAvatarOrWearableChange;

        /// <summary>
        /// Target avatar
        /// </summary>
        GameObject TargetAvatar { get; }

        /// <summary>
        /// Target wearable
        /// </summary>
        GameObject TargetWearable { get; }

        /// <summary>
        /// Trigger update avatar preview
        /// </summary>
        void UpdateAvatarPreview();
    }
}
