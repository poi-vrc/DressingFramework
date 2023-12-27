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

#if DK_NDMF
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Menu;
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
        public override object RuntimeContext => _ndmfCtx;
        public override Report Report => _report;

        private readonly DKReport _report; // NDMF does not have an error-reporting solution yet
        private readonly BuildContext _ndmfCtx;

        public NDMFContext(BuildContext ndmfCtx) : base(ndmfCtx.AvatarRootObject)
        {
            _ndmfCtx = ndmfCtx;
            _report = new DKReport();
#if DK_MA && VRC_SDK_VRCSDK3
            AddContextFeature(new MA.MAMenuStore(this));
#else
            // fallback to DK if MA is not available
            AddContextFeature(new DK.DKMenuStore(this));
#endif
        }

        public override void CreateUniqueAsset(Object obj, string name)
        {
            obj.name = name;
            AssetDatabase.AddObjectToAsset(obj, _ndmfCtx.AssetContainer);
        }
    }
}
#endif
