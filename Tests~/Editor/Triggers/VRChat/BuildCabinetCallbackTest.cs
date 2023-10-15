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

#if VRC_SDK_VRCSDK3
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Triggers.VRChat;
using Chocopoi.DressingFramework.Wearable;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Chocopoi.DressingFramework.Tests.Triggers.VRChat
{
    public class BuildCabinetCallbackTest : EditorTestBase
    {
        private GameObject CreateTestSetup()
        {
            var avatarObj = CreateGameObject("Avatar");

            var cabinet = DKEditorUtils.GetAvatarCabinet(avatarObj, true);
            Assert.NotNull(cabinet);
            var cabinetConfig = CabinetConfigUtility.Deserialize(cabinet.ConfigJson);
            Assert.NotNull(cabinetConfig);

            var wearableObj = CreateGameObject("Wearable", avatarObj.transform);
            var wearableConfig = new WearableConfig();
            cabinet.AddWearable(wearableConfig, wearableObj);

            return avatarObj;
        }

        [Test]
        public void OnPreprocessAvatar_Valid()
        {
            var avatarObj = CreateTestSetup();
            var mock = new Mock<BuildCabinetCallback.UI>();

            var callback = new BuildCabinetCallback
            {
                ui = mock.Object
            };

            Assert.True(callback.OnPreprocessAvatar(avatarObj));
            mock.Verify(ui => ui.ShowProgressBar(), Times.Once);
            mock.Verify(ui => ui.ClearProgressBar(), Times.Once);
        }

        [Test]
        public void OnPreprocessAvatar_Invalid()
        {
            var avatarObj = CreateTestSetup();

            // destroy the cabinet json
            var cabinet = DKEditorUtils.GetAvatarCabinet(avatarObj);
            Assert.NotNull(cabinet);
            cabinet.ConfigJson = cabinet.ConfigJson.Substring(5);

            var mock = new Mock<BuildCabinetCallback.UI>();

            var callback = new BuildCabinetCallback
            {
                ui = mock.Object
            };

            Assert.False(callback.OnPreprocessAvatar(avatarObj));
            mock.Verify(ui => ui.ShowProgressBar(), Times.Once);
            mock.Verify(ui => ui.ClearProgressBar(), Times.Once);
            mock.Verify(ui => ui.ShowReportWindow(), Times.Once);
            mock.Verify(ui => ui.ShowErrorPreprocessingAvatarDialog(), Times.Once);
        }
    }
}
#endif
