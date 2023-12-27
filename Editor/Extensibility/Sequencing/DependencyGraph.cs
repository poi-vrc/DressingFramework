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

namespace Chocopoi.DressingFramework.Extensibility.Sequencing
{
    /// <summary>
    /// Dependency graph
    /// </summary>
    public class DependencyGraph<T>
    {
        private class Vertex
        {
            public bool resolved;
            public bool optional;

            public Vertex()
            {
                resolved = false;
                optional = false;
            }
        }

        private readonly Dictionary<T, Vertex> _vertices;
        private readonly Dictionary<T, List<T>> _edges;

        /// <summary>
        /// Constructs a new dependency graph
        /// </summary>
        public DependencyGraph()
        {
            _vertices = new Dictionary<T, Vertex>();
            _edges = new Dictionary<T, List<T>>();
        }

        /// <summary>
        /// Add a vertex and edges to the graph
        /// </summary>
        /// <param name="source">Dependency source</param>
        /// <param name="identifier">Identifier</param>
        /// <param name="constraint">Constraint</param>
        public void Add(T identifier, ExecutionConstraint<T> constraint)
        {
            // add vertex if not exist
            if (!_vertices.TryGetValue(identifier, out var ourVertex))
            {
                _vertices[identifier] = ourVertex = new Vertex();
            }
            ourVertex.resolved = true;

            // get our edge list
            if (!_edges.TryGetValue(identifier, out var ourEdges))
            {
                _edges[identifier] = ourEdges = new List<T>();
            }

            // add before dependences
            foreach (var dependency in constraint.beforeDependencies)
            {
                if (!_vertices.TryGetValue(dependency.identifier, out var dependencyVertex))
                {
                    _vertices[dependency.identifier] = dependencyVertex = new Vertex();
                    dependencyVertex.optional = dependency.optional;
                }
                dependencyVertex.optional &= dependency.optional;

                ourEdges.Add(dependency.identifier);
            }

            // add after dependencies
            foreach (var dependency in constraint.afterDependencies)
            {
                if (!_vertices.TryGetValue(dependency.identifier, out var dependencyVertex))
                {
                    _vertices[dependency.identifier] = dependencyVertex = new Vertex();
                    dependencyVertex.optional = dependency.optional;
                }
                dependencyVertex.optional &= dependency.optional;

                if (!_edges.TryGetValue(dependency.identifier, out var dependencyEdges))
                {
                    _edges[dependency.identifier] = dependencyEdges = new List<T>();
                }
                dependencyEdges.Add(identifier);
            }
        }

        /// <summary>
        /// Check whether the graph is resolved, that all required dependencies/vertices exist.
        /// </summary>
        /// <returns>Resolved</returns>
        public bool IsResolved()
        {
            foreach (var vertex in _vertices.Values)
            {
                if (!vertex.optional && !vertex.resolved)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Runs a topological sort to generate a top order of execution
        /// </summary>
        /// <returns>Top order list</returns>
        public List<T> Sort()
        {
            // just... a common topological sort algorithm learnt in computer science lectures
            // reference: https://www.geeksforgeeks.org/topological-sorting-indegree-based-solution/

            var inDegrees = new Dictionary<T, int>();

            // traverse edges to fill in-degrees
            foreach (var u in _vertices.Keys)
            {
                // init to zero if not exist
                if (!inDegrees.ContainsKey(u)) inDegrees[u] = 0;
                if (!_edges.ContainsKey(u)) continue;

                foreach (var v in _edges[u])
                {
                    if (!inDegrees.ContainsKey(v)) inDegrees[v] = 0;
                    inDegrees[v]++;
                }
            }

            // add in-degree zero vertices to queue
            var queue = new Queue<T>();
            foreach (var u in _vertices.Keys)
            {
                if (inDegrees[u] == 0)
                {
                    queue.Enqueue(u);
                }
            }

            var visitedCount = 0;
            var topOrder = new List<T>();
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                topOrder.Add(u);

                if (_edges.ContainsKey(u))
                {
                    foreach (var v in _edges[u])
                    {
                        if (--inDegrees[v] == 0)
                        {
                            queue.Enqueue(v);
                        }
                    }
                }

                visitedCount++;
            }

            if (visitedCount != _vertices.Count)
            {
                // cyclic dependency cannot resolve
                return null;
            }

            return topOrder;
        }
    }
}
