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
using UnityEngine;

namespace Chocopoi.DressingFramework.Animations
{
    /// <summary>
    /// A container class that holds an original clip and a new clip to be written.
    /// </summary>
    public class AnimationClipContainer
    {
        /// <summary>
        /// Original clip, you should not modify this clip at all. Modify `newClip` instead.
        /// </summary>
        public AnimationClip originalClip;

        /// <summary>
        /// New clip to be written. If not null, this clip will be written using the dispatch function
        /// </summary>
        public AnimationClip newClip;

        /// <summary>
        /// Used internally and should only be invoked by the store. This function dispatches the `newClip` changes to the `originalClip` container.
        /// </summary>
        public Action<AnimationClip> dispatchFunc;

        /// <summary>
        /// Constructs a container
        /// </summary>
        public AnimationClipContainer()
        {
            originalClip = null;
            newClip = null;
            dispatchFunc = null;
        }
    }

    /// <summary>
    /// Animation store interface
    /// </summary>
    public interface IAnimationStore
    {
        /// <summary>
        /// Invoked on write
        /// </summary>
        event Action Write;

        /// <summary>
        /// Clips in the store
        /// </summary>
        List<AnimationClipContainer> Clips { get; }

        /// <summary>
        /// Dispatch all changes
        /// </summary>
        void Dispatch();

        /// <summary>
        /// Register a motion and its children recursively.
        /// </summary>
        /// <param name="motion">Motion to register</param>
        /// <param name="dispatchFunc">Dispatch function to the upper container</param>
        /// <param name="filterMotionFunc">Optional filter motion function</param>
        /// <param name="visitedMotions">Optional visited motion set</param>
        void RegisterMotion(Motion motion, Action<AnimationClip> dispatchFunc, Func<Motion, bool> filterMotionFunc = null, HashSet<Motion> visitedMotions = null);

        /// <summary>
        /// Register a single clip.
        /// </summary>
        /// <param name="clip">Clip to register</param>
        /// <param name="dispatchFunc">Dispatch function to the upper container</param>
        void RegisterClip(AnimationClip clip, Action<AnimationClip> dispatchFunc);
    }
}
