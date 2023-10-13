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

namespace Chocopoi.DressingFramework.Animations
{
    /// <summary>
    /// Path remapper
    /// </summary>
    public class PathRemapper
    {
        private Dictionary<GameObject, List<string>> _originalPaths;
        private bool _invalidMappingsCache;
        private Dictionary<string, string> _mappingsCache;
        private bool _invalidNonContainerBoneMappingsCache;
        private Dictionary<string, string> _nonContainerBoneMappingsCache;
        private List<GameObject> _taggedContainerBones;
        private GameObject _avatarRoot;

        /// <summary>
        /// Constructs a new path remapper, and scans the specified avatar root to obtain all original paths.
        /// </summary>
        /// <param name="avatarRoot">Avatar root object</param>
        public PathRemapper(GameObject avatarRoot)
        {
            _avatarRoot = avatarRoot;
            _originalPaths = new Dictionary<GameObject, List<string>>();
            _invalidMappingsCache = true;
            _invalidNonContainerBoneMappingsCache = true;
            _mappingsCache = new Dictionary<string, string>();
            _nonContainerBoneMappingsCache = new Dictionary<string, string>();
            _taggedContainerBones = new List<GameObject>();

            GenerateAllOriginalPaths(avatarRoot);
        }

        private void GenerateAllOriginalPaths(GameObject root)
        {
            _originalPaths.Clear();
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var trans in transforms)
            {
                if (trans == root.transform) continue;
                var path = DKEditorUtils.GetRelativePath(trans, root.transform);
                _originalPaths[trans.gameObject] = new List<string>() {
                    path
                };
            }
        }

        /// <summary>
        /// Invalidate mappings cache
        /// </summary>
        public void InvalidateCache()
        {
            _invalidMappingsCache = true;
            _invalidNonContainerBoneMappingsCache = true;
            _mappingsCache.Clear();
            _nonContainerBoneMappingsCache.Clear();
        }

        /// <summary>
        /// Tag GameObject as container bone.
        /// </summary>
        /// <param name="gameObject"></param>
        public void TagContainerBone(GameObject gameObject)
        {
            if (!_taggedContainerBones.Contains(gameObject))
            {
                _taggedContainerBones.Add(gameObject);
                InvalidateCache();
            }
        }

        /// <summary>
        /// Remap the provided path if the GameObject was moved. Otherwise, the output will be the original path.
        /// </summary>
        /// <param name="originalPath">Path to remap</param>
        /// <param name="avoidContainerBones">Avoid tagged container bones</param>
        /// <returns>Remapped path</returns>
        public string Remap(string originalPath, bool avoidContainerBones = false)
        {
            // generate mappings or return cache
            var remappings = GenerateMappings(avoidContainerBones);

            // attempts to find a remapped path
            if (remappings.TryGetValue(originalPath, out var remappedPath))
            {
                return remappedPath;
            }

            // no remaps
            return originalPath;
        }

        private void RegeneratePaths(GameObject go, List<string> originalPaths, Dictionary<string, string> mappings, bool avoidContainerBones)
        {
            var p = go.transform;

            // traverse up until no container bones are found
            if (avoidContainerBones)
            {
                while (p != null && _taggedContainerBones.Contains(p.gameObject))
                {
                    p = p.parent;
                }

                if (p == null || _taggedContainerBones.Contains(p.gameObject))
                {
                    throw new System.Exception("Unable to generate mappings with root being tagged as container bone");
                }
            }

            // regenerate relative path
            foreach (var originalPath in originalPaths)
            {
                mappings[originalPath] = DKEditorUtils.GetRelativePath(p, _avatarRoot.transform);
            }
        }

        private Dictionary<string, string> GenerateMappings(bool avoidContainerBones)
        {
            Dictionary<string, string> mappings = avoidContainerBones ? _nonContainerBoneMappingsCache : _mappingsCache;
            bool invalidCache = avoidContainerBones ? _invalidNonContainerBoneMappingsCache : _invalidMappingsCache;

            // return cache if possible
            if (!invalidCache && mappings != null)
            {
                _invalidMappingsCache = true;
                _invalidNonContainerBoneMappingsCache = true;
                return mappings;
            }

            foreach (KeyValuePair<GameObject, List<string>> pair in _originalPaths)
            {
                RegeneratePaths(pair.Key, pair.Value, mappings, avoidContainerBones);
            }

            if (avoidContainerBones)
            {
                _invalidNonContainerBoneMappingsCache = false;
            }
            else
            {
                _invalidMappingsCache = false;
            }
            return mappings;
        }
    }
}
