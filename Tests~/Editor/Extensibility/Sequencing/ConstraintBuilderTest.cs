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
        private class TestPass : BuildPass
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
        public void BuildConstraintBuilderTest()
        {
            var output = new BuildConstraintBuilder(BuildStage.Generation)
                .BeforePass("com.example.before-hook")
                .BeforePass("com.example.before-hook.optional", true)
                .BeforePass(typeof(TestPass))
                .BeforeRuntimePass("com.example.before-runtime-hook")
                .BeforeRuntimePass("com.example.before-runtime-hook.optional", true)
                .BeforeRuntimePass(typeof(TestPass))
                .BeforePass(typeof(TestPass))
                .AfterPass("com.example.after-hook")
                .AfterPass("com.example.after-hook.optional", true)
                .AfterPass(typeof(TestPass))
                .AfterRuntimePass("com.example.after-runtime-hook")
                .AfterRuntimePass("com.example.after-runtime-hook.optional", true)
                .AfterRuntimePass(typeof(TestPass))
                .Build();

            Assert.AreEqual(BuildStage.Generation, output.stage);

            AssertContainsDependency(output.beforeDependencies, "com.example.before-hook", false);
            AssertContainsDependency(output.beforeDependencies, "com.example.before-hook.optional", true);
            AssertContainsDependency(output.beforeDependencies, typeof(TestPass).FullName, false);
            AssertContainsDependency(output.beforeRuntimePasses, "com.example.before-runtime-hook", false);
            AssertContainsDependency(output.beforeRuntimePasses, "com.example.before-runtime-hook.optional", true);
            AssertContainsDependency(output.beforeRuntimePasses, typeof(TestPass).FullName, false);

            AssertContainsDependency(output.afterDependencies, "com.example.after-hook", false);
            AssertContainsDependency(output.afterDependencies, "com.example.after-hook.optional", true);
            AssertContainsDependency(output.afterDependencies, typeof(TestPass).FullName, false);
            AssertContainsDependency(output.afterRuntimePasses, "com.example.after-runtime-hook", false);
            AssertContainsDependency(output.afterRuntimePasses, "com.example.after-runtime-hook.optional", true);
            AssertContainsDependency(output.afterRuntimePasses, typeof(TestPass).FullName, false);
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
