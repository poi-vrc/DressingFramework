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
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Serialization
{
    public class CabinetConfigUtilityTest : EditorTestBase
    {
        private class TestCabinetConfigUtilityCabinetModuleConfig : IModuleConfig
        {
            public const string ModuleIdentifier = "com.chocopoi.vrc.dressingframework.tests.test-cabinet-config-utility-cabinet-module";
            public SerializationVersion version;
        }

        private class TestCabinetConfigUtilityCabinetModuleProvider : CabinetModuleProviderBase
        {
            public override CabinetApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Transpose, CabinetHookStageRunOrder.Before).Build();

            public override string Identifier => TestCabinetConfigUtilityCabinetModuleConfig.ModuleIdentifier;
            public override string FriendlyName => "Test cabinet config utility";
            public override bool AllowMultiple => false;

            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => jObject.ToObject<TestCabinetConfigUtilityCabinetModuleConfig>();
            public override bool Invoke(ApplyCabinetContext cabCtx, ReadOnlyCollection<CabinetModule> modules, bool isPreview) => true;
            public override IModuleConfig NewModuleConfig() => new TestCabinetConfigUtilityCabinetModuleConfig();
        }

        private class TestCabinetConfigUtilityPlugin : PluginBase
        {
            public override string FriendlyName => "Test cabinet config utility plugin";
            public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

            public override void OnEnable()
            {
                RegisterCabinetModuleProvider(new TestCabinetConfigUtilityCabinetModuleProvider());
            }

            public override void OnDisable()
            {
            }
        }

        private const string ValidCabinetJson = "{\"version\":\"1.0.0\",\"avatarArmatureName\":\"Armature\",\"groupDynamics\":true,\"groupDynamicsSeparateGameObjects\":true,\"animationWriteDefaults\":true,\"modules\":[{\"moduleName\":\"com.chocopoi.vrc.dressingframework.tests.test-cabinet-config-utility-cabinet-module\",\"config\":{\"version\":\"1.0.0\"}}]}";

        private static void AssertValidCabinetConfig(CabinetConfig config)
        {
            Assert.AreEqual("1.0.0", config.version.ToString());
            Assert.AreEqual("Armature", config.avatarArmatureName);
            Assert.AreEqual(true, config.groupDynamics);
            Assert.AreEqual(true, config.groupDynamicsSeparateGameObjects);
            Assert.AreEqual(true, config.animationWriteDefaults);

            Assert.AreEqual(1, config.modules.Count);
            Assert.AreEqual(TestCabinetConfigUtilityCabinetModuleConfig.ModuleIdentifier, config.modules[0].moduleName);
            Assert.IsInstanceOf(typeof(TestCabinetConfigUtilityCabinetModuleConfig), config.modules[0].config);
        }

        [Test]
        public void TryDeserializeTest()
        {
            Assert.True(CabinetConfigUtility.TryDeserialize(ValidCabinetJson, out var config));
            AssertValidCabinetConfig(config);

            LogAssert.Expect(LogType.Exception, new Regex("^JsonReaderException: Unexpected character encountered while parsing value.+"));
            Assert.False(CabinetConfigUtility.TryDeserialize(ValidCabinetJson.Substring(5), out _));
        }

        [Test]
        public void SerializeTest()
        {
            var config = new CabinetConfig();

            var json = CabinetConfigUtility.Serialize(config);
            Assert.NotNull(json);

            // maintain consistency with JsonConvert.SerializeObject for now
            // since some codes rely on this currently
            Assert.AreEqual(json, JsonConvert.SerializeObject(config));
        }

        [Test]
        public void DeserializeTest()
        {
            var config = CabinetConfigUtility.Deserialize(ValidCabinetJson);
            AssertValidCabinetConfig(config);
        }

        [Test]
        public void CloneTest()
        {
            var config = new CabinetConfig();
            var clone = CabinetConfigUtility.Clone(config);
            Assert.AreNotEqual(clone, config);
            Assert.AreEqual(CabinetConfigUtility.Serialize(clone), CabinetConfigUtility.Serialize(config));
        }
    }
}
