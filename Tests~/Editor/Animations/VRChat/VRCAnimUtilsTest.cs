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
using Chocopoi.DressingFramework.Animations.VRChat;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Logging;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace Chocopoi.DressingFramework.Tests.Animations.VRChat
{
    public class VRCAnimUtilsTest : EditorTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            if (!AssetDatabase.IsValidFolder("Assets/_DTGeneratedAssets"))
            {
                AssetDatabase.CreateFolder("Assets", "_DTGeneratedAssets");
            }
        }

        public override void TearDown()
        {
            base.TearDown();
            AssetDatabase.DeleteAsset("Assets/_DTGeneratedAssets");
        }

        [Test]
        public void GetAnimLayerAnimatorTest()
        {
            var defaultFxLayerAnimator = VRCAnimUtils.GetDefaultLayerAnimator(AnimLayerType.FX);

            var ctrl1 = new AnimatorController();
            var animator = CreateGameObject("TestAnimator");
            var ctrl2 = animator.AddComponent<Animator>().runtimeAnimatorController;

            var dummy1 = new VRCAvatarDescriptor.CustomAnimLayer()
            {
                type = AnimLayerType.FX,
                isDefault = true,
                animatorController = null
            };

            var dummy2 = new VRCAvatarDescriptor.CustomAnimLayer()
            {
                type = AnimLayerType.FX,
                isDefault = false,
                animatorController = null
            };

            var dummy3 = new VRCAvatarDescriptor.CustomAnimLayer()
            {
                type = AnimLayerType.FX,
                isDefault = false,
                animatorController = ctrl2
            };

            var dummy4 = new VRCAvatarDescriptor.CustomAnimLayer()
            {
                type = AnimLayerType.FX,
                isDefault = false,
                animatorController = ctrl1
            };

            Assert.AreEqual(defaultFxLayerAnimator, VRCAnimUtils.GetCustomAnimLayerAnimator(dummy1));
            Assert.AreEqual(defaultFxLayerAnimator, VRCAnimUtils.GetCustomAnimLayerAnimator(dummy2));
            Assert.AreEqual(defaultFxLayerAnimator, VRCAnimUtils.GetCustomAnimLayerAnimator(dummy3));
            Assert.AreEqual(ctrl1, VRCAnimUtils.GetCustomAnimLayerAnimator(dummy4));
        }

        [Test]
        public void GetDefaultLayerAnimatorTest()
        {
            var layers = new AnimLayerType[] {
                AnimLayerType.Base,
                AnimLayerType.Additive,
                AnimLayerType.Action,
                AnimLayerType.Gesture,
                AnimLayerType.FX,
                AnimLayerType.Sitting,
                AnimLayerType.IKPose,
                AnimLayerType.TPose
             };

            foreach (var layer in layers)
            {
                var animator = VRCAnimUtils.GetDefaultLayerAnimator(layer);
                Assert.NotNull(animator, $"{layer} default animator should not be null");
            }
        }

        [Test]
        public void FindAnimLayerArrayAndIndexTest()
        {
            var obj = CreateGameObject("Test");
            var ad = obj.AddComponent<VRCAvatarDescriptor>();

            var dummy1 = new AnimatorController();
            var dummy2 = new AnimatorController();

            ad.baseAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
                new VRCAvatarDescriptor.CustomAnimLayer() {
                    type = AnimLayerType.Base,
                    animatorController = dummy1
                }
            };

            ad.specialAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
                new VRCAvatarDescriptor.CustomAnimLayer() {
                    type = AnimLayerType.IKPose,
                    animatorController = dummy2
                }
            };

            VRCAnimUtils.FindAnimLayerArrayAndIndex(ad, AnimLayerType.Base, out var layers1, out var index1);
            Assert.AreEqual(ad.baseAnimationLayers, layers1);
            Assert.AreEqual(0, index1);

            VRCAnimUtils.FindAnimLayerArrayAndIndex(ad, AnimLayerType.IKPose, out var layers2, out var index2);
            Assert.AreEqual(ad.specialAnimationLayers, layers2);
            Assert.AreEqual(0, index2);
        }

        [Test]
        public void GetAvatarLayerAnimator()
        {
            var obj = CreateGameObject("Test");
            var ad = obj.AddComponent<VRCAvatarDescriptor>();

            var dummy1 = new AnimatorController();

            ad.baseAnimationLayers = new VRCAvatarDescriptor.CustomAnimLayer[] {
                new VRCAvatarDescriptor.CustomAnimLayer() {
                    type = AnimLayerType.Base,
                    animatorController = dummy1
                }
            };

            Assert.AreEqual(dummy1, VRCAnimUtils.GetAvatarLayerAnimator(ad, AnimLayerType.Base));
        }

        private class TestContext : Context
        {
            public override BuildRuntime CurrentRuntime => throw new System.NotImplementedException();
            public override object RuntimeContext => throw new System.NotImplementedException();
            internal override Report Report => throw new System.NotImplementedException();

            public override Object AssetContainer { get => _ac; }

            private readonly Object _ac;

            public TestContext(GameObject avatarGameObject) : base(avatarGameObject)
            {
                _ac = ScriptableObject.CreateInstance<DKNativeAssetContainer>();
                AssetDatabase.CreateAsset(_ac, "Assets/_DTGeneratedAssets/VRCAnimUtilsTestOutput.asset");
            }

            public override void CreateAsset(Object obj, string name)
            {
                obj.name = name;
                AssetDatabase.AddObjectToAsset(obj, _ac);
                EditorUtility.SetDirty(_ac);
            }
        }

        [Test]
        public void CloneAnimatorTest()
        {
            var source = LoadEditorTestAsset<AnimatorController>("TestCloneAnimator.controller");
            Assert.NotNull(source);

            var ctx = new TestContext(CreateGameObject("abc"));
            var cloned = VRCAnimUtils.DeepCopyAnimator(ctx, source);
            Assert.NotNull(cloned);

            Assert.AreEqual(3, cloned.parameters.Length);
            Assert.AreEqual(4, cloned.layers.Length);

            AssetDatabase.SaveAssets();
        }
    }
}
#endif
