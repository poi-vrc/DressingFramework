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

using Newtonsoft.Json;
using UnityEngine;

namespace Chocopoi.DressingFramework.Cabinet
{
    /// <summary>
    /// DressingTools cabinet component. Used for storing cabinet settings and apply trigger.
    /// </summary>
    [AddComponentMenu("DressingTools/DT Cabinet")]
    [DefaultExecutionOrder(-19999)]
    [ExecuteInEditMode]
    public class DTCabinet : DKBaseComponent, ICabinet
    {
        public GameObject AvatarGameObject { get => rootGameObject != null ? rootGameObject : gameObject; set => rootGameObject = value; }
        public string ConfigJson { get => configJson; set => configJson = value; }

        /// <summary>
        /// Root GameObject. Set to `null` to use the GameObject holding this component.
        /// </summary>
        public GameObject rootGameObject;

        /// <summary>
        /// Config JSON. This is the same as the property `ConfigJson`.
        /// </summary>
        public string configJson;

        public DTCabinet()
        {
            rootGameObject = null;
            configJson = JsonConvert.SerializeObject(new CabinetConfig());
        }

        public void Awake()
        {
            OnLifecycle(DKRuntimeUtils.LifecycleStage.Awake);
        }

        public void Start()
        {
            OnLifecycle(DKRuntimeUtils.LifecycleStage.Start);
        }

        private void OnLifecycle(DKRuntimeUtils.LifecycleStage lifecycleStage)
        {
#if UNITY_EDITOR
            var playing = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            var playing = true;
#endif
            // ensure we are in play mode and not destroyed
            if (playing && this != null)
            {
                DKRuntimeUtils.OnCabinetLifecycle(lifecycleStage, this);
            }
        }
    }
}
