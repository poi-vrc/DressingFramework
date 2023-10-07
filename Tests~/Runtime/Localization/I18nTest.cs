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

using Chocopoi.DressingFramework.Localization;
using NUnit.Framework;

namespace Chocopoi.DressingFramework.Tests.Cabinet
{
    public class I18nTest : RuntimeTestBase
    {
        private static readonly I18nTranslator t = I18nManager.Instance.Translator(I18nIdentifier);
        private const string I18nIdentifier = "com.chocopoi.vrc.dressingframework.tests.cabinet.i18n-test";
        private string _originalLocale;

        public override void SetUp()
        {
            base.SetUp();
            if (t.GetAvailableLocales().Length == 0)
            {
                t.LoadTranslations("Packages/com.chocopoi.vrc.dressingframework/Tests/Runtime/Resources/I18nTest");
            }
            Assert.AreEqual(2, t.GetAvailableLocales().Length);
            _originalLocale = I18nManager.Instance.CurrentLocale;
        }

        public override void TearDown()
        {
            base.TearDown();
            I18nManager.Instance.SetLocale(_originalLocale);
        }

        [Test]
        public void GetAvailableLocalesTest()
        {
            Assert.AreEqual(2, t.GetAvailableLocales().Length);
        }

        [Test]
        public void GetTranslatorTest()
        {
            var translator = I18nManager.Instance.Translator(I18nIdentifier);
            Assert.AreEqual(t, translator);
        }

        [Test]
        public void TranslateTest()
        {
            I18nManager.Instance.SetLocale("en");
            Assert.AreEqual("en", I18nManager.Instance.CurrentLocale);
            Assert.AreEqual("Abc", t._("abc"));
            Assert.AreEqual("Def hi", t._("def", "hi"));
            Assert.AreEqual("Abc", t.Translate("abc"));
            Assert.AreEqual("Def hi", t.Translate("def", null, "hi"));
            Assert.AreEqual("not-a-key ()", t.Translate("not-a-key"));
            Assert.AreEqual("not-a-key (hi)", t.Translate("not-a-key", null, "hi"));
            Assert.AreEqual("fallback", t.Translate("not-a-key", "fallback"));

            I18nManager.Instance.SetLocale("ja");
            Assert.AreEqual("ja", I18nManager.Instance.CurrentLocale);
            Assert.AreEqual("エービーシー", t._("abc"));
            Assert.AreEqual("ディーイーフー hi", t._("def", "hi"));
            Assert.AreEqual("エービーシー", t.Translate("abc"));
            Assert.AreEqual("ディーイーフー hi", t.Translate("def", null, "hi"));
            Assert.AreEqual("not-a-key ()", t.Translate("not-a-key"));
            Assert.AreEqual("not-a-key (hi)", t.Translate("not-a-key", null, "hi"));
            Assert.AreEqual("fallback", t.Translate("not-a-key", "fallback"));
            // no locale fallback
            Assert.AreEqual("Ghi", t.Translate("ghi"));
        }

        [Test]
        public void TranslateByLocaleTest()
        {
            Assert.AreEqual("Abc", t.TranslateByLocale("en", "abc"));
            Assert.AreEqual("Def hi", t.TranslateByLocale("en", "def", "hi"));
            Assert.AreEqual("エービーシー", t.TranslateByLocale("ja", "abc"));
            Assert.AreEqual("ディーイーフー hi", t.TranslateByLocale("ja", "def", "hi"));
        }
    }
}
