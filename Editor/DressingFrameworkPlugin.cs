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

using Chocopoi.DressingFramework.Detail.MA;
using Chocopoi.DressingFramework.Extensibility;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Menu;

namespace Chocopoi.DressingFramework
{
    internal class DressingFrameworkPlugin : Plugin
    {
        public const string PluginIdentifier = "com.chocopoi.vrc.dressingframework.plugin";

        public override string Identifier => PluginIdentifier;
        public override string FriendlyName => "DressingFramework";
        public override PluginConstraint Constraint => PluginConstraint.Empty;

        private class TestPass : BuildPass
        {
            public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Generation).Build();

            public override bool Invoke(Context ctx)
            {
                var feature = ctx.Feature<MenuStore>();
                var menuGp = new MenuGroupBuilder(new MenuGroup())
                    .AddButton("hi", "hi", 1.0f)
                    .AddToggle("test", "test", 0.2f)
                    .BeginNewSubMenu("testmenu")
                        .AddToggle("wow", "w", 1.0f)
                    .EndNewSubMenu()
                    .GetContainingMenuRepository() as MenuGroup;
                feature.AppendMenu("a", menuGp);
                return true;
            }
        }

        public override void OnEnable()
        {
#if DK_MA && VRC_SDK_VRCSDK3
            RegisterBuildPass(new DKToMAGenerationPass());
            RegisterBuildPass(new TestPass());
#endif
        }

        public override void OnDisable()
        {
        }
    }
}
