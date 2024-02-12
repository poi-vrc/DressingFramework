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

using Chocopoi.DressingFramework.Detail.DK;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Logging;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Detail.DK
{
    public class AvatarBuilderTest : EditorTestBase
    {
        [Test]
        public void AvatarWithOneWearable_BuildsNormally()
        {
            // TODO: dynbone check?
            // This test requires PhysBone
            AssertPassImportedVRCSDK();

            var avatarRoot = InstantiateEditorTestPrefab("DKTest_PhysBoneAvatarWithWearable.prefab");
            var ab = new AvatarBuilder(avatarRoot);
            var report = (DKReport)ab.Context.Report;
            ab.RunStages();

            Assert.False(report.HasLogType(LogType.Error), "Should have no errors");
        }
    }
}
