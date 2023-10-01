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

namespace Chocopoi.DressingFramework.Animations
{
    /// <summary>
    /// Path remapper interface
    /// </summary>
    public interface IPathRemapper
    {
        /// <summary>
        /// Invalidate mappings cache
        /// </summary>
        void InvalidateCache();

        /// <summary>
        /// Tag GameObject as container bone.
        /// </summary>
        /// <param name="gameObject"></param>
        void TagContainerBone(GameObject gameObject);

        /// <summary>
        /// Remap the provided path if the GameObject was moved. Otherwise, the output will be the original path.
        /// </summary>
        /// <param name="originalPath">Path to remap</param>
        /// <param name="avoidContainerBones">Avoid tagged container bones</param>
        /// <returns>Remapped path</returns>
        string Remap(string originalPath, bool avoidContainerBones = false);
    }
}
