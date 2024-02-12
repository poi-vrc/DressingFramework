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
using UnityEditor.Animations;
using UnityEngine;

namespace Chocopoi.DressingFramework.Animations
{
    /// <summary>
    /// Animation store
    /// 
    /// Warning: This API is subject to change.
    /// </summary>
    public class AnimationStore : ContextFeature
    {
        /// <summary>
        /// Invoked on write
        /// </summary>
        public event Action Write;

        /// <summary>
        /// Clips in the store
        /// </summary>
        public List<AnimationClipContainer> Clips { get; private set; }

        private Context _ctx;

        /// <summary>
        /// Constructs a new animation store
        /// </summary>
        /// <param name="cabCtx">Cabinet context used for creating animation clip copies</param>
        public AnimationStore(Context ctx)
        {
            _ctx = ctx;
            Clips = new List<AnimationClipContainer>();
        }

        /// <summary>
        /// Dispatch all changes
        /// </summary>
        public void Dispatch()
        {
            foreach (var clip in Clips)
            {
                if (clip.newClip != null && clip.originalClip != clip.newClip)
                {
                    _ctx.CreateUniqueAsset(clip.newClip, $"Clip_{clip.newClip.name}_{DKEditorUtils.RandomString(6)}.anim");
                    clip.dispatchFunc?.Invoke(clip.newClip);
                }
            }
            Write?.Invoke();
        }

        /// <summary>
        /// Register a motion and its children recursively.
        /// </summary>
        /// <param name="motion">Motion to register</param>
        /// <param name="dispatchFunc">Dispatch function to the upper container</param>
        /// <param name="filterMotionFunc">Optional filter motion function</param>
        /// <param name="visitedMotions">Optional visited motion set</param>
        public void RegisterMotion(Motion motion, Action<AnimationClip> dispatchFunc, Func<Motion, bool> filterMotionFunc = null, HashSet<Motion> visitedMotions = null)
        {
            if (visitedMotions == null)
            {
                visitedMotions = new HashSet<Motion>();
            }

            if (motion == null || (filterMotionFunc != null && !filterMotionFunc(motion)))
            {
                return;
            }

            // avoid visited motions
            if (visitedMotions.Contains(motion))
            {
                return;
            }
            visitedMotions.Add(motion);

            if (motion is AnimationClip clip)
            {
                RegisterClip(clip, dispatchFunc);
            }
            else if (motion is BlendTree tree)
            {
                for (var i = 0; i < tree.children.Length; i++)
                {
                    var childIndex = i;
                    RegisterMotion(tree.children[childIndex].motion, (AnimationClip newClip) => tree.children[childIndex].motion = newClip, filterMotionFunc, visitedMotions);
                }
            }
        }

        /// <summary>
        /// Register a single clip.
        /// </summary>
        /// <param name="clip">Clip to register</param>
        /// <param name="dispatchFunc">Dispatch function to the upper container</param>
        public void RegisterClip(AnimationClip clip, Action<AnimationClip> dispatchFunc)
        {
            Clips.Add(new AnimationClipContainer()
            {
                originalClip = clip,
                dispatchFunc = dispatchFunc
            });
        }

        internal override void OnEnable() { }

        internal override void OnDisable() { }
    }
}
