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
using System.Collections.Generic;
using System.Linq;
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Animations.VRChat;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Chocopoi.DressingFramework.Detail.DK.Passes.VRChat
{
    internal class ApplyVRCExParamsPass : BuildPass
    {
        private const string LogLabel = "ApplyVRCExParamsPass";

        public static class MessageCode
        {
            // Warn
            public const string UnsupportedUnityParamTypeForVRC = "detail.dk.passes.vrc.applyVrcExParams.msgCode.warn.unsupportedUnityParamTypeForVRC";
            // Error
            public const string MismatchParameterTypesBetweenAnimators = "detail.dk.passes.vrc.applyVrcExParams.msgCode.error.mismatchParameterTypesBetweenAnimators";
        }

        public override BuildConstraint Constraint => InvokeAtStage(BuildStage.Transpose).Build();

        private static void ScanAnimatorParameters(Context ctx, Dictionary<string, AnimatorControllerParameterType> parameters, VRCAvatarDescriptor.CustomAnimLayer[] customAnimLayers)
        {
            foreach (var customAnimLayer in customAnimLayers)
            {
                var animator = VRCAnimUtils.GetCustomAnimLayerAnimator(customAnimLayer);
                foreach (var animParam in animator.parameters)
                {
                    if (parameters.TryGetValue(animParam.name, out var type))
                    {
                        if (type != animParam.type)
                        {
                            ctx.Report.LogErrorLocalized(I18nManager.Instance.FrameworkTranslator, LogLabel, MessageCode.MismatchParameterTypesBetweenAnimators, type, animParam.type);
                        }
                    }
                    else
                    {
                        parameters[animParam.name] = animParam.type;
                    }
                }
            }
        }

        private static VRCExpressionParameters.ValueType? UnityParamTypeToVRCParamType(AnimatorControllerParameterType type)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Int:
                    return VRCExpressionParameters.ValueType.Int;
                case AnimatorControllerParameterType.Bool:
                    return VRCExpressionParameters.ValueType.Bool;
                case AnimatorControllerParameterType.Float:
                    return VRCExpressionParameters.ValueType.Float;
            }
            return null;
        }

        public override bool Invoke(Context ctx)
        {
            if (!ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                // not a VRC avatar
                return true;
            }

            var vrcParams = avatarDesc.expressionParameters;

            var parameters = new Dictionary<string, AnimatorControllerParameterType>();
            ScanAnimatorParameters(ctx, parameters, avatarDesc.baseAnimationLayers);
            ScanAnimatorParameters(ctx, parameters, avatarDesc.specialAnimationLayers);

            var animParams = ctx.Feature<AnimatorParameters>();
            var vrcParamList = new List<VRCExpressionParameters.Parameter>(vrcParams.parameters ?? new VRCExpressionParameters.Parameter[0]);

            // attempt to see if any config associated with the parameter
            foreach (var parameterName in parameters.Keys)
            {
                var paramConfig = animParams.FindConfig(parameterName);
                // no param config means unsynced / original
                if (paramConfig == null)
                {
                    continue;
                }

                var vrcParamType = UnityParamTypeToVRCParamType(parameters[parameterName]);
                if (vrcParamType == null)
                {
                    ctx.Report.LogWarnLocalized(I18nManager.Instance.FrameworkTranslator, LogLabel, MessageCode.UnsupportedUnityParamTypeForVRC, parameters[parameterName].ToString());
                    continue;
                }

                var vrcParam = vrcParamList.Where(p => p.name == parameterName).FirstOrDefault();
                if (vrcParam == null)
                {
                    vrcParam = new VRCExpressionParameters.Parameter();
                    vrcParamList.Add(vrcParam);
                }

                vrcParam.name = parameterName;
                vrcParam.saved = paramConfig.saved;
                vrcParam.defaultValue = paramConfig.defaultValue;
                vrcParam.networkSynced = paramConfig.networkSynced;
                vrcParam.valueType = vrcParamType.Value;
            }

            // write into vrc params
            vrcParams.parameters = vrcParamList.ToArray();
            EditorUtility.SetDirty(vrcParams);

            return true;
        }
    }
}
#endif
