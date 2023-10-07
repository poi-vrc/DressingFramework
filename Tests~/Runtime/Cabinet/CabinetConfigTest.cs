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

using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Serialization;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Cabinet
{
    public class CabinetConfigTest : RuntimeTestBase
    {
        private class TestModuleConfig : IModuleConfig
        {
            public const string ModuleIdentifier = "test.module.config";
        }

        private static void AddTestModuleConfig(CabinetConfig config)
        {
            config.modules.Add(new CabinetModule()
            {
                moduleName = TestModuleConfig.ModuleIdentifier,
                config = new TestModuleConfig()
            });
        }

        [Test]
        public void FindModuleConfigTest()
        {
            var config = new CabinetConfig();
            Assert.Null(config.FindModuleConfig<TestModuleConfig>());
            AddTestModuleConfig(config);
            Assert.NotNull(config.FindModuleConfig<TestModuleConfig>());
        }

        [Test]
        public void FindModuleConfigsTest()
        {
            var config = new CabinetConfig();
            Assert.AreEqual(0, config.FindModuleConfigs<TestModuleConfig>().Count);
            AddTestModuleConfig(config);
            AddTestModuleConfig(config);
            Assert.AreEqual(2, config.FindModuleConfigs<TestModuleConfig>().Count);
        }

        [Test]
        public void FindModuleTest()
        {
            var config = new CabinetConfig();
            Assert.Null(config.FindModule(TestModuleConfig.ModuleIdentifier));
            AddTestModuleConfig(config);
            Assert.NotNull(config.FindModule(TestModuleConfig.ModuleIdentifier));
        }

        [Test]
        public void FindModulesTest()
        {
            var config = new CabinetConfig();
            Assert.AreEqual(0, config.FindModules(TestModuleConfig.ModuleIdentifier).Count);
            AddTestModuleConfig(config);
            AddTestModuleConfig(config);
            Assert.AreEqual(2, config.FindModules(TestModuleConfig.ModuleIdentifier).Count);
        }
    }
}
