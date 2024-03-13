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

using Chocopoi.DressingFramework.Menu;
#if DK_VRCSDK3A
using Chocopoi.DressingFramework.Menu.VRChat;
#endif

namespace Chocopoi.DressingFramework.Detail.DK
{
    internal class DKMenuStore : MenuRepositoryStore
    {
        private readonly Context _ctx;

        public DKMenuStore(Context ctx) : base(ctx)
        {
            _ctx = ctx;
        }

        public override IMenuRepository GetRootMenu()
        {
            IMenuRepository rootMenu;
#if DK_VRCSDK3A
            {
                if (_ctx.AvatarGameObject.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out var avatarDesc))
                {
                    rootMenu = new VRCMenuSafeWrapper(avatarDesc.expressionsMenu, _ctx);
                }
                else
                {
                    // not a VRC avatar
                    rootMenu = new MenuGroup();
                }
            }
#else
            {
                // nothing that we could do, just an empty menu
                rootMenu = new MenuGroup();
                _ctx.Report.LogInfo("DKMenuStore", "No compatible menu platform detected. An empty DK menu group is created as root menu.");
            }
#endif
            return rootMenu;
        }

        internal override void OnDisable() { }

        internal override void OnEnable() { }
    }
}
