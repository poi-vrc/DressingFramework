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
using System.IO;
using System.Linq;
using Chocopoi.DressingFramework.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
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
            if (destGameObject.scene == null)
            {
                throw new Exception("Report this to the DressingTools developer! Destination GameObject does not contain a scene!");
            }
            if (PrefabUtility.IsPartOfAnyPrefab(destGameObject))
            {
                throw new Exception("Report this to the DressingTools developer! Destination GameObject is part of a prefab!");
            }
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

        public static bool TrySafeRun(string label, Report report, Action func)
        {
            try
            {
                func();
            }
            catch (Exception ex)
            {
                report.LogException(label, ex);
                return false;
            }
            return true;
        }
    }
}
