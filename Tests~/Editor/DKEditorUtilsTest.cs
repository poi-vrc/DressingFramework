using System.Collections.Generic;
using Chocopoi.DressingFramework.Proxy;
using NUnit.Framework;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests
{
    public class DKEditorUtilsTest : EditorTestBase
    {
        public class DummyClass1 { }
        public class DummyClass2 { }

        [Test]
        public void FindType_ReturnsCorrectType()
        {
            var myDummyType = typeof(DummyClass1);
            Debug.Log("DummyClass1 type full name: " + myDummyType.FullName);

            var ret = DKEditorUtils.FindType(myDummyType.FullName);
            Assert.NotNull(ret);
            Assert.AreEqual(myDummyType, ret);
        }

        [Test]
        public void FindType_CacheCoverage()
        {
            // just for passing coverage, doesn't really test anything
            var myDummyType = typeof(DummyClass2);
            Debug.Log("DummyClass2 type full name: " + myDummyType.FullName);

            var ret1 = DKEditorUtils.FindType(myDummyType.FullName);
            Assert.NotNull(ret1);
            Assert.AreEqual(myDummyType, ret1);

            var ret2 = DKEditorUtils.FindType(myDummyType.FullName);
            Assert.NotNull(ret2);
            Assert.AreEqual(myDummyType, ret2);
        }

        [Test]
        public void FindType_NoSuchType_ReturnsNull()
        {
            var ret = DKEditorUtils.FindType("Abababababa.No.Such.Class");
            Assert.Null(ret);
        }

        [Test]
        public void IsGrandParentTest()
        {
            //
            // Root
            //  |- Child1
            //  |   |- GrandChild1
            //  |   |   |- GrandGrandChild1
            //  |   |   |- GrandGrandChild2
            //  |   |- GrandChild2
            //  |- Child2
            //
            var root = InstantiateEditorTestPrefab("IsGrandParentTestPrefab.prefab");
            var randomObject = CreateGameObject("IsGrandParentRandomObject");

            var child1 = root.transform.Find("Child1");
            var grandChild1 = child1.Find("GrandChild1");
            var grandGrandChild1 = grandChild1.Find("GrandGrandChild1");
            var grandGrandChild2 = grandChild1.Find("GrandGrandChild2");
            var grandChild2 = child1.Find("GrandChild2");
            var child2 = root.transform.Find("Child2");
            Assert.NotNull(child1);
            Assert.NotNull(grandChild1);
            Assert.NotNull(grandGrandChild1);
            Assert.NotNull(grandGrandChild2);
            Assert.NotNull(grandChild2);
            Assert.NotNull(child2);

            // depth 0
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, child1));
            Assert.True(DKEditorUtils.IsGrandParent(child1, grandChild1));
            Assert.True(DKEditorUtils.IsGrandParent(grandChild1, grandGrandChild1));
            Assert.True(DKEditorUtils.IsGrandParent(grandChild1, grandGrandChild2));
            Assert.True(DKEditorUtils.IsGrandParent(child1, grandChild2));
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, child2));
            Assert.False(DKEditorUtils.IsGrandParent(child2, grandChild1));
            Assert.False(DKEditorUtils.IsGrandParent(child2, grandChild2));
            Assert.False(DKEditorUtils.IsGrandParent(grandChild2, grandGrandChild1));
            Assert.False(DKEditorUtils.IsGrandParent(grandChild2, grandGrandChild2));

            // depth 1
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, grandChild1));
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, grandChild2));
            Assert.True(DKEditorUtils.IsGrandParent(child1, grandGrandChild1));
            Assert.True(DKEditorUtils.IsGrandParent(child1, grandGrandChild2));
            Assert.False(DKEditorUtils.IsGrandParent(child2, grandGrandChild1));
            Assert.False(DKEditorUtils.IsGrandParent(child2, grandGrandChild2));

            // depth 2
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, grandGrandChild1));
            Assert.True(DKEditorUtils.IsGrandParent(root.transform, grandGrandChild2));

            // Random object
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, root.transform));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, child1));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, grandChild1));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, grandGrandChild1));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, grandGrandChild2));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, grandChild2));
            Assert.False(DKEditorUtils.IsGrandParent(randomObject.transform, child2));
        }

        private static void AssertScannedDynamics(GameObject root)
        {
            var excludeWearableDynamics = DKEditorUtils.ScanDynamics(root, true);
            Assert.AreEqual(2, excludeWearableDynamics.Count, "Should have 2 dynamics with wearable dynamics excluded");

            var includeWearableDynamics = DKEditorUtils.ScanDynamics(root, false);
            Assert.AreEqual(4, includeWearableDynamics.Count, "Should have 4 dynamics with wearable dynamics included");
        }

        [Test]
        public void ScanDynamics_DynamicsBone()
        {
            var DynamicBoneType = DKEditorUtils.FindType("DynamicBone");
            if (DynamicBoneType == null)
            {
                Assert.Pass("DynamicBone is not imported, skipping this test");
                return;
            }

            var root = InstantiateEditorTestPrefab("DKTest_DynamicBoneAvatar.prefab");
            InstantiateEditorTestPrefab("DKTest_DynamicBoneWearable.prefab", root.transform);
            AssertScannedDynamics(root);
        }

        [Test]
        public void ScanDynamics_PhysBone()
        {
#if !VRC_SDK_VRCSDK3
            Assert.Pass("VRCSDK is not imported, skipping this test");
#else
            var root = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatar.prefab");
            InstantiateEditorTestPrefab("DKTest_PhysBoneWearable.prefab", root.transform);
            AssertScannedDynamics(root);
#endif
        }

        private class DummyDynamicsProxy : IDynamicsProxy
        {
            public Component Component { get; set; } = null;
            public Transform Transform => null;
            public GameObject GameObject => null;
            public Transform RootTransform { get; set; }
            public List<Transform> IgnoreTransforms { get; set; }

            public DummyDynamicsProxy(Transform rootTransform)
            {
                RootTransform = rootTransform;
                IgnoreTransforms = null;
            }
        }

        [Test]
        public void FindDynamicsWithRootTest()
        {
            var obj1 = CreateGameObject("FindDynamicsWithRootObj1");
            var obj2 = CreateGameObject("FindDynamicsWithRootObj2");
            var obj3 = CreateGameObject("FindDynamicsWithRootObj3");
            var obj4 = CreateGameObject("FindDynamicsWithRootObj4");
            var list = new List<IDynamicsProxy>() {
                new DummyDynamicsProxy(obj1.transform),
                new DummyDynamicsProxy(obj2.transform),
                new DummyDynamicsProxy(obj3.transform)
            };

            var dynObj1 = DKEditorUtils.FindDynamicsWithRoot(list, obj1.transform);
            Assert.NotNull(dynObj1);
            Assert.AreEqual(obj1.transform, dynObj1.RootTransform);

            var dynObj2 = DKEditorUtils.FindDynamicsWithRoot(list, obj2.transform);
            Assert.NotNull(dynObj2);
            Assert.AreEqual(obj2.transform, dynObj2.RootTransform);

            var dynObj3 = DKEditorUtils.FindDynamicsWithRoot(list, obj3.transform);
            Assert.NotNull(dynObj3);
            Assert.AreEqual(obj3.transform, dynObj3.RootTransform);

            Assert.Null(DKEditorUtils.FindDynamicsWithRoot(list, obj4.transform));
        }

        [Test]
        public void IsDynamicsExistsTest()
        {
            var obj1 = CreateGameObject("IsDynamicsExistsTestObj1");
            var obj2 = CreateGameObject("IsDynamicsExistsTestObj2");
            var list = new List<IDynamicsProxy>() {
                new DummyDynamicsProxy(obj1.transform),
            };
            Assert.True(DKEditorUtils.IsDynamicsExists(list, obj1.transform));
            Assert.False(DKEditorUtils.IsDynamicsExists(list, obj2.transform));
        }

        private class DummyComponent : MonoBehaviour
        {
            public static int SomeStaticField = 24680;

            public string SomeStringProperty { get; set; }
            public int SomeIntProperty { get; set; }

            public string someString;
            public bool someBool;
            public int someInt;
        }

        [Test]
        public void CopyComponentTest()
        {
            var obj1 = CreateGameObject("CopyComponentObj1");
            var obj2 = CreateGameObject("CopyComponentObj2");

            var comp = obj1.AddComponent<DummyComponent>();
            comp.SomeStringProperty = "Bakabaka";
            comp.SomeIntProperty = 123456;
            comp.someBool = true;
            comp.someInt = 654321;
            comp.someString = "HelloWorld";

            var copiedComp = DKEditorUtils.CopyComponent(comp, obj2);

            var gotComp = obj2.GetComponent<DummyComponent>();
            Assert.NotNull(gotComp);
            Assert.AreEqual(copiedComp, gotComp);

            Assert.AreEqual(comp.SomeStringProperty, gotComp.SomeStringProperty);
            Assert.AreEqual(comp.SomeIntProperty, gotComp.SomeIntProperty);
            Assert.AreEqual(comp.someBool, gotComp.someBool);
            Assert.AreEqual(comp.someInt, gotComp.someInt);
            Assert.AreEqual(comp.someString, gotComp.someString);
        }

        [Test]
        public void IsOriginatedFromAnyWearableTest()
        {
            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatar.prefab");
            var wearableRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneWearable.prefab", avatarRoot.transform);

            // we take the armatures GameObject as the test subjects
            var avatarRootArmature = avatarRoot.transform.Find("Armature");
            Assert.NotNull(avatarRootArmature);
            var wearableRootArmature = wearableRoot.transform.Find("Armature");
            Assert.NotNull(wearableRootArmature);

            Assert.True(DKEditorUtils.IsOriginatedFromAnyWearable(avatarRoot.transform, wearableRootArmature));
            Assert.False(DKEditorUtils.IsOriginatedFromAnyWearable(avatarRoot.transform, avatarRootArmature));
        }
    }
}
