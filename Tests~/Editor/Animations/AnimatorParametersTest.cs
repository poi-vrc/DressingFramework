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

using System.Text.RegularExpressions;
using Chocopoi.DressingFramework.Animations;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Animations
{
    public class AnimatorParametersTest : EditorTestBase
    {
        [Test]
        public void ParameterConfigMatchTest()
        {
            var config1 = new AnimatorParameters.ParameterConfig("^abc$");
            Assert.True(config1.IsMatch("abc"));
            Assert.False(config1.IsMatch("abd"));

            var regex = new Regex("^Test_");
            var config2 = new AnimatorParameters.ParameterConfig(regex);
            Assert.True(config2.IsMatch("Test_123"));
            Assert.False(config2.IsMatch("Abc_456"));
        }

        [Test]
        public void FindConfigTest()
        {
            var animParams = new AnimatorParameters();

            var config1 = new AnimatorParameters.ParameterConfig("^Test1_");
            var config2 = new AnimatorParameters.ParameterConfig(".*_abc$");

            animParams.AddConfig(config1);
            animParams.AddConfig(config2);

            Assert.AreEqual(config1, animParams.FindConfig("Test1_123"));
            Assert.AreEqual(config1, animParams.FindConfig("Test1_456"));
            Assert.AreEqual(config2, animParams.FindConfig("abcdefg_abc"));
            Assert.AreEqual(config2, animParams.FindConfig("efghijk_abc"));
            Assert.Null(animParams.FindConfig("abcdefg"));

            animParams.RemoveConfig(config1);
            Assert.Null(animParams.FindConfig("Test1_789"));
        }
    }
}
