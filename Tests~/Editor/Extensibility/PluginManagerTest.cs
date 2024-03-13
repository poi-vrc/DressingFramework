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
using NUnit.Framework.Internal;

namespace Chocopoi.DressingFramework.Tests.Extensibility
{
    public class PluginManagerTest : EditorTestBase
    {
        private class TestPass : BuildPass
        {
            public override string FriendlyName => "Test pass";

            public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Generation).Build();

            public override bool Invoke(Context ctx) => true;
        }

        private class TestPluginManagerPlugin : Plugin
        {
            public override string FriendlyName => "Test plugin manager plugin";
            public override PluginConstraint Constraint => PluginConstraint.Empty;

            public override void OnEnable()
            {
                RegisterBuildPass(new TestPass());
            }

            public override void OnDisable() { }
        }

        private static void AssertHasType<T>(List<T> passes, Type type) where T : class
        {
            foreach (var pass in passes)
            {
                if (pass.GetType() == type)
                {
                    return;
                }
            }
            Assert.Fail("Does not have type " + type.FullName);
        }

        [Test]
        public void PassesAtStageTest()
        {
            AssertHasType(new PluginManager().GetBuildPassesAtStage(BuildRuntime.DK, BuildStage.Generation), typeof(TestPass));
        }
    }
}
