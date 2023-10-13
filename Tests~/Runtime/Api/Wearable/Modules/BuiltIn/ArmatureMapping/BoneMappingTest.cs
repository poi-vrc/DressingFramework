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

using Chocopoi.DressingTools.Api.Wearable.Modules.BuiltIn.ArmatureMapping;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Api.Wearable.Modules.BuiltIn.ArmatureMapping
{
    public class BoneMappingTest : RuntimeTestBase
    {
        [Test]
        public void EqualityTest()
        {
            var bm1 = new BoneMapping()
            {
                mappingType = BoneMappingType.DoNothing,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            var bm2 = new BoneMapping()
            {
                mappingType = BoneMappingType.DoNothing,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            Assert.True(bm1.Equals(bm2));

            var bm3 = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            var bm4 = new BoneMapping()
            {
                mappingType = BoneMappingType.DoNothing,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            Assert.False(bm3.Equals(bm4));

            var bm5 = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Efg",
                wearableBonePath = "Def"
            };
            var bm6 = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            Assert.False(bm5.Equals(bm6));

            var bm7 = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Efg",
                wearableBonePath = "Hij"
            };
            var bm8 = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Efg",
                wearableBonePath = "Def"
            };
            Assert.False(bm7.Equals(bm8));
        }

        [Test]
        public void ToStringTest()
        {
            var bm = new BoneMapping()
            {
                mappingType = BoneMappingType.MoveToBone,
                avatarBonePath = "Abc",
                wearableBonePath = "Def"
            };
            Assert.AreEqual("MoveToBone: Def -> Abc", bm.ToString());
        }
    }
}
