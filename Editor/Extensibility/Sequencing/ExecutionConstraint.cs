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
    /// Dependency
    /// </summary>
    public class Dependency<T>
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public T identifier;

        /// <summary>
        /// Whether this dependency is optional or not. If not optional, the dependency solving will fail if the dependency does not exist
        /// </summary>
        public bool optional;

        /// <summary>
        /// Constructs a new dependency
        /// </summary>
        public Dependency()
        {
            optional = false;
        }
    }

    /// <summary>
    /// Execution constraint
    /// </summary>
    public class ExecutionConstraint<T>
    {
        /// <summary>
        /// Before dependencies
        /// </summary>
        public List<Dependency<T>> beforeDependencies;

        /// <summary>
        /// After dependencies
        /// </summary>
        public List<Dependency<T>> afterDependencies;

        public ExecutionConstraint()
        {
            beforeDependencies = new List<Dependency<T>>();
            afterDependencies = new List<Dependency<T>>();
        }
    }

    /// <summary>
    /// Abstract base class for building execution constraints
    /// </summary>
    public abstract class ExecutionConstraintBuilder<T>
    {
        protected List<Dependency<T>> beforeDependencies;
        protected List<Dependency<T>> afterDependencies;

        /// <summary>
        /// Constructs a new execution constraint builder
        /// </summary>
        public ExecutionConstraintBuilder()
        {
            beforeDependencies = new List<Dependency<T>>();
            afterDependencies = new List<Dependency<T>>();
        }

        protected ExecutionConstraintBuilder<T> InnerBefore(T identifier, bool optional = false)
        {
            beforeDependencies.Add(new Dependency<T>()
            {
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        protected ExecutionConstraintBuilder<T> InnerAfter(T identifier, bool optional = false)
        {
            afterDependencies.Add(new Dependency<T>()
            {
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        /// <summary>
        /// Build the constraint
        /// </summary>
        /// <returns>ExecutionConstraint</returns>
        public ExecutionConstraint<T> Build()
        {
            return new ExecutionConstraint<T>()
            {
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies
            };
        }
    }
}
