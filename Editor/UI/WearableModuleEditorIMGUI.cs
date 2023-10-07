/*
 * File: WearableModuleEditorIMGUI.cs
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

namespace Chocopoi.DressingFramework.UI
{
    /// <summary>
    /// Wearable module editor base class for IMGUI
    /// </summary>
    public abstract class WearableModuleEditorIMGUI : IMGUIViewBase, IWearableModuleEditor
    {
        /// <summary>
        /// Human-readable friendly name of this module editor
        /// </summary>
        public virtual string FriendlyName => Provider.FriendlyName;

        /// <summary>
        /// Used internally. A temporary status for the UI to store the foldout state.
        /// </summary>
        public bool FoldoutState { get; set; }

        /// <summary>
        /// Parent view
        /// </summary>
        public IWearableModuleEditorViewParent ParentView { get; set; }

        /// <summary>
        /// Module provider
        /// </summary>
        public WearableModuleProviderBase Provider { get; set; }

        /// <summary>
        /// Target module
        /// </summary>
        public IModuleConfig Target { get; set; }

        /// <summary>
        /// Initialize a module editor
        /// </summary>
        /// <param name="parentView">Parent view</param>
        /// <param name="provider">Module provider</param>
        /// <param name="target">Target module</param>
        public WearableModuleEditorIMGUI(IWearableModuleEditorViewParent parentView, WearableModuleProviderBase provider, IModuleConfig target)
        {
            ParentView = parentView;
            Provider = provider;
            Target = target;
        }

        /// <summary>
        /// Check if the module editor content is valid
        /// </summary>
        /// <returns>Is valid</returns>
        public virtual bool IsValid() => true;
    }
}
