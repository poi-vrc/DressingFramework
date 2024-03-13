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
using Chocopoi.DressingFramework.Detail.DK;
using Chocopoi.DressingFramework.Detail.NDMF;
using Chocopoi.DressingFramework.Detail.NDMF.Passes;
using Moq;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Detail.NDMF
{
    public class CheckDKNDMFCallOrderPassTest : EditorTestBase
    {
        public override void SetUp()
        {
            base.SetUp();
            CheckDKNDMFCallOrderPass.Reset();
        }

        public override void TearDown()
        {
            base.TearDown();
            CheckDKNDMFCallOrderPass.Reset();
        }

        [Test]
        public void CorrectOrderTest()
        {
            CheckDKNDMFCallOrderPass.Reset();

            var avatar = CreateGameObject("Avatar");

            var mock = new Mock<CheckDKNDMFCallOrderPass.UI>();

            var dkCtx = new DKNativeContext(avatar);
            var dkPass = new CheckDKNDMFCallOrderPass { ui = mock.Object };
            dkPass.Invoke(dkCtx);

            var ndmfCtx = new NDMFContext(new nadena.dev.ndmf.BuildContext(avatar, DKNativeContext.GeneratedAssetsPath));
            var ndmfPass = new CheckDKNDMFCallOrderPass() { ui = mock.Object };
            ndmfPass.Invoke(ndmfCtx);

            mock.Verify(m => m.Log(It.IsAny<string>()), Times.Once);
        }
    }
}
#endif
