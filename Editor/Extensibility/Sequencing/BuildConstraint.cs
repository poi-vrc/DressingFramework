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
    /// Build stage
    /// </summary>
    public enum BuildStage
    {
        /// <summary>
        /// Generic pre-stage. The earliest stage to execute hooks.
        /// You are preferred to categorize your hook instead of using this generic stage.
        /// </summary>
        Pre = 0,

        /// <summary>
        /// Analyze stage. Scanning etc. are done in this stage.
        /// </summary>
        Analyze = 1,

        /// <summary>
        /// Generation stage. Component generation, cloning are done in this stage.
        /// </summary>
        Generation = 2,

        /// <summary>
        /// Transpose stage. Mapping, animation etc. general stuff are done in this stage.
        /// </summary>
        Transpose = 3,

        /// <summary>
        /// Optimization stage
        /// </summary>
        Optimization = 4,

        /// <summary>
        /// Generic post-stage. The latest stage to execute hooks.
        /// You are preferred to categorize your hook instead of using this generic stage.
        /// </summary>
        Post = 5
    }

    /// <summary>
    /// Build constraint
    /// </summary>
    public class BuildConstraint : ExecutionConstraint<string>
    {
        /// <summary>
        /// Stage to execute
        /// </summary>
        public BuildStage stage;

        /// <summary>
        /// Before runtime hooks
        /// </summary>
        public List<Dependency<string>> beforeRuntimeHooks;

        /// <summary>
        /// After runtime hooks
        /// </summary>
        public List<Dependency<string>> afterRuntimeHooks;

        public BuildConstraint()
        {
            beforeRuntimeHooks = new List<Dependency<string>>();
            afterRuntimeHooks = new List<Dependency<string>>();
        }
    }

    /// <summary>
    /// Build constraint builder
    /// </summary>
    public class BuildConstraintBuilder : ExecutionConstraintBuilder<string>
    {
        protected List<Dependency<string>> beforeRuntimeHooks;
        protected List<Dependency<string>> afterRuntimeHooks;

        private readonly BuildStage _stage;

        /// <summary>
        /// Constructs a new builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        public BuildConstraintBuilder(BuildStage stage)
        {
            beforeRuntimeHooks = new List<Dependency<string>>();
            afterRuntimeHooks = new List<Dependency<string>>();
            _stage = stage;
        }

        /// <summary>
        /// Schedule this before a specific framework hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforeHook(Type type, bool optional = false)
        {
            InnerBefore(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific framework hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforeHook(string identifier, bool optional = false)
        {
            InnerBefore(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific framework hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterHook(Type type, bool optional = false)
        {
            InnerAfter(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific framework hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterHook(string identifier, bool optional = false)
        {
            InnerAfter(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforeRuntimeHook(Type type, bool optional = false)
        {
            InnerBeforeRuntimeHook(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. 
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforeRuntimeHook(string identifier, bool optional = false)
        {
            InnerBeforeRuntimeHook(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterRuntimeHook(Type type, bool optional = false)
        {
            InnerAfterRuntimeHook(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. 
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterRuntimeHook(string identifier, bool optional = false)
        {
            InnerAfterRuntimeHook(identifier, optional);
            return this;
        }

        protected BuildConstraintBuilder InnerBeforeRuntimeHook(string identifier, bool optional = false)
        {
            beforeRuntimeHooks.Add(new Dependency<string>()
            {
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        protected BuildConstraintBuilder InnerAfterRuntimeHook(string identifier, bool optional = false)
        {
            afterRuntimeHooks.Add(new Dependency<string>()
            {
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        /// <summary>
        /// Build this constraint
        /// </summary>
        /// <returns>BuildConstraint</returns>
        public new BuildConstraint Build()
        {
            return new BuildConstraint()
            {
                stage = _stage,
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies,
            };
        }
    }
}
