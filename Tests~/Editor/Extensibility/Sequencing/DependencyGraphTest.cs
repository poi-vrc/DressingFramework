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
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Sequencing
{
    public class DependencyGraphTest : EditorTestBase
    {
        private static void AssertListContent(DependencySource expectedSource, string[] expected, List<Tuple<DependencySource, string>> topOrder)
        {
            Assert.AreEqual(expected.Length, topOrder.Count);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(DependencySource.Plugin, topOrder[i].Item1, $"expected {i} is Plugin instead of {topOrder[i].Item1}");
                Assert.AreEqual(expected[i], topOrder[i].Item2, $"expected {i} is {expected[i]} instead of {topOrder[i].Item2}");
            }
        }

        [Test]
        public void ResolvedGraph1Test()
        {
            //
            // e -> b -> c <
            //       \     /
            //        > a <- d
            //
            var graph = new DependencyGraph();
            graph.Add(DependencySource.Plugin, "plugin-a",
                new PluginExecutionConstraintBuilder()
                    .BeforePlugin("plugin-b")
                    .BeforePlugin("plugin-c")
                    .AfterPlugin("plugin-d")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-b",
                new PluginExecutionConstraintBuilder()
                    .BeforePlugin("plugin-c")
                    .AfterPlugin("plugin-e")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-c", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-d", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-e", ExecutionConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(DependencySource.Plugin, new string[] { "plugin-d", "plugin-e", "plugin-a", "plugin-b", "plugin-c" }, topOrder);
        }

        [Test]
        public void ResolvedGraph2Test()
        {
            //
            // e <- b <- c -
            //       \     /
            //        > a <
            //          |
            //          -> d
            //
            var graph = new DependencyGraph();
            graph.Add(DependencySource.Plugin, "plugin-a",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-b")
                    .AfterPlugin("plugin-c")
                    .BeforePlugin("plugin-d")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-b",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-c")
                    .BeforePlugin("plugin-e")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-c", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-d", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-e", ExecutionConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(DependencySource.Plugin, new string[] { "plugin-c", "plugin-b", "plugin-a", "plugin-e", "plugin-d" }, topOrder);
        }

        [Test]
        public void OptionalDependencyTest()
        {
            // c,d,e are unresolved (we did not add these vertices)
            // but c,d are set to be optional, so the behaviour should be the same as graph 2
            //
            // e <- b <- c -
            //       \     /
            //        > a <
            //          |
            //          -> d
            //
            var graph = new DependencyGraph();
            graph.Add(DependencySource.Plugin, "plugin-a",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-b")
                    .AfterPlugin("plugin-c", true)
                    .BeforePlugin("plugin-d", true)
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-b",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-c", true)
                    .BeforePlugin("plugin-e")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-e", ExecutionConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(DependencySource.Plugin, new string[] { "plugin-c", "plugin-b", "plugin-a", "plugin-e", "plugin-d" }, topOrder);
        }

        [Test]
        public void UnresolvedGraphTest()
        {
            // c,d,e are unresolved (we did not add these vertices)
            //
            // e <- b <- c -
            //       \     /
            //        > a <
            //          |
            //          -> d
            //
            var graph = new DependencyGraph();
            graph.Add(DependencySource.Plugin, "plugin-a",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-b")
                    .AfterPlugin("plugin-c")
                    .BeforePlugin("plugin-d")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-b",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-c")
                    .BeforePlugin("plugin-e")
                    .Build());

            Assert.False(graph.IsResolved());
        }

        [Test]
        public void CyclicGraphTest()
        {
            // a cycle exists between b,c,a, cannot perform sort
            //
            // e <- b <- c <
            //       \      /
            //        > a  -
            //          |
            //          -> d
            //
            var graph = new DependencyGraph();
            graph.Add(DependencySource.Plugin, "plugin-a",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-b")
                    .BeforePlugin("plugin-c")
                    .BeforePlugin("plugin-d")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-b",
                new PluginExecutionConstraintBuilder()
                    .AfterPlugin("plugin-c")
                    .BeforePlugin("plugin-e")
                    .Build());
            graph.Add(DependencySource.Plugin, "plugin-c", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-d", ExecutionConstraint.Empty);
            graph.Add(DependencySource.Plugin, "plugin-e", ExecutionConstraint.Empty);

            Assert.True(graph.IsResolved());
            Assert.Null(graph.Sort());
        }
    }
}
