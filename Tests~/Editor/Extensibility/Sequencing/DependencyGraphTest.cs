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
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Extensibility.Sequencing
{
    public class DependencyGraphTest : EditorTestBase
    {
        private static void AssertListContent<T>(T[] expected, List<T> topOrder)
        {
            Assert.AreEqual(expected.Length, topOrder.Count);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], topOrder[i], $"expected {i} is {expected[i]} instead of {topOrder[i]}");
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
            var graph = new DependencyGraph<string>();
            graph.Add("plugin-a",
                new PluginConstraintBuilder()
                    .Before("plugin-b")
                    .Before("plugin-c")
                    .After("plugin-d")
                    .Build());
            graph.Add("plugin-b",
                new PluginConstraintBuilder()
                    .Before("plugin-c")
                    .After("plugin-e")
                    .Build());
            graph.Add("plugin-c", PluginConstraint.Empty);
            graph.Add("plugin-d", PluginConstraint.Empty);
            graph.Add("plugin-e", PluginConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(new string[] { "plugin-d", "plugin-e", "plugin-a", "plugin-b", "plugin-c" }, topOrder);
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
            var graph = new DependencyGraph<string>();
            graph.Add("plugin-a",
                new PluginConstraintBuilder()
                    .After("plugin-b")
                    .After("plugin-c")
                    .Before("plugin-d")
                    .Build());
            graph.Add("plugin-b",
                new PluginConstraintBuilder()
                    .After("plugin-c")
                    .Before("plugin-e")
                    .Build());
            graph.Add("plugin-c", PluginConstraint.Empty);
            graph.Add("plugin-d", PluginConstraint.Empty);
            graph.Add("plugin-e", PluginConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(new string[] { "plugin-c", "plugin-b", "plugin-a", "plugin-e", "plugin-d" }, topOrder);
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
            var graph = new DependencyGraph<string>();
            graph.Add("plugin-a",
                new PluginConstraintBuilder()
                    .After("plugin-b")
                    .After("plugin-c", true)
                    .Before("plugin-d", true)
                    .Build());
            graph.Add("plugin-b",
                new PluginConstraintBuilder()
                    .After("plugin-c", true)
                    .Before("plugin-e")
                    .Build());
            graph.Add("plugin-e", PluginConstraint.Empty);

            Assert.True(graph.IsResolved());

            var topOrder = graph.Sort();
            Assert.NotNull(topOrder);

            AssertListContent(new string[] { "plugin-c", "plugin-b", "plugin-a", "plugin-e", "plugin-d" }, topOrder);
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
            var graph = new DependencyGraph<string>();
            graph.Add("plugin-a",
                new PluginConstraintBuilder()
                    .After("plugin-b")
                    .After("plugin-c")
                    .Before("plugin-d")
                    .Build());
            graph.Add("plugin-b",
                new PluginConstraintBuilder()
                    .After("plugin-c")
                    .Before("plugin-e")
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
            var graph = new DependencyGraph<string>();
            graph.Add("plugin-a",
                new PluginConstraintBuilder()
                    .After("plugin-b")
                    .Before("plugin-c")
                    .Before("plugin-d")
                    .Build());
            graph.Add("plugin-b",
                new PluginConstraintBuilder()
                    .After("plugin-c")
                    .Before("plugin-e")
                    .Build());
            graph.Add("plugin-c", PluginConstraint.Empty);
            graph.Add("plugin-d", PluginConstraint.Empty);
            graph.Add("plugin-e", PluginConstraint.Empty);

            Assert.True(graph.IsResolved());
            Assert.Null(graph.Sort());
        }
    }
}
