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

using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Logging;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.DK
{
    /// <summary>
    /// DK native implementation context
    /// </summary>
    internal class DKNativeContext : Context
    {
        /// <summary>
        /// Folder of generated assets
        /// </summary>
        public const string GeneratedAssetsFolderName = "_DTGeneratedAssets";
        /// <summary>
        /// Full path of generated assets folder
        /// </summary>
        public const string GeneratedAssetsPath = "Assets/" + GeneratedAssetsFolderName;

        public override object RuntimeContext => null;
        internal override Report Report => _report;
        public override Object AssetContainer => _assetContainer;

        private readonly DKReport _report;
        private readonly DKNativeAssetContainer _assetContainer;

        public DKNativeContext(GameObject avatarGameObject) : base(avatarGameObject)
        {
            _report = new DKReport();
            _assetContainer = ScriptableObject.CreateInstance<DKNativeAssetContainer>();
            AssetDatabase.CreateAsset(_assetContainer, $"{GeneratedAssetsPath}/{AvatarGameObject.name}_{DKEditorUtils.RandomString(8)}.asset");

            AddContextFeature(new AnimationStore(this));
            AddContextFeature(new DKMenuStore(this));
        }

        public override void CreateAsset(Object obj, string name)
        {
            obj.name = name;
            AssetDatabase.AddObjectToAsset(obj, _assetContainer);
        }
    }
}
