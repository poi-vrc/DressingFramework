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

using System;
using System.Collections.Generic;
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Dynamics;
using Chocopoi.DressingFramework.Logging;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// Context of the current avatar build
    /// </summary>
    public abstract class Context : IContext
    {
        public GameObject AvatarGameObject { get; private set; }

        public abstract object RuntimeContext { get; }
        public abstract Report Report { get; }

        private readonly Dictionary<Type, ContextFeature> _contextFeatures;
        private readonly Dictionary<Type, IExtraContext> _extraContexts;
        private readonly string _randomString;

        internal Context(GameObject avatarGameObject)
        {
            AvatarGameObject = avatarGameObject;

            _extraContexts = new Dictionary<Type, IExtraContext>();
            _contextFeatures = new Dictionary<Type, ContextFeature>();
            _randomString = DKEditorUtils.RandomString(8);

            AddContextFeature(new DynamicsStore(avatarGameObject));
            AddContextFeature(new PathRemapper(avatarGameObject));
            AddContextFeature(new AnimatorParameters());
        }

        protected void AddContextFeature(ContextFeature feature)
        {
            _contextFeatures.Add(feature.GetType(), feature);
        }

        public ContextFeature Feature(Type type)
        {
            foreach (var kvp in _contextFeatures)
            {
                var featureType = kvp.Key;
                if (featureType == type || featureType.IsSubclassOf(type))
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        public T Feature<T>() where T : ContextFeature
        {
            return (T)Feature(typeof(T));
        }

        public abstract void CreateUniqueAsset(UnityEngine.Object obj, string name);

        public string MakeUniqueName(string name)
        {
            return $"{AvatarGameObject.name}_{_randomString}_{name}";
        }

        public T Extra<T>() where T : IExtraContext
        {
            return (T)Extra(typeof(T));
        }

        public IExtraContext Extra(Type type)
        {
            if (!_extraContexts.TryGetValue(type, out var extraContext))
            {
                extraContext = _extraContexts[type] = (IExtraContext)Activator.CreateInstance(type);
                extraContext.OnEnable(this);
            }
            return extraContext;
        }

        internal virtual void OnEnable()
        {
            foreach (var feature in _contextFeatures.Values)
            {
                feature.OnEnable();
            }
        }

        internal virtual void OnDisable()
        {
            foreach (var feature in _contextFeatures.Values)
            {
                feature.OnDisable();
            }
        }
    }
}
