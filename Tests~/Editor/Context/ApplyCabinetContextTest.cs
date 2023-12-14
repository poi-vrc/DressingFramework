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

using Chocopoi.DressingFramework.Context;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests.Context
{
    public class ApplyCabinetContextTest : EditorTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            AssetDatabase.DeleteAsset(ApplyCabinetContext.GeneratedAssetsPath);
            AssetDatabase.CreateFolder("Assets", ApplyCabinetContext.GeneratedAssetsFolderName);
        }

        public override void TearDown()
        {
            base.TearDown();
            AssetDatabase.DeleteAsset(ApplyCabinetContext.GeneratedAssetsPath);
        }

        [Test]
        public void MakeUnqiueNameTest()
        {
            var ctx1 = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            var ctx2 = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            Assert.AreEqual(ctx1.avatarGameObject.name, ctx2.avatarGameObject.name);
            Assert.AreNotEqual(ctx1.MakeUniqueName("123"), ctx2.MakeUniqueName("123"));
        }

        [Test]
        public void MakeUniqueAssetPathTest()
        {
            var ctx1 = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            var ctx2 = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            Assert.AreEqual(ctx1.avatarGameObject.name, ctx2.avatarGameObject.name);
            Assert.AreNotEqual(ctx1.MakeUniqueAssetPath("123"), ctx2.MakeUniqueAssetPath("123"));
        }

        [Test]
        public void CreateUniqueAssetTest()
        {
            var ctx = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            var obj = new AnimationClip();

            var name = "assetName123.asset";
            var expectedPath = ctx.MakeUniqueAssetPath(name);
            ctx.CreateUniqueAsset(obj, name);

            Assert.False(string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(expectedPath)));
        }

        private class TestExtraCabinetContext : ExtraCabinetContext
        {
            public int test = 123456;
        }

        [Test]
        public void ExtraContextTest()
        {
            var ctx = new ApplyCabinetContext() { avatarGameObject = CreateGameObject("abc") };
            var extraCtx = ctx.Extra<TestExtraCabinetContext>();
            Assert.NotNull(extraCtx);
            Assert.AreEqual(123456, extraCtx.test);
            Assert.AreEqual(extraCtx, ctx.Extra(typeof(TestExtraCabinetContext)));
        }
    }
}
