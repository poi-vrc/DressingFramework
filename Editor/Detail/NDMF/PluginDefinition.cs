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
using System;
using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using nadena.dev.ndmf;
using nadena.dev.ndmf.fluent;
using UnityEngine;

[assembly: ExportsPlugin(typeof(Chocopoi.DressingFramework.Detail.NDMF.PluginDefinition))]

namespace Chocopoi.DressingFramework.Detail.NDMF
{
    internal class PluginDefinition : Plugin<PluginDefinition>
    {
        // the class nature of NDMF BuildPhase cannot be iterated directly
        private static readonly BuildPhase[] NDMFPhases = { BuildPhase.Resolving, BuildPhase.Generating, BuildPhase.Transforming, BuildPhase.Optimizing };

        // wrapper class to connect DK to NDMF Passes
        private class DKToNDMFPassWrapper : Pass<DKToNDMFPassWrapper>
        {
            public override string QualifiedName => _buildPass.Identifier;
            public override string DisplayName => _buildPass.FriendlyName;

            private BuildPass _buildPass;

            // empty args constructor to use as generic parameter
            public DKToNDMFPassWrapper() { }

            public DKToNDMFPassWrapper(BuildPass hook)
            {
                _buildPass = hook;
            }

            protected override void Execute(BuildContext context)
            {
                _buildPass.Invoke(context.Extension<DKExtensionContext>().Context);
            }
        }

        public override string DisplayName => "DressingFramework";

        private static Tuple<BuildStage, BuildStage> NDMFPhaseToDKStage(BuildPhase phase)
        {
            if (phase == BuildPhase.Resolving)
            {
                return new Tuple<BuildStage, BuildStage>(BuildStage.Pre, BuildStage.Preparation);
            }
            else if (phase == BuildPhase.Generating)
            {
                return new Tuple<BuildStage, BuildStage>(BuildStage.Generation, BuildStage.Generation);
            }
            else if (phase == BuildPhase.Transforming)
            {
                return new Tuple<BuildStage, BuildStage>(BuildStage.Transpose, BuildStage.Transpose);
            }
            else if (phase == BuildPhase.Optimizing)
            {
                return new Tuple<BuildStage, BuildStage>(BuildStage.Optimization, BuildStage.Post);
            }
            return null;
        }

        private void ConfigureStages(PluginManager pluginMgr, BuildPhase ndmfPhase, BuildStage startStage, BuildStage endStage)
        {
            InPhase(ndmfPhase).WithRequiredExtension(typeof(DKExtensionContext), seq =>
            {
                string lastIdentifier = null;
                for (var stage = startStage; stage <= endStage; stage++)
                {
                    var dkPasses = pluginMgr.GetSortedBuildPassesAtStage(BuildRuntime.NDMF, stage);

                    foreach (var dkPass in dkPasses)
                    {
                        if (lastIdentifier != null)
                        {
                            // add the last hook to ensure NDMF runs the same order from DK
                            seq.AfterPass(lastIdentifier);
                        }

                        // NDMF-level dependencies (no optionality)
                        foreach (var dep in dkPass.Constraint.afterRuntimePasses)
                        {
                            seq.AfterPass(dep.identifier);
                        }

                        var ndmfDp = seq.Run(new DKToNDMFPassWrapper(dkPass));

                        foreach (var dep in dkPass.Constraint.beforeRuntimePasses)
                        {
                            ndmfDp.BeforePass(dep.identifier);
                        }

                        lastIdentifier = dkPass.Identifier;
                    }
                }
            });
        }

        protected override void Configure()
        {
            var pluginMgr = new PluginManager();
            foreach (var ndmfPhase in NDMFPhases)
            {
                var stages = NDMFPhaseToDKStage(ndmfPhase);
                ConfigureStages(pluginMgr, ndmfPhase, stages.Item1, stages.Item2);
            }
        }
    }
}
#endif
