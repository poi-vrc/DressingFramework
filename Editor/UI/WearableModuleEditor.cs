/*
 * File: WearableModuleEditor.cs
 * Project: DressingFramework
 * Created Date: Tuesday, August 1st 2023, 12:37:10 am
 * Author: chocopoi (poi@chocopoi.com)
 * -----
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

using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Serialization;
using UnityEditor;

namespace Chocopoi.DressingFramework.UI
{
    /// <summary>
    /// Wearable module editor base class. (This API will be changed soon to use ElementViewBase)
    /// </summary>
    public class WearableModuleEditor : IMGUIViewBase
    {
        /// <summary>
        /// Human-readable friendly name of this module editor
        /// </summary>
        public virtual string FriendlyName => provider.FriendlyName;

        /// <summary>
        /// Used internally. A temporary status for the UI to store the foldout state.
        /// </summary>
        public bool foldout;

        /// <summary>
        /// Parent view
        /// </summary>
        protected IWearableModuleEditorViewParent parentView;

        /// <summary>
        /// Module provider
        /// </summary>
        protected WearableModuleProviderBase provider;

        /// <summary>
        /// Target module
        /// </summary>
        protected IModuleConfig target;

        /// <summary>
        /// Initialize a module editor
        /// </summary>
        /// <param name="parentView">Parent view</param>
        /// <param name="provider">Module provider</param>
        /// <param name="target">Target module</param>
        public WearableModuleEditor(IWearableModuleEditorViewParent parentView, WearableModuleProviderBase provider, IModuleConfig target)
        {
            this.parentView = parentView;
            this.provider = provider;
            this.target = target;
        }

        public override void OnGUI()
        {
            // TODO: default module editor?
            HelpBox("No editor available for this module.", MessageType.Error);
        }

        /// <summary>
        /// Check if the module editor content is valid
        /// </summary>
        /// <returns>Is valid</returns>
        public virtual bool IsValid()
        {
            return true;
        }
    }
}
