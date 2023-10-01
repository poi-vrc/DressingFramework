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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocopoi.DressingFramework.Localization
{
    /// <summary>
    /// I18n translator, specific for a plugin/namespace.
    /// </summary>
    public class I18nTranslator
    {
        private readonly Dictionary<string, JObject> _translations = null;

        internal I18nTranslator()
        {
            _translations = new Dictionary<string, JObject>();
        }

        /// <summary>
        /// Loads translations from a folder path with JSON files.
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        public void LoadTranslations(string folderPath)
        {
            _translations.Clear();
            var translationFileNames = Directory.GetFiles(folderPath, "*.json");
            foreach (var translationFileName in translationFileNames)
            {
                try
                {
                    var reader = new StreamReader(translationFileName);
                    var json = reader.ReadToEnd();
                    reader.Close();
                    _translations.Add(Path.GetFileNameWithoutExtension(translationFileName), JObject.Parse(json));
                }
                catch (IOException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// Get available locales of this translator
        /// </summary>
        /// <returns>String array of locale names</returns>
        public string[] GetAvailableLocales()
        {
            var keys = new string[_translations.Keys.Count];
            _translations.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// Translate. Short-hand of `Translate()`
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <param name="args">Arguments</param>
        /// <returns>Localized string</returns>
        public string _(string key, params object[] args)
        {
            return Translate(key, args);
        }

        /// <summary>
        /// Localize an Unity UIElement
        /// </summary>
        /// <param name="elem">VisualElement</param>
        /// <param name="recursively">Recursively localize the child elements</param>
        public void LocalizeElement(VisualElement elem, bool recursively = true)
        {
            if (elem is Foldout foldout)
            {
                // we process texts that starts with @
                var text = foldout.text;
                if (!string.IsNullOrEmpty(text) && text.StartsWith("@"))
                {
                    var i18nKey = text.Substring(1);
                    foldout.text = Translate(i18nKey);
                }
            }
            else if (elem is TextElement textElem)
            {
                // we process texts that starts with @
                var text = textElem.text;
                if (!string.IsNullOrEmpty(text) && text.StartsWith("@"))
                {
                    var i18nKey = text.Substring(1);
                    textElem.text = Translate(i18nKey);
                }
            }

            if (recursively)
            {
                foreach (var child in elem.Children())
                {
                    LocalizeElement(child, recursively);
                }
            }
        }

        /// <summary>
        /// Translate
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <param name="args">Arguments</param>
        /// <returns>Localized string</returns>
        public string Translate(string key, params object[] args)
        {
            return Translate(key, null, args);
        }

        private string JoinArrayToString(object[] arr)
        {
            string output = "";
            for (var i = 0; i < arr.Length; i++)
            {
                output += arr[i].ToString();
                if (i != arr.Length - 1)
                {
                    output += ", ";
                }
            }
            return output;
        }

        /// <summary>
        /// Translate with fallback string
        /// </summary>
        /// <param name="key">Translation key</param>
        /// <param name="fallback">Fallback string</param>
        /// <param name="args">Arguments</param>
        /// <returns>Localized string</returns>
        public string Translate(string key, string fallback = null, params object[] args)
        {
            string value;

            if ((value = TranslateByLocale(I18nManager.Instance.CurrentLocale, key, args)) != null)
            {
                return value;
            }

            if ((value = TranslateByLocale(I18nManager.DefaultLocale, key, args)) != null)
            {
                return value;
            }

            return fallback ?? string.Format("{0} ({1})", key, JoinArrayToString(args));
        }

        /// <summary>
        /// Translate by locale
        /// </summary>
        /// <param name="locale">Locale</param>
        /// <param name="key">Translation key</param>
        /// <param name="args">Arguments</param>
        /// <returns>Localized string</returns>
        public string TranslateByLocale(string locale, string key, params object[] args)
        {
            if (locale != null && _translations.ContainsKey(locale))
            {
                _translations.TryGetValue(locale, out var t);

                var value = t?.Value<string>(key);

                if (value != null)
                {
                    return string.Format(value, args);
                }
            }

            return null;
        }
    }

    /// <summary>
    /// I18n manager. Handles all translations that use DressingFramework's localization component
    /// </summary>
    public class I18nManager
    {
        /// <summary>
        /// Default locale
        /// </summary>
        public const string DefaultLocale = "en";

        private const string FrameworkTranslatorIdentifier = "com.chocopoi.vrc.dressingframework.i18n";
        private const string TranslationsFolder = "Packages/com.chocopoi.vrc.dressingframework/Translations";

        private static I18nManager s_instance = null;

        // internal framework translator
        internal I18nTranslator FrameworkTranslator
        {
            get
            {
                var translator = Translator(FrameworkTranslatorIdentifier);
                if (translator.GetAvailableLocales().Length == 0)
                {
                    translator.LoadTranslations(TranslationsFolder);
                }
                return translator;
            }
        }

        /// <summary>
        /// I18n manager instance
        /// </summary>
        public static I18nManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new I18nManager();
                }
                return s_instance;
            }
        }

        private readonly Dictionary<string, I18nTranslator> _translators;

        /// <summary>
        /// Current locale
        /// </summary>
        public string CurrentLocale { get; private set; }

        private I18nManager()
        {
            _translators = new Dictionary<string, I18nTranslator>();
        }

        /// <summary>
        /// Get all available locale names
        /// </summary>
        /// <returns>Locale names</returns>
        public string[] GetAvailableLocales()
        {
            var locales = new HashSet<string>();
            foreach (var translator in _translators.Values)
            {
                var keys = translator.GetAvailableLocales();
                foreach (var key in keys)
                {
                    locales.Add(key);
                }
            }
            return locales.ToArray();
        }

        /// <summary>
        /// Set locale
        /// </summary>
        /// <param name="locale">Locale name</param>
        public void SetLocale(string locale)
        {
            CurrentLocale = locale;
        }

        /// <summary>
        /// Obtain a translator with an identifier
        /// </summary>
        /// <param name="identifier">Identifier, you can also use plugin identifier for this</param>
        /// <returns></returns>
        public I18nTranslator Translator(string identifier)
        {
            if (!_translators.TryGetValue(identifier, out var translator))
            {
                translator = _translators[identifier] = new I18nTranslator();
            }
            return translator;
        }
    }
}

