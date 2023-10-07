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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingFramework.Wearable.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// DK Runtime Utilities
    /// </summary>
    internal static class DKRuntimeUtils
    {
        public enum LifecycleStage
        {
            Awake,
            Start
        }

        public delegate void OnCabinetLifecycleDelegate(LifecycleStage stage, ICabinet cabinet);
        public static OnCabinetLifecycleDelegate OnCabinetLifecycle = (stage, cabinet) => { };
    }
}
