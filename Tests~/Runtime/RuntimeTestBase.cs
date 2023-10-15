using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests
{
    // a test script base containing utility functions
    public class RuntimeTestBase
    {
        public const string GeneratedAssetsFolderName = "_DTGeneratedAssets";
        public const string GeneratedAssetsPath = "Assets/" + GeneratedAssetsFolderName;
        protected List<GameObject> instantiatedGameObjects;

        protected T LoadRuntimeTestAsset<T>(string relativePath) where T : Object
        {
            // load test asset from resources folder
            var path = "Packages/com.chocopoi.vrc.dressingframework/Tests/Runtime/Resources/" + GetType().Name + "/" + relativePath;
            var obj = AssetDatabase.LoadAssetAtPath<T>(path);
            Assert.NotNull(obj, "Could not find test asset at path:" + path);
            return obj;
        }

        protected GameObject InstantiateRuntimeTestPrefab(string relativePath, Transform parent = null)
        {
            // load test prefab and instantiate it
            var prefab = LoadRuntimeTestAsset<GameObject>(relativePath);
            var obj = Object.Instantiate(prefab);
            instantiatedGameObjects.Add(obj);

            if (parent)
            {
                obj.transform.parent = parent;
            }
            return obj;
        }

        protected GameObject CreateGameObject(string name = null, Transform parent = null)
        {
            // create an object and bound it to the parent (if any)
            var obj = new GameObject(name);
            if (parent)
            {
                obj.transform.parent = parent.transform;
            }
            instantiatedGameObjects.Add(obj);
            return obj;
        }

        [SetUp]
        public virtual void SetUp()
        {
            // init list
            instantiatedGameObjects = new List<GameObject>();

            // remove previous generated files
            AssetDatabase.DeleteAsset(GeneratedAssetsPath);
            AssetDatabase.CreateFolder("Assets", GeneratedAssetsFolderName);
        }

        [TearDown]
        public virtual void TearDown()
        {
            // remove all instantiated gameobjects from tests
            foreach (var obj in instantiatedGameObjects)
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
}
