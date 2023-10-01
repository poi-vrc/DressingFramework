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

namespace Chocopoi.DressingFramework.Extensibility.Sequencing
{
    /// <summary>
    /// Cabinet apply stage
    /// </summary>
    public enum CabinetApplyStage
    {
        /// <summary>
        /// Generic pre-stage. The earliest stage to execute hooks.
        /// You are preferred to categorize your hook instead of using this generic stage.
        /// </summary>
        Pre = 0,

        /// <summary>
        /// Analyzing stage. Scanning, generation, cloning are done in this stage.
        /// </summary>
        Analyzing = 1,

        /// <summary>
        /// Transpose stage. Mapping, animation etc. general stuff are done in this stage.
        /// </summary>
        Transpose = 2,

        /// <summary>
        /// Integration stage. Integration-specific hooks and modules are invoked in this stage.
        /// </summary>
        Integration = 3,

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
    /// Cabinet hook stage run order
    /// </summary>
    public enum CabinetHookStageRunOrder
    {
        /// <summary>
        /// Before stage
        /// </summary>
        Before = 0,

        /// <summary>
        /// After stage
        /// </summary>
        After = 1,
    }

    /// <summary>
    /// Cabinet apply constraint
    /// </summary>
    public class CabinetApplyConstraint : ExecutionConstraint
    {
        /// <summary>
        /// Stage to execute
        /// </summary>
        public CabinetApplyStage stage;

        /// <summary>
        /// Run this hook before or after the stage
        /// </summary>
        public CabinetHookStageRunOrder order;
    }

    /// <summary>
    /// Wearable apply constraint builder
    /// </summary>
    public class CabinetApplyConstraintBuilder : ExecutionConstraintBuilder
    {
        private readonly CabinetApplyStage _stage;
        private readonly CabinetHookStageRunOrder _order;

        /// <summary>
        /// Constructs a new builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        /// <param name="order">Run this hook before or after the stage</param>
        public CabinetApplyConstraintBuilder(CabinetApplyStage stage, CabinetHookStageRunOrder order)
        {
            _stage = stage;
            _order = order;
        }

        /// <summary>
        /// Schedule this before a specific cabinet hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder BeforeCabinetHook(Type type, bool optional = false)
        {
            Before(DependencySource.CabinetHook, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific cabinet hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder BeforeCabinetHook(string identifier, bool optional = false)
        {
            Before(DependencySource.CabinetHook, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific cabinet hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder AfterCabinetHook(Type type, bool optional = false)
        {
            After(DependencySource.CabinetHook, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific cabinet hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder AfterCabinetHook(string identifier, bool optional = false)
        {
            After(DependencySource.CabinetHook, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific cabinet module. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder BeforeCabinetModule(Type type, bool optional = false)
        {
            Before(DependencySource.CabinetModule, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific cabinet module
        /// </summary>
        /// <param name="identifier">Cabinet module identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder BeforeCabinetModule(string identifier, bool optional = false)
        {
            Before(DependencySource.CabinetModule, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific cabinet module. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder AfterCabinetModule(Type type, bool optional = false)
        {
            After(DependencySource.CabinetModule, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific cabinet module
        /// </summary>
        /// <param name="identifier">Cabinet module identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public CabinetApplyConstraintBuilder AfterCabinetModule(string identifier, bool optional = false)
        {
            After(DependencySource.CabinetModule, identifier, optional);
            return this;
        }

        /// <summary>
        /// Build this cabinet apply constraint
        /// </summary>
        /// <returns>CabinetApplyConstraint</returns>
        public new CabinetApplyConstraint Build()
        {
            return new CabinetApplyConstraint()
            {
                stage = _stage,
                order = _order,
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies,
            };
        }
    }
}
