/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

#if DK_VRCSDK3A
using System.Collections.Generic;
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Animations.VRChat;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Chocopoi.DressingFramework.Detail.DK.Passes.VRChat
{
    internal class ScanVRCAnimationsPass : BuildPass
    {
        public override BuildConstraint Constraint =>
            InvokeAtStage(BuildStage.Preparation)
                .AfterPass(typeof(CloneVRCAnimLayersPass))
                .Build();

        private void ScanAnimLayers(Context ctx, VRCAvatarDescriptor.CustomAnimLayer[] animLayers)
        {
            var store = ctx.Feature<AnimationStore>();

            foreach (var animLayer in animLayers)
            {
                if (animLayer.isDefault || animLayer.animatorController == null || !(animLayer.animatorController is AnimatorController))
                {
                    continue;
                }

                var controller = (AnimatorController)animLayer.animatorController;
                var visitedMotions = new HashSet<Motion>();

                foreach (var layer in controller.layers)
                {
                    var stack = new Stack<AnimatorStateMachine>();
                    stack.Push(layer.stateMachine);

                    while (stack.Count > 0)
                    {
                        var stateMachine = stack.Pop();

                        foreach (var state in stateMachine.states)
                        {
                            store.RegisterMotion(state.state.motion, (AnimationClip clip) => state.state.motion = clip, (Motion m) => !VRCAnimUtils.IsProxyAnimation(m), visitedMotions);
                        }

                        foreach (var childStateMachine in stateMachine.stateMachines)
                        {
                            stack.Push(childStateMachine.stateMachine);
                        }
                    }
                }
            }
        }

        public override bool Invoke(Context ctx)
        {
            if (!ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // not a VRC avatar
                return true;
            }

            ScanAnimLayers(ctx, avatarDesc.baseAnimationLayers);
            ScanAnimLayers(ctx, avatarDesc.specialAnimationLayers);

            return true;
        }
    }
}
#endif
