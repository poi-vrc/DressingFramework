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

#if DK_NDMF
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.NDMF.Passes
{
    internal class CheckDKNDMFCallOrderPass : BuildPass
    {
        private static BuildRuntime? s_lastRuntime = null;

        public override string FriendlyName => "Check DK and NDMF Call Order";

        public override BuildConstraint Constraint =>
            InvokeAtStage(BuildStage.Pre)
                .WithRuntimes(BuildRuntime.DK, BuildRuntime.NDMF)
                .Build();

        internal static void Reset()
        {
            s_lastRuntime = null;
        }

        internal UI ui = new UnityUI();

        public override bool Invoke(Context ctx)
        {
            if (s_lastRuntime == ctx.CurrentRuntime)
            {
                return true;
            }

            if (ctx.CurrentRuntime == BuildRuntime.NDMF)
            {
                if (s_lastRuntime == BuildRuntime.DK)
                {
                    ui.Log($"[DressingFramework] DK->NDMF Run order correct.");
                }
                s_lastRuntime = null;
                return true;
            }

            s_lastRuntime = ctx.CurrentRuntime;
            return true;
        }

        internal interface UI
        {
            void Log(string str);
        }

        private class UnityUI : UI
        {
            public void Log(string str)
            {
                Debug.Log(str);
            }
        }
    }
}
#endif
