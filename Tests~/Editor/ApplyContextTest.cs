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

using Chocopoi.DressingFramework.Detail.DK;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests
{
    public class ApplyContextTest : EditorTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            AssetDatabase.DeleteAsset(DKNativeContext.GeneratedAssetsPath);
            AssetDatabase.CreateFolder("Assets", DKNativeContext.GeneratedAssetsFolderName);
        }

        public override void TearDown()
        {
            base.TearDown();
            AssetDatabase.DeleteAsset(DKNativeContext.GeneratedAssetsPath);
        }

        [Test]
        public void MakeUnqiueNameTest()
        {
            var ctx1 = new DKNativeContext(CreateGameObject("abc"));
            var ctx2 = new DKNativeContext(CreateGameObject("abc"));
            Assert.AreEqual(ctx1.AvatarGameObject.name, ctx2.AvatarGameObject.name);
            Assert.AreNotEqual(ctx1.MakeUniqueName("123"), ctx2.MakeUniqueName("123"));
        }

        [Test]
        public void MakeUniqueAssetPathTest()
        {
            var ctx1 = new DKNativeContext(CreateGameObject("abc"));
            var ctx2 = new DKNativeContext(CreateGameObject("abc"));
            Assert.AreEqual(ctx1.AvatarGameObject.name, ctx2.AvatarGameObject.name);
            Assert.AreNotEqual(ctx1.MakeUniqueAssetPath("123"), ctx2.MakeUniqueAssetPath("123"));
        }

        [Test]
        public void CreateUniqueAssetTest()
        {
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var obj = new AnimationClip();

            var name = "assetName123.asset";
            var expectedPath = ctx.MakeUniqueAssetPath(name);
            ctx.CreateUniqueAsset(obj, name);

            Assert.False(string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(expectedPath)));
        }

        private class TestExtraContext : IExtraContext
        {
            public int test = 123456;

            public void OnDisable(Context ctx)
            {
                throw new System.NotImplementedException();
            }

            public void OnEnable(Context ctx)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void ExtraContextTest()
        {
            var ctx = new DKNativeContext(CreateGameObject("abc"));
            var extraCtx = ctx.Extra<TestExtraContext>();
            Assert.NotNull(extraCtx);
            Assert.AreEqual(123456, extraCtx.test);
            Assert.AreEqual(extraCtx, ctx.Extra(typeof(TestExtraContext)));
        }
    }
}
