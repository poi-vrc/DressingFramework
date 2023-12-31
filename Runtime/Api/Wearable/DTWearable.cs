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

using Chocopoi.DressingFramework;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingTools.Api.Cabinet;
using Newtonsoft.Json;
using UnityEngine;

namespace Chocopoi.DressingTools.Api.Wearable
{
    /// <summary>
    /// DressingTools wearable component. Used for storing wearable configuration.
    /// </summary>
    public class DTWearable : DKBaseComponent, IWearable
    {
        public GameObject WearableGameObject { get => rootGameObject != null ? rootGameObject : gameObject; set => rootGameObject = value; }
        public string ConfigJson { get => configJson; set => configJson = value; }

        /// <summary>
        /// Root GameObject. Set to `null` to use the GameObject holding this component.
        /// </summary>
        public GameObject rootGameObject;

        /// <summary>
        /// Config JSON. This is the same as the property `ConfigJson`.
        /// </summary>
        public string configJson;

        public DTWearable()
        {
            rootGameObject = null;
            configJson = JsonConvert.SerializeObject(new WearableConfig());
        }

        public DTCabinet FindCabinetComponent()
        {
            var p = transform.parent;
            DTCabinet cabinet = null;
            while (p != null)
            {
                if (p.TryGetComponent(out cabinet))
                {
                    break;
                }
                p = p.parent;
            }
            return cabinet;
        }
    }
}
