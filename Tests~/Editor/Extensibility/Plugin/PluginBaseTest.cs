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
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable.Modules;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Plugin
{
    public class PluginBaseTest : EditorTestBase
    {
        private class TestCabinetHook : CabinetHookBase
        {
            public override string FriendlyName => "Test cabinet hook";
            public override CabinetApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Pre, CabinetHookStageRunOrder.Before).Build();
            public override bool Invoke(ApplyCabinetContext cabCtx) => true;
        }

        private class TestCabinetModuleProvider : CabinetModuleProviderBase
        {
            public override string Identifier => GetType().FullName;
            public override string FriendlyName => "Test cabinet module";
            public override CabinetApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Pre, CabinetHookStageRunOrder.Before).Build();

            public override bool AllowMultiple => false;
            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx, System.Collections.ObjectModel.ReadOnlyCollection<CabinetModule> modules, bool isPreview) => true;
            public override IModuleConfig NewModuleConfig() => throw new System.NotImplementedException();
        }
        private class TestWearableHook : WearableHookBase
        {
            public override string FriendlyName => "Test cabinet hook";

            public override WearableApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Pre).Build();

            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx) => true;
        }

        private class TestWearableModuleProvider : WearableModuleProviderBase
        {
            public override string Identifier => GetType().FullName;
            public override string FriendlyName => "Test wearable module";

            public override bool AllowMultiple => false;

            public override WearableApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Pre).Build();
            public override IModuleConfig DeserializeModuleConfig(JObject jObject) => throw new System.NotImplementedException();
            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview) => true;

            public override IModuleConfig NewModuleConfig() => throw new System.NotImplementedException();
        }

        private class TestRegisterPlugin : PluginBase
        {
            public override string FriendlyName => "Test register plugin";
            public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

            public override void OnEnable()
            {
                RegisterCabinetHook(new TestCabinetHook());
                RegisterCabinetModuleProvider(new TestCabinetModuleProvider());
                RegisterWearableHook(new TestWearableHook());
                RegisterWearableModuleProvider(new TestWearableModuleProvider());
            }

            public override void OnDisable() { }
        }

        [Test]
        public void RegisterTest()
        {
            var plugin = new TestRegisterPlugin();
            plugin.OnEnable();

            Assert.AreEqual(1, plugin.GetAllCabinetHooks().Count);
            Assert.AreEqual(1, plugin.GetAllCabinetModuleProviders().Count);
            Assert.AreEqual(1, plugin.GetAllWearableHooks().Count);
            Assert.AreEqual(1, plugin.GetAllWearableModuleProviders().Count);
        }
    }
}
