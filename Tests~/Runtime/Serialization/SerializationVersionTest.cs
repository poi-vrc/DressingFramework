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

using Chocopoi.DressingFramework.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Wearable
{
    public class SerializationVersionTest : RuntimeTestBase
    {
        private class TestJson
        {
            public SerializationVersion version;
        }

        [Test]
        public void JsonSerializationTest()
        {
            var json = "{\"version\":\"1.2.3-extra\"}";
            var obj = JsonConvert.DeserializeObject<TestJson>(json, new SerializationVersionConverter());
            Assert.AreEqual(1, obj.version.Major);
            Assert.AreEqual(2, obj.version.Minor);
            Assert.AreEqual(3, obj.version.Patch);
            Assert.AreEqual("extra", obj.version.Extra);

            var output = JsonConvert.SerializeObject(obj);
            Assert.AreEqual(json, output);
        }

        [Test]
        public void MajorMinorPatchConstructorTest()
        {
            var version = new SerializationVersion(1, 2, 3);
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
            Assert.Null(version.Extra);
        }

        [Test]
        public void MajorMinorPatchExtraConstructorTest()
        {
            var version = new SerializationVersion(1, 2, 3, "extra");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
            Assert.AreEqual("extra", version.Extra);
        }

        [Test]
        public void StringConstructorTest()
        {
            var version = new SerializationVersion("1.2.3-extra");
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
            Assert.AreEqual("extra", version.Extra);
        }

        [Test]
        public void ToStringTest()
        {
            var version = new SerializationVersion("1.2.3-extra");
            Assert.AreEqual("1.2.3-extra", version.ToString());
        }
    }
}
