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

using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.DK.Triggers.PlayMode
{
    /// <summary>
    /// This will be automatically added to found avatars when entering play mode by PlayModeBuildTriggerSpawner.
    /// And then AvatarBuilder will be initialized for build from AutoPlayModeBuild from Awake or Start depending which state Unity is in.
    /// 
    /// This mechanism is referenced from MA/NDMF with modifications.
    /// Copyright (c) 2022 bd_ under MIT License
    /// </summary>
    [AddComponentMenu("")]
    [DefaultExecutionOrder(-19998)]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    internal class PlayModeBuildTrigger : MonoBehaviour
    {
        private void Awake()
        {
            OnLifecycle(PlayModeBuildTriggerSpawner.Lifecycle.Awake);
        }

        private void Start()
        {
            OnLifecycle(PlayModeBuildTriggerSpawner.Lifecycle.Start);
        }

        private void Update()
        {
            DestroyImmediate(this);
        }

        private void OnLifecycle(PlayModeBuildTriggerSpawner.Lifecycle lifecycle)
        {
            // ensure we are in play mode and not destroyed
            if (DKRuntimeUtils.IsPlaying && this != null)
            {
                PlayModeBuildTriggerSpawner.OnAvatarLifecycle(lifecycle, gameObject);
            }
        }
    }
}
