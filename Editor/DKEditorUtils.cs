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
using System.IO;
using System.Linq;
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Proxy;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingTools.Api.Cabinet;
using Chocopoi.DressingTools.Api.Wearable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// DK Editor Utilities
    /// </summary>
    internal static class DKEditorUtils
    {
        private static readonly System.Random Random = new System.Random();
        private static readonly Dictionary<string, System.Type> s_reflectionTypeCache = new Dictionary<string, System.Type>();

        public static System.Type FindType(string typeName)
        {
            // try getting from cache to avoid scanning the assemblies again
            if (s_reflectionTypeCache.ContainsKey(typeName))
            {
                return s_reflectionTypeCache[typeName];
            }

            // scan from assemblies and save to cache
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    s_reflectionTypeCache[typeName] = type;
                    return type;
                }
            }

            // no such type found
            return null;
        }

        // copied from avatarlib since this is the only function DK needs from it
        // Referenced and modified from https://answers.unity.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
        public static string GetRelativePath(Transform transform, Transform untilTransform = null, string prefix = "", string suffix = "")
        {
            string path = transform.name;
            while (true)
            {
                transform = transform.parent;

                if (transform.parent == null || (untilTransform != null && transform == untilTransform))
                {
                    break;
                }

                path = transform.name + "/" + path;
            }
            return prefix + path + suffix;
        }

        public static string RandomString(int length)
        {
            // i just copied from stackoverflow :D
            // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings?page=1&tab=scoredesc#tab-top
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        // referenced from: http://answers.unity3d.com/questions/458207/copy-a-component-at-runtime.html
        public static Component CopyComponent(Component originalComponent, GameObject destGameObject)
        {
            System.Type type = originalComponent.GetType();

            // get the destination component or add new
            var destComp = destGameObject.AddComponent(type);

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(destComp, field.GetValue(originalComponent));
            }

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || prop.Name == "name")
                {
                    continue;
                }
                prop.SetValue(destComp, prop.GetValue(originalComponent, null), null);
            }

            return destComp;
        }

        public static JObject ParseJson(string json)
        {
            // Newtonsoft.JSON has a non-standard, silly design decision that modifies date-like strings by default
            // Our ISO date strings are modified by this library into normal date strings...
            // We can consider using other solutions soon. This is so silly.
            // https://github.com/JamesNK/Newtonsoft.Json/issues/862

            var reader = new JsonTextReader(new StringReader(json))
            {
                // why need to parse dates by default?
                DateParseHandling = DateParseHandling.None
            };
            var result = JObject.Load(reader);
            while (reader.Read()) { }

            return result;
        }

        public static List<IDynamicsProxy> ScanDynamics(GameObject obj, bool doNotScanContainingWearables = false)
        {
            var dynamicsList = new List<IDynamicsProxy>();

            // TODO: replace by reading YAML

            // get the dynbone type
            var DynamicBoneType = FindType("DynamicBone");
            var PhysBoneType = FindType("VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone");

            // scan dynbones
            if (DynamicBoneType != null)
            {
                var dynBones = obj.GetComponentsInChildren(DynamicBoneType, true);
                foreach (var dynBone in dynBones)
                {
                    if (doNotScanContainingWearables && IsOriginatedFromAnyWearable(obj.transform, dynBone.transform))
                    {
                        continue;
                    }
                    dynamicsList.Add(new DynamicBoneProxy(dynBone));
                }
            }

            // scan physbones
            if (PhysBoneType != null)
            {
                var physBones = obj.GetComponentsInChildren(PhysBoneType, true);
                foreach (var physBone in physBones)
                {
                    if (doNotScanContainingWearables && IsOriginatedFromAnyWearable(obj.transform, physBone.transform))
                    {
                        continue;
                    }
                    dynamicsList.Add(new PhysBoneProxy(physBone));
                }
            }

            return dynamicsList;
        }

        public static IDynamicsProxy FindDynamicsWithRoot(List<IDynamicsProxy> avatarDynamics, Transform dynamicsRoot)
        {
            foreach (var bone in avatarDynamics)
            {
                if (bone.RootTransform == dynamicsRoot)
                {
                    return bone;
                }
            }
            return null;
        }

        public static bool IsDynamicsExists(List<IDynamicsProxy> avatarDynamics, Transform dynamicsRoot)
        {
            return FindDynamicsWithRoot(avatarDynamics, dynamicsRoot) != null;
        }

        public static bool IsGrandParent(Transform grandParent, Transform grandChild)
        {
            var p = grandChild.parent;
            while (p != null)
            {
                if (p == grandParent)
                {
                    return true;
                }
                p = p.parent;
            }
            return false;
        }

        public static DTWearable[] GetAllSceneWearables()
        {
            // TODO: check for IWearable instead sticking to DT
            return Object.FindObjectsOfType<DTWearable>();
        }

        public static IWearable GetCabinetWearable(GameObject wearableGameObject)
        {
            if (wearableGameObject == null)
            {
                return null;
            }

            // loop through all scene wearables and search
            var wearables = GetAllSceneWearables();

            // no matter there are two occurance or not, we return the first found
            foreach (var sceneWearable in wearables)
            {
                if (sceneWearable.WearableGameObject == wearableGameObject)
                {
                    return sceneWearable;
                }
            }

            return null;
        }

        public static IWearable[] GetCabinetWearables(GameObject avatarGameObject)
        {
            if (avatarGameObject == null)
            {
                return new IWearable[0];
            }
            return avatarGameObject.GetComponentsInChildren<IWearable>();
        }

        public static DTCabinet[] GetAllCabinets()
        {
            // TODO: check for ICabinet instead sticking to DT
            return Object.FindObjectsOfType<DTCabinet>();
        }

        public static DTCabinet GetAvatarCabinet(GameObject avatarGameObject, bool createIfNotExists = false)
        {
            if (avatarGameObject == null)
            {
                return null;
            }

            // loop through all cabinets and search
            var cabinets = GetAllCabinets();

            // no matter there are two occurance or not, we return the first found
            foreach (var cabinet in cabinets)
            {
                if (cabinet.AvatarGameObject == avatarGameObject)
                {
                    return cabinet;
                }
            }

            if (createIfNotExists)
            {
                // create new cabinet if not exist
                var comp = avatarGameObject.AddComponent<DTCabinet>();

                // TODO: read default config, scan for armature names?
                comp.AvatarGameObject = avatarGameObject;
                var config = new CabinetConfig();
                comp.configJson = JsonConvert.SerializeObject(config);

                return comp;
            }

            return null;
        }

        public static bool IsOriginatedFromAnyWearable(Transform root, Transform transform)
        {
            var found = false;
            while (transform != null)
            {
                transform = transform.parent;
                if (transform == root || transform == null)
                {
                    break;
                }

                if (transform.TryGetComponent<DTWearable>(out var _))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}
