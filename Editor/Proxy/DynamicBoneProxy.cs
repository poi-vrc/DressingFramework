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
    internal class DynamicBoneProxy : IDynamicsProxy
    {
        public static readonly System.Type DynamicBoneType = DKEditorUtils.FindType("DynamicBone");

        public DynamicBoneProxy(Component component)
        {
            Component = component;
            if (DynamicBoneType == null)
            {
                throw new System.Exception("No DynamicBone component is found in this project. It is required to process DynamicBone-based clothes.");
            }
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
            get { return (Transform)DynamicBoneType.GetField("m_Root").GetValue(Component); }
            set { DynamicBoneType.GetField("m_Root").SetValue(Component, value); }
        }

        public List<Transform> IgnoreTransforms
        {
            get { return (List<Transform>)DynamicBoneType.GetField("m_Exclusions").GetValue(Component); }
            set { DynamicBoneType.GetField("m_Exclusions").SetValue(Component, value); }
        }
    }
}
