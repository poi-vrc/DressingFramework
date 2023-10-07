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
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Proxy;
using Chocopoi.DressingFramework.Wearable;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Context
{
    /// <summary>
    /// Extra cabinet context
    /// </summary>
    public class ExtraCabinetContext { }

    /// <summary>
    /// Extra wearable context
    /// </summary>
    public class ExtraWearableContext { }

    /// <summary>
    /// Apply cabinet context
    /// </summary>
    public class ApplyCabinetContext
    {
        /// <summary>
        /// Folder of generated assets
        /// </summary>
        public const string GeneratedAssetsFolderName = "_DTGeneratedAssets";
        /// <summary>
        /// Full path of generated assets folder
        /// </summary>
        public const string GeneratedAssetsPath = "Assets/" + GeneratedAssetsFolderName;

        private const string AssetNamePrefix = "cpDT_";

        private readonly Dictionary<System.Type, ExtraCabinetContext> _extraContexts;

        /// <summary>
        /// Apply cabinet report
        /// </summary>
        public DKReport report;

        /// <summary>
        /// Cabinet config
        /// </summary>
        public CabinetConfig cabinetConfig;

        /// <summary>
        /// Avatar GameObject
        /// </summary>
        public GameObject avatarGameObject;

        /// <summary>
        /// All completed wearable contexts
        /// </summary>
        public Dictionary<IWearable, ApplyWearableContext> wearableContexts;

        /// <summary>
        /// Scanned avatar dynamics
        /// </summary>
        public List<IDynamicsProxy> avatarDynamics;

        /// <summary>
        /// Path remapper
        /// </summary>
        public PathRemapper pathRemapper;

        /// <summary>
        /// Animation store
        /// </summary>
        public AnimationStore animationStore;

        private readonly string _randomString;

        /// <summary>
        /// Initialize a new apply cabinet context
        /// </summary>
        public ApplyCabinetContext()
        {
            report = null;
            cabinetConfig = null;
            avatarGameObject = null;
            wearableContexts = new Dictionary<IWearable, ApplyWearableContext>();
            _extraContexts = new Dictionary<System.Type, ExtraCabinetContext>();
            _randomString = DKEditorUtils.RandomString(8);
        }

        /// <summary>
        /// Make the specified name unique to this cabinet avatar.
        /// Note: Only guarantee uniqueness of the same name against other cabinet avatars, not within the same avatar.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Unique name</returns>
        public string MakeUniqueName(string name)
        {
            return $"{avatarGameObject.name}_{_randomString}_{name}";
        }

        /// <summary>
        /// Make the specified name unique to this cabinet avatar with asset path
        /// Note: Only guarantee uniqueness of the same name against other cabinet avatars, not within the same avatar.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Unique name asset path</returns>
        public string MakeUniqueAssetPath(string name)
        {
            return $"{GeneratedAssetsPath}/{AssetNamePrefix}{MakeUniqueName(name)}";
        }

        /// <summary>
        /// Make an asset with the specified name unique to this cabinet avatar
        /// Note: Only guarantee uniqueness of the same name against other cabinet avatars, not within the same avatar.
        /// </summary>
        /// <param name="obj">Asset to save</param>
        /// <param name="name">Name</param>
        public void CreateUniqueAsset(Object obj, string name)
        {
            AssetDatabase.CreateAsset(obj, MakeUniqueAssetPath(name));
        }

        /// <summary>
        /// Obtain an extra context
        /// </summary>
        /// <typeparam name="T">Extra context type</typeparam>
        /// <returns>Extra context</returns>
        public T Extra<T>() where T : ExtraCabinetContext
        {
            return (T)Extra(typeof(T));
        }

        /// <summary>
        /// Obtain an extra context using a type argument
        /// </summary>
        /// <param name="type">Extra context type</param>
        /// <returns>Extra context</returns>
        public ExtraCabinetContext Extra(System.Type type)
        {
            if (!_extraContexts.TryGetValue(type, out var extraContext))
            {
                extraContext = _extraContexts[type] = (ExtraCabinetContext)System.Activator.CreateInstance(type);
            }
            return extraContext;
        }
    }

    /// <summary>
    /// Apply wearable context
    /// </summary>
    public class ApplyWearableContext
    {
        /// <summary>
        /// Wearable config
        /// </summary>
        public WearableConfig wearableConfig;

        /// <summary>
        /// Wearable GameObject
        /// </summary>
        public GameObject wearableGameObject;

        /// <summary>
        /// Scanned wearable dynamics
        /// </summary>
        public List<IDynamicsProxy> wearableDynamics;

        private readonly string _randomString;

        private readonly Dictionary<System.Type, ExtraWearableContext> _extraContexts;

        /// <summary>
        /// Initialize a new wearable context
        /// </summary>
        public ApplyWearableContext()
        {
            _extraContexts = new Dictionary<System.Type, ExtraWearableContext>();
            _randomString = DKEditorUtils.RandomString(8);
        }

        /// <summary>
        /// Make the specified name unique to this cabinet wearable.
        /// Note: Only guarantee uniqueness of the same name against other wearables, not within the same wearable.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Unique name</returns>
        public string MakeUniqueName(string name)
        {
            return $"{wearableGameObject.name}_{_randomString}_{name}";
        }

        /// <summary>
        /// Make the specified name unique to this cabinet avatar and wearable with asset path
        /// Note: Only guarantee uniqueness of the same name against other wearables, not within the same wearable.
        /// </summary>
        /// <param name="cabCtx">Apply cabinet context</param>
        /// <param name="name">Name</param>
        /// <returns>Unique name</returns>
        public string MakeUniqueAssetPath(ApplyCabinetContext cabCtx, string name)
        {
            return cabCtx.MakeUniqueAssetPath(MakeUniqueName(name));
        }

        /// <summary>
        /// Make an asset with the specified name unique to this cabinet avatar and wearable
        /// Note: Only guarantee uniqueness of the same name against other wearables, not within the same wearable.
        /// </summary>
        /// <param name="cabCtx">Apply cabinet context</param>
        /// <param name="obj">Asset to save</param>
        /// <param name="name">Name</param>
        public void CreateUniqueAsset(ApplyCabinetContext cabCtx, Object obj, string name)
        {
            cabCtx.CreateUniqueAsset(obj, MakeUniqueName(name));
        }

        /// <summary>
        /// Obtain an extra context
        /// </summary>
        /// <typeparam name="T">Extra context type</typeparam>
        /// <returns>Extra context</returns>
        public T Extra<T>() where T : ExtraWearableContext
        {
            return (T)Extra(typeof(T));
        }

        /// <summary>
        /// Obtain an extra context using a type argument
        /// </summary>
        /// <param name="type">Extra context type</param>
        /// <returns>Extra context</returns>
        public ExtraWearableContext Extra(System.Type type)
        {
            if (!_extraContexts.TryGetValue(type, out var extraContext))
            {
                extraContext = _extraContexts[type] = (ExtraWearableContext)System.Activator.CreateInstance(type);
            }
            return extraContext;
        }
    }
}
