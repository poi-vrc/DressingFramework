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
using Chocopoi.DressingFramework.Animations.VRChat;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using VRC.SDK3.Avatars.Components;

namespace Chocopoi.DressingFramework.Detail.DK.Passes.VRChat
{
    internal class CloneVRCAnimLayersPass : BuildPass
    {
        private const string LogLabel = "CloneVRCAnimLayersPass";

        public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Preparation).Build();

        public class MessageCode
        {
            public const string UnableToObtainDefaultAnimLayerAsset = "detail.dk.passes.vrc.cloneVrcAnimLayers.msgCode.error.unableToObtainDefaultAnimLayerAsset";
        }

        private static VRCAvatarDescriptor.CustomAnimLayer? CopyLayer(Context ctx, VRCAvatarDescriptor.CustomAnimLayer animLayer)
        {
            var animator = VRCAnimUtils.GetCustomAnimLayerAnimator(animLayer);

            if (animator == null)
            {
                // cannot even get the default animator
                return null;
            }

            animLayer.animatorController = VRCAnimUtils.DeepCopyAnimator(ctx, animator);
            animLayer.isEnabled = true;
            animLayer.isDefault = false;

            return animLayer;
        }

        private static bool CopyAndReplaceLayers(Context ctx, VRCAvatarDescriptor.CustomAnimLayer[] layers)
        {
            for (var i = 0; i < layers.Length; i++)
            {
                var copiedLayer = CopyLayer(ctx, layers[i]);
                if (copiedLayer == null)
                {
                    ctx.Report.LogErrorLocalized(I18nManager.Instance.FrameworkTranslator, LogLabel, MessageCode.UnableToObtainDefaultAnimLayerAsset, layers[i].type.ToString());
                    return false;
                }
                layers[i] = copiedLayer.Value;
            }
            return true;
        }

        public override bool Invoke(Context ctx)
        {
            if (!ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // not a VRC avatar
                return true;
            }

            if (!CopyAndReplaceLayers(ctx, avatarDesc.baseAnimationLayers)) return false;
            if (!CopyAndReplaceLayers(ctx, avatarDesc.specialAnimationLayers)) return false;

            return true;
        }
    }
}
#endif
