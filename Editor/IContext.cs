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

namespace Chocopoi.DressingFramework
{
    internal interface IContext
    {
        /// <summary>
        /// Runtime-level context
        /// </summary>
        object RuntimeContext { get; }

        /// <summary>
        /// Apply report
        /// </summary>
        // Report Report { get; }

        /// <summary>
        /// Avatar GameObject
        /// </summary>
        GameObject AvatarGameObject { get; }

        /// <summary>
        /// Obtains a context feature.
        /// </summary>
        /// <param name="type">Type of the feature</param>
        /// <returns>Context feature. Null if such feature is not available.</returns>
        ContextFeature Feature(System.Type type);

        /// <summary>
        /// Obtains a context feature.
        /// </summary>
        /// <typeparam name="T">Type of the feature</typeparam>
        /// <returns>Context feature. Null if such feature is not available.</returns>
        T Feature<T>() where T : ContextFeature;

        /// <summary>
        /// Make an asset with the specified name unique among other assets in the same avatar
        /// </summary>
        /// <param name="obj">Asset to save</param>
        /// <param name="name">Name</param>
        void CreateUniqueAsset(Object obj, string name);

        /// <summary>
        /// Make an asset with the specified name
        /// </summary>
        /// <param name="obj">Asset to save</param>
        /// <param name="name">Name</param>
        void CreateAsset(Object obj, string name);


        /// <summary>
        /// Obtain an extra context
        /// </summary>
        /// <typeparam name="T">Extra context type</typeparam>
        /// <returns>Extra context</returns>
        T Extra<T>() where T : IExtraContext;

        /// <summary>
        /// Obtain an extra context using a type argument
        /// </summary>
        /// <param name="type">Extra context type</param>
        /// <returns>Extra context</returns>
        IExtraContext Extra(System.Type type);
    }
}
