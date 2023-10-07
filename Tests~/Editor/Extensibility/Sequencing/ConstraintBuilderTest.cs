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
using System.Collections.ObjectModel;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable.Modules;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Sequencing
{
    public class ConstraintBuilderTest : EditorTestBase
    {
        private class TestCabinetHook : CabinetHookBase
        {
            public override string FriendlyName => throw new System.NotImplementedException();
            public override CabinetApplyConstraint Constraint => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx) => throw new System.NotImplementedException();
        }

        private class TestCabinetModule : CabinetModuleProviderBase
        {
            public override string Identifier => throw new System.NotImplementedException();
            public override string FriendlyName => throw new System.NotImplementedException();
            public override CabinetApplyConstraint Constraint => throw new System.NotImplementedException();
            public override bool AllowMultiple => throw new System.NotImplementedException();
            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx, ReadOnlyCollection<CabinetModule> modules, bool isPreview) => throw new System.NotImplementedException();
            public override IModuleConfig NewModuleConfig() => throw new System.NotImplementedException();
        }

        private class TestWearableHook : WearableHookBase
        {
            public override string FriendlyName => throw new System.NotImplementedException();
            public override WearableApplyConstraint Constraint => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx) => throw new System.NotImplementedException();
        }

        private class TestWearableModule : WearableModuleProviderBase
        {
            public override string Identifier => throw new System.NotImplementedException();
            public override string FriendlyName => throw new System.NotImplementedException();
            public override WearableApplyConstraint Constraint => throw new System.NotImplementedException();
            public override bool AllowMultiple => throw new System.NotImplementedException();
            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview) => throw new System.NotImplementedException();
            public override IModuleConfig NewModuleConfig() => throw new System.NotImplementedException();
        }

        private static void AssertContainsDependency(List<Dependency> dependencies, DependencySource source, string identifier, bool optional)
        {
            foreach (var dependency in dependencies)
            {
                if (dependency.source == source && dependency.identifier == identifier)
                {
                    Assert.AreEqual(optional, dependency.optional, $"Dependency {source} {identifier} optional mismatch");
                    return;
                }
            }
            Assert.Fail($"Dependency not found {source} {identifier} (Optional: {optional})");
        }

        [Test]
        public void CabinetApplyConstraintBuilderTest()
        {
            var output = new CabinetApplyConstraintBuilder(CabinetApplyStage.Analyzing, CabinetHookStageRunOrder.After)
                .BeforeCabinetHook("com.example.before-cabinet-hook")
                .BeforeCabinetHook("com.example.before-cabinet-hook.optional", true)
                .BeforeCabinetHook(typeof(TestCabinetHook))
                .BeforeCabinetModule("com.example.before-cabinet-module")
                .BeforeCabinetModule("com.example.before-cabinet-module.optional", true)
                .BeforeCabinetModule(typeof(TestCabinetModule))
                .AfterCabinetHook("com.example.after-cabinet-hook")
                .AfterCabinetHook("com.example.after-cabinet-hook.optional", true)
                .AfterCabinetHook(typeof(TestCabinetHook))
                .AfterCabinetModule("com.example.after-cabinet-module")
                .AfterCabinetModule("com.example.after-cabinet-module.optional", true)
                .AfterCabinetModule(typeof(TestCabinetModule))
                .Build();

            Assert.AreEqual(CabinetApplyStage.Analyzing, output.stage);
            Assert.AreEqual(CabinetHookStageRunOrder.After, output.order);

            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetHook, "com.example.before-cabinet-hook", false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetHook, "com.example.before-cabinet-hook.optional", true);
            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetHook, typeof(TestCabinetHook).FullName, false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetModule, "com.example.before-cabinet-module", false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetModule, "com.example.before-cabinet-module.optional", true);
            AssertContainsDependency(output.beforeDependencies, DependencySource.CabinetModule, typeof(TestCabinetModule).FullName, false);

            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetHook, "com.example.after-cabinet-hook", false);
            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetHook, "com.example.after-cabinet-hook.optional", true);
            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetHook, typeof(TestCabinetHook).FullName, false);
            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetModule, "com.example.after-cabinet-module", false);
            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetModule, "com.example.after-cabinet-module.optional", true);
            AssertContainsDependency(output.afterDependencies, DependencySource.CabinetModule, typeof(TestCabinetModule).FullName, false);
        }

        [Test]
        public void WearableApplyConstraintBuilderTest()
        {
            var output = new WearableApplyConstraintBuilder(CabinetApplyStage.Transpose)
                .BeforeWearableHook("com.example.before-wearable-hook")
                .BeforeWearableHook("com.example.before-wearable-hook.optional", true)
                .BeforeWearableHook(typeof(TestWearableHook))
                .BeforeWearableModule("com.example.before-wearable-module")
                .BeforeWearableModule("com.example.before-wearable-module.optional", true)
                .BeforeWearableModule(typeof(TestWearableModule))
                .AfterWearableHook("com.example.after-wearable-hook")
                .AfterWearableHook("com.example.after-wearable-hook.optional", true)
                .AfterWearableHook(typeof(TestWearableHook))
                .AfterWearableModule("com.example.after-wearable-module")
                .AfterWearableModule("com.example.after-wearable-module.optional", true)
                .AfterWearableModule(typeof(TestWearableModule))
                .Build();

            Assert.AreEqual(CabinetApplyStage.Transpose, output.stage);

            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableHook, "com.example.before-wearable-hook", false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableHook, "com.example.before-wearable-hook.optional", true);
            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableHook, typeof(TestWearableHook).FullName, false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableModule, "com.example.before-wearable-module", false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableModule, "com.example.before-wearable-module.optional", true);
            AssertContainsDependency(output.beforeDependencies, DependencySource.WearableModule, typeof(TestWearableModule).FullName, false);

            AssertContainsDependency(output.afterDependencies, DependencySource.WearableHook, "com.example.after-wearable-hook", false);
            AssertContainsDependency(output.afterDependencies, DependencySource.WearableHook, "com.example.after-wearable-hook.optional", true);
            AssertContainsDependency(output.afterDependencies, DependencySource.WearableHook, typeof(TestWearableHook).FullName, false);
            AssertContainsDependency(output.afterDependencies, DependencySource.WearableModule, "com.example.after-wearable-module", false);
            AssertContainsDependency(output.afterDependencies, DependencySource.WearableModule, "com.example.after-wearable-module.optional", true);
            AssertContainsDependency(output.afterDependencies, DependencySource.WearableModule, typeof(TestWearableModule).FullName, false);
        }

        private class TestPlugin : PluginBase
        {
            public override string FriendlyName => "Test constraint builder test plugin";
            public override ExecutionConstraint Constraint => throw new System.NotImplementedException();
            public override void OnDisable() { }
            public override void OnEnable() { }
        }

        [Test]
        public void PluginExecutionConstraintBuilderTest()
        {
            var output = new PluginExecutionConstraintBuilder()
                .BeforePlugin("com.example.before-plugin")
                .BeforePlugin("com.example.before-plugin.optional", true)
                .BeforePlugin(typeof(TestPlugin))
                .AfterPlugin("com.example.after-plugin")
                .AfterPlugin("com.example.after-plugin.optional", true)
                .AfterPlugin(typeof(TestPlugin))
                .Build();

            AssertContainsDependency(output.beforeDependencies, DependencySource.Plugin, "com.example.before-plugin", false);
            AssertContainsDependency(output.beforeDependencies, DependencySource.Plugin, "com.example.before-plugin.optional", true);
            AssertContainsDependency(output.beforeDependencies, DependencySource.Plugin, typeof(TestPlugin).FullName, false);

            AssertContainsDependency(output.afterDependencies, DependencySource.Plugin, "com.example.after-plugin", false);
            AssertContainsDependency(output.afterDependencies, DependencySource.Plugin, "com.example.after-plugin.optional", true);
            AssertContainsDependency(output.afterDependencies, DependencySource.Plugin, typeof(TestPlugin).FullName, false);
        }
    }
}
