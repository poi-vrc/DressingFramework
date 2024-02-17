/*
 * Copyright (c) 2024 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Detail.DK.Passes
{
    internal class DispatchAnimationStorePass : BuildPass
    {
        public override BuildConstraint Constraint =>
            InvokeAtStage(BuildStage.Transpose)
                .Build();

        public override bool Invoke(Context ctx)
        {
            var store = ctx.Feature<AnimationStore>();
            store.Dispatch();
            return true;
        }
    }
}
