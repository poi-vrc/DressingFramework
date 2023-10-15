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
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Context;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests.Animations
{
    public class AnimationStoreTest : EditorTestBase
    {
        [Test]
        public void ConstructAnimationClipContainerTest()
        {
            // just for passing cov
            new AnimationClipContainer();
        }

        [Test]
        public void RegisterClipTest()
        {
            var cabCtx = new ApplyCabinetContext();
            var store = new AnimationStore(cabCtx);

            var clip1 = new AnimationClip();
            store.RegisterClip(clip1, (AnimationClip _) => { });
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip1));

            var clip2 = new AnimationClip();
            store.RegisterMotion(clip2, (AnimationClip _) => { });
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip2));

            var clip3 = new AnimationClip();
            var visitedMotions = new HashSet<Motion>();
            store.RegisterMotion(clip3, (AnimationClip _) => { }, null, visitedMotions);
            store.RegisterMotion(clip3, (AnimationClip _) => { }, null, visitedMotions);
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip3));
        }

        [Test]
        public void RegisterBlendTreeTest()
        {
            var cabCtx = new ApplyCabinetContext();
            var store = new AnimationStore(cabCtx);

            var bt = new BlendTree();
            var clip1 = new AnimationClip();
            var clip2 = new AnimationClip();
            var clip3 = new AnimationClip();
            bt.AddChild(clip1);
            bt.AddChild(clip2);
            bt.AddChild(clip3);
            bt.AddChild(clip3);

            store.RegisterMotion(bt, (AnimationClip _) => { });

            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip1));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip2));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip3));
        }

        [Test]
        public void RegisterNestedBlendTreeTest()
        {
            var cabCtx = new ApplyCabinetContext();
            var store = new AnimationStore(cabCtx);

            var bt1 = new BlendTree();
            var clip1 = new AnimationClip();
            var clip2 = new AnimationClip();
            var clip3 = new AnimationClip();
            bt1.AddChild(clip1);
            bt1.AddChild(clip2);
            bt1.AddChild(clip3);
            bt1.AddChild(clip3);

            var bt2 = new BlendTree();
            var clip4 = new AnimationClip();
            bt1.AddChild(bt2);
            bt2.AddChild(clip1);
            bt2.AddChild(clip2);
            bt2.AddChild(clip3);
            bt2.AddChild(clip4);

            var visitedMotions = new HashSet<Motion>();
            var clip5 = new AnimationClip();
            store.RegisterMotion(clip5, (AnimationClip _) => { }, null, visitedMotions);
            store.RegisterMotion(bt1, (AnimationClip _) => { }, null, visitedMotions);
            store.RegisterMotion(bt2, (AnimationClip _) => { }, null, visitedMotions);

            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip1));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip2));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip3));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip4));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip5));
        }

        [Test]
        public void FilterMotionFuncTest()
        {
            var cabCtx = new ApplyCabinetContext();
            var store = new AnimationStore(cabCtx);

            var bt = new BlendTree();
            var clip1 = new AnimationClip();
            var clip2 = new AnimationClip();
            var clip3 = new AnimationClip();
            bt.AddChild(clip1);
            bt.AddChild(clip2);
            bt.AddChild(clip3);

            store.RegisterMotion(bt, (AnimationClip _) => { }, (Motion m) => m != clip3);

            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip1));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip2));
            Assert.That(store.Clips, Has.Exactly(0).Matches<AnimationClipContainer>(c => c.originalClip == clip3));
        }

        [Test]
        public void DispatchTest()
        {
            var cabCtx = new ApplyCabinetContext()
            {
                avatarGameObject = CreateGameObject("Avatar")
            };
            var store = new AnimationStore(cabCtx);

            var bt = new BlendTree();
            var clip1 = new AnimationClip();
            var clip2 = new AnimationClip();
            var clip3 = new AnimationClip();
            bt.AddChild(clip1);
            bt.AddChild(clip2);
            bt.AddChild(clip3);

            var dispatchedClips = new List<AnimationClip>();

            var visitedMotions = new HashSet<Motion>();
            var clip4 = new AnimationClip();
            store.RegisterMotion(clip4, (AnimationClip c) => dispatchedClips.Add(c), null, visitedMotions);
            store.RegisterMotion(bt, (AnimationClip c) => dispatchedClips.Add(c), null, visitedMotions);

            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip1));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip2));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip3));
            Assert.That(store.Clips, Has.Exactly(1).Matches<AnimationClipContainer>(c => c.originalClip == clip4));

            var newClip1 = new AnimationClip();
            var newClip2 = new AnimationClip();
            store.Clips[0].newClip = newClip1;
            store.Clips[0].newClip = newClip2;

            store.Dispatch();

            // TODO: asserts?
        }
    }
}
