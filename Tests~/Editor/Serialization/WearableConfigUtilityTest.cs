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

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingFramework.Wearable.Modules;
using Chocopoi.DressingFramework.Wearable.Modules.BuiltIn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Serialization
{
    public class WearableConfigUtilityTest : EditorTestBase
    {
        private class TestWearableConfigUtilityWearableModuleConfig : IModuleConfig
        {
            public const string ModuleIdentifier = "com.chocopoi.vrc.dressingframework.tests.test-wearable-config-utility-wearable-module";
            public SerializationVersion version;
        }

        private class TestWearableConfigUtilityWearableModuleProvider : WearableModuleProviderBase
        {
            public override WearableApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Transpose).Build();

            public override string Identifier => TestWearableConfigUtilityWearableModuleConfig.ModuleIdentifier;
            public override string FriendlyName => "Test wearable config utility";
            public override bool AllowMultiple => false;

            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => jObject.ToObject<TestWearableConfigUtilityWearableModuleConfig>();
            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview) => true;
            public override IModuleConfig NewModuleConfig() => new TestWearableConfigUtilityWearableModuleConfig();
        }

        private class TestWearableConfigUtilityPlugin : PluginBase
        {
            public override string FriendlyName => "Test wearable config utility plugin";
            public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

            public override void OnEnable()
            {
                RegisterWearableModuleProvider(new TestWearableConfigUtilityWearableModuleProvider());
            }

            public override void OnDisable()
            {
            }
        }

        private const string ValidWearableJson = "{\"version\":\"1.0.0\",\"modules\":[{\"moduleName\":\"com.chocopoi.vrc.dressingframework.tests.test-wearable-config-utility-wearable-module\",\"config\":{\"version\":\"1.0.0\"}}],\"info\":{\"uuid\":\"ec9958bf-17f0-4598-9a3e-450e477e2e4b\",\"name\":\"GameObject\",\"author\":\"\",\"description\":\"\",\"createdTime\":\"2023-10-04T09:37:19.9417231Z\",\"updatedTime\":\"2023-10-04T09:37:28.2582680Z\",\"thumbnail\":null},\"avatarConfig\":{\"guids\":[],\"name\":\"GameObject (1)\",\"armatureName\":\"Armature\",\"worldPosition\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"worldRotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0},\"avatarLossyScale\":{\"x\":1.0,\"y\":1.0,\"z\":1.0},\"wearableLossyScale\":{\"x\":1.0,\"y\":1.0,\"z\":1.0}}}";

        private static void AssertValidWearableConfig(WearableConfig config)
        {
            Assert.AreEqual("1.0.0", config.version.ToString());

            Assert.AreEqual(1, config.modules.Count);
            Assert.AreEqual(TestWearableConfigUtilityWearableModuleConfig.ModuleIdentifier, config.modules[0].moduleName);
            Assert.IsInstanceOf(typeof(TestWearableConfigUtilityWearableModuleConfig), config.modules[0].config);

            Assert.IsInstanceOf(typeof(WearableInfo), config.info);
            Assert.IsInstanceOf(typeof(AvatarConfig), config.avatarConfig);
        }

        [Test]
        public void TryDeserializeTest()
        {
            Assert.True(WearableConfigUtility.TryDeserialize(ValidWearableJson, out var config));
            AssertValidWearableConfig(config);

            LogAssert.Expect(LogType.Exception, new Regex("^JsonReaderException: Unexpected character encountered while parsing value.+"));
            Assert.False(WearableConfigUtility.TryDeserialize(ValidWearableJson.Substring(5), out _));
        }

        [Test]
        public void SerializeTest()
        {
            var config = new WearableConfig();

            var json = WearableConfigUtility.Serialize(config);
            Assert.NotNull(json);

            // maintain consistency with JsonConvert.SerializeObject for now
            // since some codes rely on this currently
            Assert.AreEqual(json, JsonConvert.SerializeObject(config));
        }

        [Test]
        public void DeserializeTest()
        {
            var config = WearableConfigUtility.Deserialize(ValidWearableJson);
            AssertValidWearableConfig(config);
        }

        [Test]
        public void CloneTest()
        {
            var config = new WearableConfig();
            var clone = WearableConfigUtility.Clone(config);
            Assert.AreNotEqual(config, clone);
            Assert.AreEqual(WearableConfigUtility.Serialize(config), WearableConfigUtility.Serialize(clone));
        }
    }
}
