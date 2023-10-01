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
    /// Wearable apply constraint
    /// </summary>
    public class WearableApplyConstraint : ExecutionConstraint
    {
        /// <summary>
        /// Stage to execute
        /// </summary>
        public CabinetApplyStage stage;
    }

    /// <summary>
    /// Wearable apply constraint builder
    /// </summary>
    public class WearableApplyConstraintBuilder : ExecutionConstraintBuilder
    {
        private readonly CabinetApplyStage _stage;

        /// <summary>
        /// Constructs a new builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        public WearableApplyConstraintBuilder(CabinetApplyStage stage)
        {
            _stage = stage;
        }

        /// <summary>
        /// Schedule this before a specific wearable hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder BeforeWearableHook(Type type, bool optional = false)
        {
            Before(DependencySource.WearableHook, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific wearable hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist.</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder BeforeWearableHook(string identifier, bool optional = false)
        {
            Before(DependencySource.WearableHook, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific wearable hook. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder AfterWearableHook(Type type, bool optional = false)
        {
            After(DependencySource.WearableHook, type.FullName, optional);
            return this;
        }


        /// <summary>
        /// Schedule this after a specific wearable hook
        /// </summary>
        /// <param name="identifier">Hook identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder AfterWearableHook(string identifier, bool optional = false)
        {
            After(DependencySource.WearableHook, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific wearable module. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder BeforeWearableModule(Type type, bool optional = false)
        {
            Before(DependencySource.WearableModule, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific wearable module
        /// </summary>
        /// <param name="identifier">Wearable module identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder BeforeWearableModule(string identifier, bool optional = false)
        {
            Before(DependencySource.WearableModule, identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific wearable module. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Hook type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder AfterWearableModule(Type type, bool optional = false)
        {
            After(DependencySource.WearableModule, type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific wearable module
        /// </summary>
        /// <param name="identifier">Wearable module identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public WearableApplyConstraintBuilder AfterWearableModule(string identifier, bool optional = false)
        {
            After(DependencySource.WearableModule, identifier, optional);
            return this;
        }

        /// <summary>
        /// Build this wearable apply constraint
        /// </summary>
        /// <returns>WearableApplyConstraint</returns>
        public new WearableApplyConstraint Build()
        {
            return new WearableApplyConstraint()
            {
                stage = _stage,
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies,
            };
        }
    }
}
