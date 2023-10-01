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

using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Extensibility.Plugin
{
    /// <summary>
    /// Cabinet hook base class
    /// </summary>
    public abstract class CabinetHookBase : ICabinetHook
    {
        public virtual string Identifier => GetType().FullName;
        public abstract string FriendlyName { get; }

        /// <summary>
        /// Execution dependency and constraint of this hook
        /// </summary>
        public abstract CabinetApplyConstraint Constraint { get; }

        /// <summary>
        /// Shortcut to create a cabinet apply constraint builder
        /// </summary>
        /// <param name="stage">Stage to execute</param>
        /// <returns>Builder</returns>
        protected CabinetApplyConstraintBuilder ApplyAtStage(CabinetApplyStage stage, CabinetHookStageRunOrder order) => new CabinetApplyConstraintBuilder(stage, order);

        /// <summary>
        /// Invoke this hook
        /// </summary>
        /// <param name="cabCtx">Apply cabinet context</param>
        /// <returns>Return false to stop continuing execution</returns>
        public abstract bool Invoke(ApplyCabinetContext cabCtx);
    }
}
