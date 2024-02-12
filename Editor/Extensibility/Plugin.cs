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
using System.Linq;
using Chocopoi.DressingFramework.Extensibility.Sequencing;

namespace Chocopoi.DressingFramework.Extensibility
{
    /// <summary>
    /// Plugin entry base class
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        public virtual string Identifier => GetType().FullName;
        public abstract string FriendlyName { get; }
        public abstract PluginConstraint Constraint { get; }

        private readonly Dictionary<string, BuildPass> _buildPasses;

        /// <summary>
        /// Constructs a new plugin. If you override this constructor, make sure you also call the base constructor via `base()`.
        /// </summary>
        public Plugin()
        {
            _buildPasses = new Dictionary<string, BuildPass>();
        }

        public abstract void OnEnable();
        public abstract void OnDisable();

        /// <summary>
        /// Register a cabinet hook. Which will be executed on the cabinet apply level.
        /// </summary>
        /// <param name="hook">BuildPass</param>
        protected void RegisterBuildPass(BuildPass hook)
        {
            _buildPasses[hook.Identifier] = hook;
        }

        internal List<BuildPass> GetAllBuildPasses()
        {
            return _buildPasses.Values.ToList();
        }
    }
}
