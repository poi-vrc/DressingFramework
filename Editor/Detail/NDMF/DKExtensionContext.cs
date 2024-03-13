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
using nadena.dev.ndmf;

namespace Chocopoi.DressingFramework.Detail.NDMF
{
    public class DKExtensionContext : IExtensionContext
    {
        public Context Context { get; private set; }

        public DKExtensionContext()
        {
            Context = null;
        }

        public void OnActivate(BuildContext ndmfCtx)
        {
            if (Context == null)
            {
                Context = new NDMFContext(ndmfCtx);
                Context.OnEnable();
            }
        }

        public void OnDeactivate(BuildContext ndmfCtx)
        {
            // TODO: when to disable?
        }
    }
}
#endif
