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
using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Sequencing
{
    public class ConstraintBuilderTest : EditorTestBase
    {
        private class TestHook : BuildPass
        {
            public override string FriendlyName => throw new NotImplementedException();
            public override BuildConstraint Constraint => throw new NotImplementedException();
            public override bool Invoke(Context ctx) => throw new NotImplementedException();
        }

        private static void AssertContainsDependency(List<Dependency<string>> dependencies, string identifier, bool optional)
        {
            foreach (var dependency in dependencies)
            {
                if (dependency.identifier == identifier)
                {
                    Assert.AreEqual(optional, dependency.optional, $"Dependency {identifier} optional mismatch");
                    return;
                }
            }
            Assert.Fail($"Dependency not found {identifier} (Optional: {optional})");
        }

        [Test]
        public void ApplyConstraintBuilderTest()
        {
            var output = new BuildConstraintBuilder(BuildStage.Generation)
                .BeforeHook("com.example.before-hook")
                .BeforeHook("com.example.before-hook.optional", true)
                .BeforeHook(typeof(TestHook))
                .BeforeRuntimeHook("com.example.before-runtime-hook")
                .BeforeRuntimeHook("com.example.before-runtime-hook.optional", true)
                .BeforeRuntimeHook(typeof(TestHook))
                .BeforeHook(typeof(TestHook))
                .AfterHook("com.example.after-hook")
                .AfterHook("com.example.after-hook.optional", true)
                .AfterHook(typeof(TestHook))
                .AfterRuntimeHook("com.example.after-runtime-hook")
                .AfterRuntimeHook("com.example.after-runtime-hook.optional", true)
                .AfterRuntimeHook(typeof(TestHook))
                .Build();

            Assert.AreEqual(BuildStage.Generation, output.stage);

            AssertContainsDependency(output.beforeDependencies, "com.example.before-hook", false);
            AssertContainsDependency(output.beforeDependencies, "com.example.before-hook.optional", true);
            AssertContainsDependency(output.beforeDependencies, typeof(TestHook).FullName, false);
            AssertContainsDependency(output.beforeRuntimeHooks, "com.example.before-runtime-hook", false);
            AssertContainsDependency(output.beforeRuntimeHooks, "com.example.before-runtime-hook.optional", true);
            AssertContainsDependency(output.beforeRuntimeHooks, typeof(TestHook).FullName, false);

            AssertContainsDependency(output.afterDependencies, "com.example.after-hook", false);
            AssertContainsDependency(output.afterDependencies, "com.example.after-hook.optional", true);
            AssertContainsDependency(output.afterDependencies, typeof(TestHook).FullName, false);
            AssertContainsDependency(output.afterRuntimeHooks, "com.example.after-runtime-hook", false);
            AssertContainsDependency(output.afterRuntimeHooks, "com.example.after-runtime-hook.optional", true);
            AssertContainsDependency(output.afterRuntimeHooks, typeof(TestHook).FullName, false);
        }

        private class TestPlugin : Plugin
        {
            public override string FriendlyName => "Test constraint builder test plugin";
            public override PluginConstraint Constraint => throw new NotImplementedException();
            public override void OnDisable() { }
            public override void OnEnable() { }
        }

        [Test]
        public void PluginExecutionConstraintBuilderTest()
        {
            var output = new PluginConstraintBuilder()
                .Before("com.example.before-plugin")
                .Before("com.example.before-plugin.optional", true)
                .Before(typeof(TestPlugin))
                .After("com.example.after-plugin")
                .After("com.example.after-plugin.optional", true)
                .After(typeof(TestPlugin))
                .Build();

            AssertContainsDependency(output.beforeDependencies, "com.example.before-plugin", false);
            AssertContainsDependency(output.beforeDependencies, "com.example.before-plugin.optional", true);
            AssertContainsDependency(output.beforeDependencies, typeof(TestPlugin).FullName, false);

            AssertContainsDependency(output.afterDependencies, "com.example.after-plugin", false);
            AssertContainsDependency(output.afterDependencies, "com.example.after-plugin.optional", true);
            AssertContainsDependency(output.afterDependencies, typeof(TestPlugin).FullName, false);
        }
    }
}
