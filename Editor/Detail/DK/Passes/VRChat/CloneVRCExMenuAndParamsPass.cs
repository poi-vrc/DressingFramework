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

#if DK_VRCSDK3A
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Menu.VRChat;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Chocopoi.DressingFramework.Detail.DK.Passes.VRChat
{
    internal class CloneVRCExMenuAndParamsPass : BuildPass
    {
        private const string LogLabel = "CloneVRCExMenuAndParamsPass";

        public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Preparation).Build();

        public class MessageCode
        {
            public const string UnableToObtainDefaultExMenuAsset = "detail.dk.passes.vrc.cloneVrcExMenuAndParams.msgCode.error.unableToObtainDefaultExMenuAsset";
            public const string UnableToObtainDefaultExParamsAsset = "detail.dk.passes.vrc.cloneVrcExMenuAndParams.msgCode.error.unableToObtainDefaultExParamsAsset";
        }

        private static void CopyAndReplaceExpressionMenu(Context ctx, VRCAvatarDescriptor avatarDesc)
        {
            var vrcMenu = VRCMenuUtils.GetExpressionsMenu(avatarDesc);

            if (vrcMenu == null)
            {
                ctx.Report.LogErrorLocalized(I18nManager.Instance.FrameworkTranslator, LogLabel, MessageCode.UnableToObtainDefaultExMenuAsset);
                return;
            }

            var menuCopy = Object.Instantiate(vrcMenu);

            avatarDesc.expressionsMenu = menuCopy;
            ctx.CreateUniqueAsset(menuCopy, "RootMenu");
        }

        private static void CopyAndReplaceExpressionParameters(Context ctx, VRCAvatarDescriptor avatarDesc)
        {
            var vrcParams = VRCMenuUtils.GetExpressionsParameters(avatarDesc);

            if (vrcParams == null)
            {
                ctx.Report.LogErrorLocalized(I18nManager.Instance.FrameworkTranslator, LogLabel, MessageCode.UnableToObtainDefaultExParamsAsset);
                return;
            }

            var paramsCopy = Object.Instantiate(vrcParams);

            avatarDesc.expressionParameters = paramsCopy;
            ctx.CreateUniqueAsset(paramsCopy, "RootParameters");
        }

        public override bool Invoke(Context ctx)
        {
            if (!ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // not a VRC avatar
                return true;
            }

            CopyAndReplaceExpressionMenu(ctx, avatarDesc);
            CopyAndReplaceExpressionParameters(ctx, avatarDesc);

            return true;
        }
    }
}
#endif
