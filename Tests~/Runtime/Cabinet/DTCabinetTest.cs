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

using System.Collections;
using Chocopoi.DressingTools.Api.Cabinet;
using Chocopoi.DressingTools.Api.Wearable;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Chocopoi.DressingFramework.Tests.Cabinet
{
    public class DTCabinetTest : RuntimeTestBase
    {
        [Test]
        public void RootGameObjectTest()
        {
            var obj1 = CreateGameObject();
            var obj2 = CreateGameObject();
            var cabinet = obj1.AddComponent<DTCabinet>();
            cabinet.rootGameObject = null;
            Assert.AreEqual(obj1, cabinet.AvatarGameObject);
            cabinet.rootGameObject = obj2;
            Assert.AreEqual(obj2, cabinet.AvatarGameObject);
        }

        [UnityTest]
        public IEnumerator ApplyInPlayModeOnLoad_AppliesNormally()
        {
            var avatarRoot = InstantiateRuntimeTestPrefab("DKTest_PhysBoneAvatarWithWearable.prefab");
            yield return null;
            // we are unable to check DTReport logs so we just check if all components are removed
            var cabinetComps = avatarRoot.GetComponents<DTCabinet>();
            Assert.AreEqual(0, cabinetComps.Length);
            var wearableComps = avatarRoot.GetComponents<DTWearable>();
            Assert.AreEqual(0, wearableComps.Length);
        }
    }
}
