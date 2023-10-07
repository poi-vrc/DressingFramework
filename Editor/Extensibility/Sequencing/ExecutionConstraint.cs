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

namespace Chocopoi.DressingFramework.Extensibility.Sequencing
{
    /// <summary>
    /// Dependency source. Determines where the dependency comes from.
    /// </summary>
    public enum DependencySource
    {
        /// <summary>
        /// Plugin dependency
        /// </summary>
        Plugin = 0,

        /// <summary>
        /// Cabinet hook dependency
        /// </summary>
        CabinetHook = 1,

        /// <summary>
        /// Wearable hook dependency
        /// </summary>
        WearableHook = 2,

        /// <summary>
        /// Cabinet module dependency
        /// </summary>
        CabinetModule = 3,

        /// <summary>
        /// Wearable module dependency
        /// </summary>
        WearableModule = 4,
    }

    /// <summary>
    /// Dependency. Defines the dependency source and identifier
    /// </summary>
    public class Dependency
    {
        /// <summary>
        /// Source of the dependency
        /// </summary>
        public DependencySource source;

        /// <summary>
        /// Identifier
        /// </summary>
        public string identifier;

        /// <summary>
        /// Whether this dependency is optional or not. If not optional, the dependency solving will fail if the dependency does not exist
        /// </summary>
        public bool optional;

        /// <summary>
        /// Constructs a new dependency
        /// </summary>
        public Dependency()
        {
            source = DependencySource.Plugin;
            identifier = null;
            optional = false;
        }
    }

    /// <summary>
    /// Execution constraint
    /// </summary>
    public class ExecutionConstraint
    {
        /// <summary>
        /// No constraints
        /// </summary>
        public static ExecutionConstraint Empty => new ExecutionConstraint();

        /// <summary>
        /// Before dependencies
        /// </summary>
        public List<Dependency> beforeDependencies;

        /// <summary>
        /// After dependencies
        /// </summary>
        public List<Dependency> afterDependencies;

        public ExecutionConstraint()
        {
            beforeDependencies = new List<Dependency>();
            afterDependencies = new List<Dependency>();
        }
    }

    /// <summary>
    /// Abstract base class for building execution constraints
    /// </summary>
    public abstract class ExecutionConstraintBuilder
    {
        protected List<Dependency> beforeDependencies;
        protected List<Dependency> afterDependencies;

        /// <summary>
        /// Constructs a new execution constraint builder
        /// </summary>
        public ExecutionConstraintBuilder()
        {
            beforeDependencies = new List<Dependency>();
            afterDependencies = new List<Dependency>();
        }

        protected ExecutionConstraintBuilder Before(DependencySource source, string identifier, bool optional = false)
        {
            beforeDependencies.Add(new Dependency()
            {
                source = source,
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        protected ExecutionConstraintBuilder After(DependencySource source, string identifier, bool optional = false)
        {
            afterDependencies.Add(new Dependency()
            {
                source = source,
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        /// <summary>
        /// Build the constraint
        /// </summary>
        /// <returns>ExecutionConstraint</returns>
        public ExecutionConstraint Build()
        {
            return new ExecutionConstraint()
            {
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies
            };
        }
    }

    /// <summary>
    /// Plugin execution constraint builder
    /// </summary>
    public class PluginExecutionConstraintBuilder : ExecutionConstraintBuilder
    {
        /// <summary>
        /// Schedule this before a specific plugin
        /// </summary>
        /// <param name="identifier">Plugin identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginExecutionConstraintBuilder BeforePlugin(string identifier, bool optional = false)
        {
            Before(DependencySource.Plugin, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific plugin. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Plugin type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginExecutionConstraintBuilder BeforePlugin(Type type, bool optional = false)
        {
            Before(DependencySource.Plugin, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific plugin
        /// </summary>
        /// <param name="identifier">Plugin identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginExecutionConstraintBuilder AfterPlugin(string identifier, bool optional = false)
        {
            After(DependencySource.Plugin, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific plugin. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Plugin type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginExecutionConstraintBuilder AfterPlugin(Type type, bool optional = false)
        {
            After(DependencySource.Plugin, type.FullName, optional);
            return this;
        }
    }
}
