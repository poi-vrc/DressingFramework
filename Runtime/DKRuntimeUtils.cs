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
using System.Linq;
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingFramework.Wearable.Modules;
using Newtonsoft.Json;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// DK Runtime Utilities
    /// </summary>
    internal static class DKRuntimeUtils
    {
        private static readonly System.Random Random = new System.Random();

        public enum LifecycleStage
        {
            Awake,
            Start
        }

        public delegate void OnCabinetLifecycleDelegate(LifecycleStage stage, ICabinet cabinet);
        public static OnCabinetLifecycleDelegate OnCabinetLifecycle = (stage, cabinet) => { };

        public static string RandomString(int length)
        {
            // i just copied from stackoverflow :D
            // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings?page=1&tab=scoredesc#tab-top
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
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

        public static ICabinet GetAvatarCabinet(GameObject avatarGameObject, bool createIfNotExists = false)
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

        public static List<CabinetModule> GetCabinetModulesByName(CabinetConfig config, string moduleName)
        {
            var list = new List<CabinetModule>();
            foreach (var module in config.modules)
            {
                if (module.moduleName == moduleName)
                {
                    list.Add(module);
                }
            }
            return list;
        }

        public static List<WearableModule> GetWearableModulesByName(WearableConfig config, string moduleName)
        {
            var list = new List<WearableModule>();
            foreach (var module in config.modules)
            {
                if (module.moduleName == moduleName)
                {
                    list.Add(module);
                }
            }
            return list;
        }
    }
}
