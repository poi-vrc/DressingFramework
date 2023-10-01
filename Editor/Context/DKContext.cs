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
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Proxy;

namespace Chocopoi.DressingFramework.Context
{
    /// <summary>
    /// DressingFramework extra cabinet context.
    /// This includes some optional extensions which require other plugins (i.e. DressingTools) to provide to use.
    /// </summary>
    public class DKCabinetContext : ExtraCabinetContext
    {
        /// <summary>
        /// Scanned avatar dynamics
        /// </summary>
        public List<IDynamicsProxy> avatarDynamics;

        /// <summary>
        /// Path remapper
        /// </summary>
        public IPathRemapper pathRemapper;

        /// <summary>
        /// Animation store
        /// </summary>
        public IAnimationStore animationStore;
    }

    /// <summary>
    /// DressingFramework extra wearable context.
    /// This includes some optional extensions which require other plugins (i.e. DressingTools) to provide to use
    /// </summary>
    public class DKWearableContext : ExtraWearableContext
    {
        /// <summary>
        /// Scanned wearable dynamics
        /// </summary>
        public List<IDynamicsProxy> wearableDynamics;
    }
}
