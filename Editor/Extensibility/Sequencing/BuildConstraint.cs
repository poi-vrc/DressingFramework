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
        /// Preparation stage. Scanning, cloning etc. are done in this stage.
        /// </summary>
        Preparation = 1,

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
        public List<Dependency<string>> beforeRuntimePasses;

        /// <summary>
        /// After runtime hooks
        /// </summary>
        public List<Dependency<string>> afterRuntimePasses;

        public BuildConstraint()
        {
            beforeRuntimePasses = new List<Dependency<string>>();
            afterRuntimePasses = new List<Dependency<string>>();
        }
    }

    /// <summary>
    /// Build constraint builder
    /// </summary>
    public class BuildConstraintBuilder : ExecutionConstraintBuilder<string>
    {
        protected List<Dependency<string>> beforeRuntimePasses;
        protected List<Dependency<string>> afterRuntimePasses;

        private readonly BuildStage _stage;

        /// <summary>
        /// Constructs a new builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        public BuildConstraintBuilder(BuildStage stage)
        {
            beforeRuntimePasses = new List<Dependency<string>>();
            afterRuntimePasses = new List<Dependency<string>>();
            _stage = stage;
        }

        public BuildConstraintBuilder BeforePass<T>(bool optional = false)
        {
            return BeforePass(typeof(T), optional);
        }

        /// <summary>
        /// Schedule this before a specific framework hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforePass(Type type, bool optional = false)
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
        public BuildConstraintBuilder BeforePass(string identifier, bool optional = false)
        {
            InnerBefore(identifier, optional);
            return this;
        }

        public BuildConstraintBuilder AfterPass<T>(bool optional = false)
        {
            return AfterPass(typeof(T), optional);
        }

        /// <summary>
        /// Schedule this after a specific framework hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterPass(Type type, bool optional = false)
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
        public BuildConstraintBuilder AfterPass(string identifier, bool optional = false)
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
        public BuildConstraintBuilder BeforeRuntimePass(Type type, bool optional = false)
        {
            InnerBeforeRuntimePass(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. 
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder BeforeRuntimePass(string identifier, bool optional = false)
        {
            InnerBeforeRuntimePass(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterRuntimePass(Type type, bool optional = false)
        {
            InnerAfterRuntimePass(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific runtime hook. Optionalily is runtime-dependent and not guaranteed. 
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public BuildConstraintBuilder AfterRuntimePass(string identifier, bool optional = false)
        {
            InnerAfterRuntimePass(identifier, optional);
            return this;
        }

        protected BuildConstraintBuilder InnerBeforeRuntimePass(string identifier, bool optional = false)
        {
            beforeRuntimePasses.Add(new Dependency<string>()
            {
                identifier = identifier,
                optional = optional
            });
            return this;
        }

        protected BuildConstraintBuilder InnerAfterRuntimePass(string identifier, bool optional = false)
        {
            afterRuntimePasses.Add(new Dependency<string>()
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
                beforeRuntimePasses = beforeRuntimePasses,
                afterRuntimePasses = afterRuntimePasses
            };
        }
    }
}
