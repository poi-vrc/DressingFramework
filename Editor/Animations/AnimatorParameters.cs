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
using System.Text.RegularExpressions;

namespace Chocopoi.DressingFramework.Animations
{
    /// <summary>
    /// Animator parameters
    /// 
    /// Warning: This API is unstable and experimental. Subject to change without any notice.
    /// </summary>
    public class AnimatorParameters : ContextFeature
    {
        public class ParameterConfig
        {
            public bool networkSynced;
            public bool saved;
            public float defaultValue;
            public Regex selector;

            public ParameterConfig(Regex regex)
            {
                defaultValue = 0.0f;
                networkSynced = true;
                saved = true;
                selector = regex;
            }

            public ParameterConfig(string regex) : this(new Regex(regex))
            {
            }

            public bool IsMatch(string parameterName) => selector.IsMatch(parameterName);
        }

        private readonly List<ParameterConfig> _configs;

        public AnimatorParameters()
        {
            _configs = new List<ParameterConfig>();
        }

        public ParameterConfig FindConfig(string parameterName)
        {
            // TODO: detect duplicates
            for (var i = _configs.Count - 1; i >= 0; i--)
            {
                var config = _configs[i];
                if (config.IsMatch(parameterName))
                {
                    return config;
                }
            }
            return null;
        }

        public void AddConfig(ParameterConfig config)
        {
            _configs.Add(config);
        }

        public void RemoveConfig(ParameterConfig config)
        {
            _configs.Remove(config);
        }

        internal override void OnEnable() { }

        internal override void OnDisable() { }
    }
}
