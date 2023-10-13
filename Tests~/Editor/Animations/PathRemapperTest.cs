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

using Chocopoi.DressingFramework.Animations;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Animations
{
    public class PathRemapperTest : EditorTestBase
    {
        [Test]
        public void NormalRemapTest()
        {
            var avatarObj = InstantiateEditorTestPrefab("DKTest_PathRemapperAvatar.prefab");
            var wearableObj = avatarObj.transform.Find("Wearable");
            Assert.NotNull(wearableObj);

            var aRoot1 = avatarObj.transform.Find("ARoot1");
            Assert.NotNull(aRoot1);
            var aRoot2 = avatarObj.transform.Find("ARoot2");
            Assert.NotNull(aRoot2);
            var aRoot3 = avatarObj.transform.Find("ARoot3");
            Assert.NotNull(aRoot3);
            var wRoot1 = avatarObj.transform.Find("Wearable/WRoot1");
            Assert.NotNull(wRoot1);
            var wRoot2 = avatarObj.transform.Find("Wearable/WRoot2");
            Assert.NotNull(wRoot2);
            var wRoot3 = avatarObj.transform.Find("Wearable/WRoot3");
            Assert.NotNull(wRoot3);

            var pm = new PathRemapper(avatarObj);

            Assert.AreEqual("ARoot1", pm.Remap("ARoot1"));
            Assert.AreEqual("ARoot2", pm.Remap("ARoot2"));
            Assert.AreEqual("ARoot3", pm.Remap("ARoot3"));
            Assert.AreEqual("Wearable/WRoot1", pm.Remap("Wearable/WRoot1"));
            Assert.AreEqual("Wearable/WRoot2", pm.Remap("Wearable/WRoot2"));
            Assert.AreEqual("Wearable/WRoot3", pm.Remap("Wearable/WRoot3"));

            aRoot1.transform.parent = wearableObj.transform;
            aRoot2.transform.parent = aRoot1.transform;
            wRoot1.transform.parent = avatarObj.transform;
            wRoot2.transform.parent = wRoot1.transform;
            pm.InvalidateCache();

            Assert.AreEqual("Wearable/ARoot1", pm.Remap("ARoot1"));
            Assert.AreEqual("Wearable/ARoot1/ARoot2", pm.Remap("ARoot2"));
            Assert.AreEqual("ARoot3", pm.Remap("ARoot3"));
            Assert.AreEqual("WRoot1", pm.Remap("Wearable/WRoot1"));
            Assert.AreEqual("WRoot1/WRoot2", pm.Remap("Wearable/WRoot2"));
            Assert.AreEqual("Wearable/WRoot3", pm.Remap("Wearable/WRoot3"));
        }

        [Test]
        public void AvoidContainerBonesRemapTest()
        {
            var avatarObj = InstantiateEditorTestPrefab("DKTest_PathRemapperAvatar.prefab");
            var wearableObj = avatarObj.transform.Find("Wearable");
            Assert.NotNull(wearableObj);

            var aRoot1 = avatarObj.transform.Find("ARoot1");
            Assert.NotNull(aRoot1);
            var aRoot2 = avatarObj.transform.Find("ARoot2");
            Assert.NotNull(aRoot2);
            var aRoot3 = avatarObj.transform.Find("ARoot3");
            Assert.NotNull(aRoot3);
            var wRoot1 = avatarObj.transform.Find("Wearable/WRoot1");
            Assert.NotNull(wRoot1);
            var wRoot2 = avatarObj.transform.Find("Wearable/WRoot2");
            Assert.NotNull(wRoot2);
            var wRoot3 = avatarObj.transform.Find("Wearable/WRoot3");
            Assert.NotNull(wRoot3);

            var pm = new PathRemapper(avatarObj);

            aRoot1.transform.parent = wearableObj.transform;
            aRoot2.transform.parent = aRoot1.transform;
            pm.TagContainerBone(aRoot1.gameObject);
            pm.TagContainerBone(aRoot2.gameObject);

            wRoot1.transform.parent = avatarObj.transform;
            wRoot2.transform.parent = wRoot1.transform;

            Assert.AreEqual("Wearable", pm.Remap("ARoot1", true));
            Assert.AreEqual("Wearable", pm.Remap("ARoot2", true));
            Assert.AreEqual("ARoot3", pm.Remap("ARoot3", true));
            Assert.AreEqual("WRoot1", pm.Remap("Wearable/WRoot1", true));
            Assert.AreEqual("WRoot1/WRoot2", pm.Remap("Wearable/WRoot2", true));
            Assert.AreEqual("Wearable/WRoot3", pm.Remap("Wearable/WRoot3", true));
        }
    }
}
