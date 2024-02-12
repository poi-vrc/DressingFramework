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
    /// Plugin execution constraint
    /// </summary>
    public class PluginConstraint : ExecutionConstraint<string>
    {
        public static PluginConstraint Empty = new PluginConstraint();
    }

    /// <summary>
    /// Plugin execution constraint builder
    /// </summary>
    public class PluginConstraintBuilder : ExecutionConstraintBuilder<string>
    {
        /// <summary>
        /// Schedule this before a specific plugin
        /// </summary>
        /// <param name="identifier">Plugin identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginConstraintBuilder Before(string identifier, bool optional = false)
        {
            InnerBefore(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this before a specific plugin. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Plugin type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginConstraintBuilder Before(Type type, bool optional = false)
        {
            InnerBefore(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific plugin
        /// </summary>
        /// <param name="identifier">Plugin identifier</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginConstraintBuilder After(string identifier, bool optional = false)
        {
            InnerAfter(identifier, optional);
            return this;
        }

        /// <summary>
        /// Schedule this after a specific plugin. The type's class full name will be used.
        /// </summary>
        /// <param name="type">Plugin type</param>
        /// <param name="optional">Optional dependency. If not optional, the dependency solving will fail if the dependency does not exist</param>
        /// <returns>This builder</returns>
        public PluginConstraintBuilder After(Type type, bool optional = false)
        {
            InnerAfter(type.FullName, optional);
            return this;
        }

        /// <summary>
        /// Build this cabinet apply constraint
        /// </summary>
        /// <returns>CabinetApplyConstraint</returns>
        public new PluginConstraint Build()
        {
            return new PluginConstraint()
            {
                beforeDependencies = beforeDependencies,
                afterDependencies = afterDependencies,
            };
        }
    }
}
