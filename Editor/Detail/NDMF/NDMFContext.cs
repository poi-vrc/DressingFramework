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
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Logging;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Detail.NDMF
{
    /// <summary>
    /// NDMF implementation context
    /// </summary>
    internal class NDMFContext : Context
    {
        public override BuildRuntime CurrentRuntime { get => BuildRuntime.NDMF; }
        public override object RuntimeContext => _ndmfCtx;
        internal override Report Report => _report;

        public override Object AssetContainer => _ndmfCtx.AssetContainer;

        private readonly DKReport _report; // TODO: integrate to NDMF error reporting solution?
        private readonly BuildContext _ndmfCtx;

        public NDMFContext(BuildContext ndmfCtx) : base(ndmfCtx.AvatarRootObject)
        {
            _ndmfCtx = ndmfCtx;
            _report = new DKReport();
        }

        public override void CreateAsset(Object obj, string name)
        {
            obj.name = name;
            AssetDatabase.AddObjectToAsset(obj, _ndmfCtx.AssetContainer);
        }
    }
}
#endif
