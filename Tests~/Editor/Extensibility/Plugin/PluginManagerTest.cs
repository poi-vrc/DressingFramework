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

using System;
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
using NUnit.Framework.Internal;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Plugin
{
    public class PluginManagerTest : EditorTestBase
    {
        private class TestCabinetHook : CabinetHookBase
        {
            public override string FriendlyName => "Test cabinet hook";

            public override CabinetApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Analyzing, CabinetHookStageRunOrder.After).Build();

            public override bool Invoke(ApplyCabinetContext cabCtx) => true;
        }

        private class TestCabinetModuleProvider : CabinetModuleProviderBase
        {
            public override CabinetApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Transpose, CabinetHookStageRunOrder.Before).Build();
            public override string Identifier => GetType().FullName;
            public override string FriendlyName => "Test cabinet module";
            public override bool AllowMultiple => false;

            public override IModuleConfig DeserializeModuleConfig(JObject jObject)
            {
                throw new System.NotImplementedException();
            }

            public override bool Invoke(ApplyCabinetContext cabCtx, ReadOnlyCollection<CabinetModule> modules, bool isPreview) => true;

            public override IModuleConfig NewModuleConfig()
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestWearableHook : WearableHookBase
        {
            public override string FriendlyName => "Test wearable hook";
            public override WearableApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Optimization).Build();
            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx) => true;
        }

        private class TestWearableModuleProvider : WearableModuleProviderBase
        {
            public override string FriendlyName => "Test wearable hook";
            public override WearableApplyConstraint Constraint => ApplyAtStage(CabinetApplyStage.Integration).Build();
            public override string Identifier => GetType().FullName;
            public override bool AllowMultiple => false;

            public override IModuleConfig DeserializeModuleConfig(JObject jObject)
            {
                throw new System.NotImplementedException();
            }

            public override bool Invoke(ApplyCabinetContext cabCtx, ApplyWearableContext wearCtx, ReadOnlyCollection<WearableModule> modules, bool isPreview) => true;

            public override IModuleConfig NewModuleConfig()
            {
                throw new System.NotImplementedException();
            }
        }

        private class TestFrameworkEventHandler : FrameworkEventAdapter { }

        private class TestPluginManagerPlugin : PluginBase
        {
            public override string FriendlyName => "Test plugin manager plugin";
            public override ExecutionConstraint Constraint => ExecutionConstraint.Empty;

            public override void OnEnable()
            {
                RegisterCabinetHook(new TestCabinetHook());
                RegisterCabinetModuleProvider(new TestCabinetModuleProvider());
                RegisterWearableHook(new TestWearableHook());
                RegisterWearableModuleProvider(new TestWearableModuleProvider());
                RegisterFrameworkEventHandler(new TestFrameworkEventHandler());
            }

            public override void OnDisable() { }
        }

        private static void AssertHasType<T>(List<T> hooks, Type type) where T : class
        {
            foreach (var hook in hooks)
            {
                if (hook.GetType() == type)
                {
                    return;
                }
            }
            Assert.Fail("Does not have type " + type.FullName);
        }

        [Test]
        public void CabinetHooksAtStageTest()
        {
            AssertHasType(PluginManager.Instance.GetCabinetHooksAtStage(CabinetApplyStage.Analyzing, CabinetHookStageRunOrder.After), typeof(TestCabinetHook));
            AssertHasType(PluginManager.Instance.GetCabinetModulesAtStage(CabinetApplyStage.Transpose, CabinetHookStageRunOrder.Before), typeof(TestCabinetModuleProvider));
        }

        [Test]
        public void WearableHooksAtStageTest()
        {
            AssertHasType(PluginManager.Instance.GetWearableHooksAtStage(CabinetApplyStage.Optimization), typeof(TestWearableHook));
            AssertHasType(PluginManager.Instance.GetWearableModulesAtStage(CabinetApplyStage.Integration), typeof(TestWearableModuleProvider));
        }

        [Test]
        public void GetAllModuleProvidersTest()
        {
            // the plugin manager is shared so we can only check if the one we added are there
            AssertHasType(PluginManager.Instance.GetAllCabinetModuleProviders(), typeof(TestCabinetModuleProvider));
            AssertHasType(PluginManager.Instance.GetAllWearableModuleProviders(), typeof(TestWearableModuleProvider));
        }

        [Test]
        public void GetAllFrameworkEventHandlers()
        {
            AssertHasType(PluginManager.Instance.GetAllFrameworkEventHandlers(), typeof(TestFrameworkEventHandler));
        }

        [Test]
        public void GetCabinetModuleProviderTest()
        {
            var type = typeof(TestCabinetModuleProvider);
            Assert.IsInstanceOf(type, PluginManager.Instance.GetCabinetModuleProvider(type.FullName));
        }

        [Test]
        public void GetWearableModuleProviderTest()
        {
            var type = typeof(TestWearableModuleProvider);
            Assert.IsInstanceOf(type, PluginManager.Instance.GetWearableModuleProvider(type.FullName));
        }
    }
}
