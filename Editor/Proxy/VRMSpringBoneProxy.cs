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
using UnityEngine;

namespace Chocopoi.DressingFramework.Proxy
{
    internal class VRMSpringBoneProxy : IDynamicsProxy
    {
        public static readonly System.Type VRMSpringBoneType = DKEditorUtils.FindType("VRM.VRMSpringBone");

        private Transform _rootTransform;

        public VRMSpringBoneProxy(Component component, Transform rootTransform)
        {
            Component = component;
            _rootTransform = rootTransform;
            if (VRMSpringBoneType == null)
            {
                throw new System.Exception("No VRMSpringBone component is found in this project. It is required to process VRMSpringBone-based dynamics.");
            }
        }

        public static List<Transform> GetRootBones(Component component)
        {
            return (List<Transform>)VRMSpringBoneType.GetField("RootBones").GetValue(component);
        }

        public Component Component { get; set; }

        public Transform Transform
        {
            get { return Component.transform; }
        }

        public GameObject GameObject
        {
            get { return Component.gameObject; }
        }

        public Transform RootTransform
        {
            get { return _rootTransform; }
            set
            {
                var rootBones = GetRootBones(Component);
                if (!rootBones.Remove(_rootTransform))
                {
                    Debug.LogWarning("[DressingFramework] The VRM Spring Bone does not have the original root transform. Unable to remove.");
                }
                rootBones.Add(value);
                _rootTransform = value;
            }
        }

        public List<Transform> IgnoreTransforms
        {
            get
            {
                Debug.LogWarning("[DressingFramework] VRM Spring Bone does not support IgnoreTransform, but this API was called!");
                return new List<Transform>();
            }
            set { Debug.LogWarning("[DressingFramework] VRM Spring Bone does not support IgnoreTransform, but this API was called!"); }
        }
    }
}
