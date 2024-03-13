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

using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility
{
    public class PluginTest : EditorTestBase
    {
        private class TestPass : BuildPass
        {
            public override string FriendlyName => "Test pass";
            public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Pre).Build();
            public override bool Invoke(Context cabCtx) => true;
        }

        private class TestRegisterPlugin : Plugin
        {
            public override string FriendlyName => "Test register plugin";
            public override PluginConstraint Constraint => PluginConstraint.Empty;

            public override void OnEnable()
            {
                RegisterBuildPass(new TestPass());
            }

            public override void OnDisable() { }
        }

        [Test]
        public void RegisterTest()
        {
            var plugin = new TestRegisterPlugin();
            plugin.OnEnable();

            Assert.AreEqual(1, plugin.GetBuildPassesAtStage(BuildRuntime.DK, BuildStage.Pre).Count);
        }
    }
}
