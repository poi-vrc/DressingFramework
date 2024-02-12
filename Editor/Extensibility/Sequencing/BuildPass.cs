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

namespace Chocopoi.DressingFramework.Extensibility.Sequencing
{
    /// <summary>
    /// Abstract build pass base class
    /// </summary>
    public abstract class BuildPass : IBuildPass
    {
        public virtual string Identifier => GetType().FullName;
        public virtual string FriendlyName => GetType().Name;
        public abstract BuildConstraint Constraint { get; }
        public abstract bool Invoke(Context ctx);

        /// <summary>
        /// Create a new apply constraint builder with the specified stage
        /// </summary>
        /// <param name="stage">Stage that this hook will run</param>
        /// <returns>Apply constraint builder</returns>
        protected BuildConstraintBuilder InvokeAtStage(BuildStage stage) => new BuildConstraintBuilder(stage);
    }
}
