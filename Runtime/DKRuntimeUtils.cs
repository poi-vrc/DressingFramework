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
using Chocopoi.DressingFramework.Components.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// DK Runtime Utilities
    /// </summary>
    internal static class DKRuntimeUtils
    {
        /// <summary>
        /// The isPlayingOrWillChangePlaymode property can only be accessed within Unity Editor.
        /// When running a build, this will fail with a compilation error. This provides a way to hardcode true in those cases.
        /// </summary>
#if UNITY_EDITOR
        public static bool IsPlaying => UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
        public static bool IsPlaying => true;
#endif

        private static void FindComponentsAndAddToListIfNotExist<T>(List<GameObject> list, GameObject rootObj) where T : Component
        {
            var comps = rootObj.GetComponentsInChildren<T>();
            foreach (var comp in comps)
            {
                if (!list.Contains(comp.gameObject)) list.Add(comp.gameObject);
            }
        }

        public static List<GameObject> FindSceneAvatars(Scene scene)
        {
            var list = new List<GameObject>();
            var rootObjs = scene.GetRootGameObjects();

            // iterate through all root objects to find the root
            foreach (var rootObj in rootObjs)
            {
#if DK_VRCSDK3A
                // find VRC-specific avatars
                FindComponentsAndAddToListIfNotExist<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(list, rootObj);
#endif

                // generic avatar root components
                FindComponentsAndAddToListIfNotExist<DKAvatarRoot>(list, rootObj);
            }

            return list;
        }
    }
}
