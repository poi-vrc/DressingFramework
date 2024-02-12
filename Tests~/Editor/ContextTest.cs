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

using Chocopoi.DressingFramework.Logging;
using NUnit.Framework;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests
{
    public class ContextTest : EditorTestBase
    {
        public class DummyContext : Context
        {

            public override object RuntimeContext => throw new System.NotImplementedException();
            public override Object AssetContainer => throw new System.NotImplementedException();
            internal override Report Report => throw new System.NotImplementedException();

            public DummyContext(GameObject avatarGameObject) : base(avatarGameObject)
            {
                AddContextFeature(new DummyFeature1());
            }

            public override void CreateAsset(Object obj, string name)
            {
                throw new System.NotImplementedException();
            }
        }

        public class DummyFeature1 : ContextFeature
        {
            internal override void OnDisable()
            {
            }

            internal override void OnEnable()
            {
            }
        }

        public class DummyFeature2 : ContextFeature
        {
            internal override void OnDisable()
            {
            }

            internal override void OnEnable()
            {
            }
        }

        private class TestExtraContext : IExtraContext
        {
            public int test = 123456;

            public void OnDisable(Context ctx) { }

            public void OnEnable(Context ctx) { }
        }

        [Test]
        public void ExtraContextTest()
        {
            var ctx = new DummyContext(CreateGameObject("abc"));
            var extraCtx = ctx.Extra<TestExtraContext>();
            Assert.NotNull(extraCtx);
            Assert.AreEqual(123456, extraCtx.test);
            Assert.AreEqual(extraCtx, ctx.Extra(typeof(TestExtraContext)));
        }

        [Test]
        public void ContextFeatureTest()
        {
            var ctx = new DummyContext(CreateGameObject("abc"));

            ctx.OnEnable();

            var existingFeature = ctx.Feature<DummyFeature1>();
            Assert.NotNull(existingFeature);

            var nonExistingFeature = ctx.Feature<DummyFeature2>();
            Assert.Null(nonExistingFeature);

            ctx.OnDisable();
        }
    }
}
