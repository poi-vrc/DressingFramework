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

#if DK_MA && VRC_SDK_VRCSDK3
using System.Collections.Generic;
using Chocopoi.DressingFramework.Animations;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Logging;
using nadena.dev.modular_avatar.core;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Chocopoi.DressingFramework.Detail.MA
{
    public class DKToMAGenerationPass : BuildPass
    {
        public override string FriendlyName => "DK to MA Conversion";
        public override BuildConstraint Constraint =>
            InvokeAtStage(BuildStage.Transpose)
                .BeforeRuntimeHook("nadena.dev.modular_avatar.core.editor.plugin.MenuInstallPluginPass")
                .BeforeRuntimeHook("nadena.dev.modular_avatar.core.editor.plugin.RenameParametersPluginPass")
                .BeforeRuntimeHook("nadena.dev.modular_avatar.core.editor.FixupExpressionsMenuPass")
                .Build();

        // TODO: move these functions to an utility class
        private static AnimatorController GetAnimLayerAnimator(VRCAvatarDescriptor.CustomAnimLayer animLayer)
        {
            return !animLayer.isDefault && animLayer.animatorController != null && animLayer.animatorController is AnimatorController controller ?
                 controller :
                 GetDefaultLayerAnimator(animLayer.type);
        }

        private static AnimatorController GetDefaultLayerAnimator(VRCAvatarDescriptor.AnimLayerType animLayerType)
        {
            string defaultControllerName = null;
            switch (animLayerType)
            {
                case VRCAvatarDescriptor.AnimLayerType.Base:
                    defaultControllerName = "LocomotionLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.Additive:
                    defaultControllerName = "IdleLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.Action:
                    defaultControllerName = "ActionLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.Gesture:
                    defaultControllerName = "HandsLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.FX:
                    defaultControllerName = "FaceLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.Sitting:
                    defaultControllerName = "SittingLayer";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.IKPose:
                    defaultControllerName = "UtilityIKPose";
                    break;
                case VRCAvatarDescriptor.AnimLayerType.TPose:
                    defaultControllerName = "UtilityTPose";
                    break;
            }

            if (defaultControllerName == null)
            {
                return null;
            }

            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Packages/com.vrchat.avatars/Samples/AV3 Demo Assets/Animation/Controllers/vrc_AvatarV3" + defaultControllerName + ".controller");
            if (controller == null)
            {
                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3" + defaultControllerName + ".controller");
            }
            return controller;
        }

        private static void GetVRCAnimLayerParameters(Dictionary<string, AnimatorControllerParameterType> parameters, Report report, VRCAvatarDescriptor.CustomAnimLayer[] customAnimLayers)
        {
            foreach (var customAnimLayer in customAnimLayers)
            {
                var animator = GetAnimLayerAnimator(customAnimLayer);
                foreach (var param in animator.parameters)
                {
                    if (!parameters.ContainsKey(param.name))
                    {
                        parameters[param.name] = param.type;
                    }
                    else if (parameters[param.name] != param.type)
                    {
                        report.LogError("DKToMAGenerationPass", $"Duplicate parameter name {param.name} with different type {parameters[param.name]} and {param.type} detected.");
                    }
                }
            }
        }

        private static void GetVRCAvatarParameters(Dictionary<string, AnimatorControllerParameterType> parameters, Context ctx)
        {
            if (!ctx.AvatarGameObject.TryGetComponent<VRCAvatarDescriptor>(out var avatarDesc))
            {
                ctx.Report.LogInfo("DKToMAGenerationPass", "This avatar does not contain a VRCAvatarDescriptor, skipping animator parameters scan");
                return;
            }
            GetVRCAnimLayerParameters(parameters, ctx.Report, avatarDesc.baseAnimationLayers);
            GetVRCAnimLayerParameters(parameters, ctx.Report, avatarDesc.specialAnimationLayers);
        }

        private static ParameterSyncType UnityParamTypeToMA(AnimatorControllerParameterType type)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Float:
                    return ParameterSyncType.Float;
                case AnimatorControllerParameterType.Int:
                    return ParameterSyncType.Int;
                case AnimatorControllerParameterType.Bool:
                    return ParameterSyncType.Bool;
                default:
                    return ParameterSyncType.NotSynced;
            }
        }

        public override bool Invoke(Context ctx)
        {
            var menuStore = ctx.Feature<MAMenuStore>() ?? throw new System.Exception("MAMenuStore is not available!");

            var scannedParameters = new Dictionary<string, AnimatorControllerParameterType>();
            GetVRCAvatarParameters(scannedParameters, ctx);

            // root menus
            foreach (var entry in menuStore.RootMenusToAppend)
            {
                // we create a gameobject to "rename" the menu
                var go = new GameObject(entry.name);
                Debug.Log("add object " + entry.name);
                go.transform.SetParent(ctx.AvatarGameObject.transform);

                var menuInstaller = go.AddComponent<ModularAvatarMenuInstaller>();
                menuInstaller.installTargetMenu = null;
                menuInstaller.menuToAppend = entry.menu;
            }

            // other menus
            foreach (var kvp in menuStore.OtherMenusToAppend)
            {
                var targetMenu = kvp.Key;
                var menusToAppend = kvp.Value;

                foreach (var entry in menusToAppend)
                {
                    // we create a gameobject to "rename" the menu
                    var go = new GameObject(entry.name);
                    go.transform.SetParent(ctx.AvatarGameObject.transform);

                    var menuInstaller = go.AddComponent<ModularAvatarMenuInstaller>();
                    menuInstaller.installTargetMenu = targetMenu;
                    menuInstaller.menuToAppend = entry.menu;
                }
            }

            // parameters config
            {
                var animParams = ctx.Feature<AnimatorParameters>();

                var go = new GameObject("DKMAParameters");
                go.transform.SetParent(ctx.AvatarGameObject.transform);
                var maParameters = go.AddComponent<ModularAvatarParameters>();
                foreach (var kvp in scannedParameters)
                {
                    // we match configs using regex and directly give the parameter name to MA
                    // instead of using the prefix option
                    var config = animParams.FindConfig(kvp.Key);
                    if (config != null)
                    {
                        // internal parameters, remapping are handled by DK already before this
                        var maParamType = UnityParamTypeToMA(kvp.Value);
                        if (maParamType == ParameterSyncType.NotSynced)
                        {
                            ctx.Report.LogWarn("DKToMAGenerationPass", $"Incompatible animator parameter \"{kvp.Key}\" with type \"{kvp.Value}\" detected. It will not be synced");
                        }
                        maParameters.parameters.Add(new ParameterConfig()
                        {
                            nameOrPrefix = kvp.Key,
                            remapTo = "",
                            internalParameter = false,
                            isPrefix = false,
                            syncType = maParamType,
                            localOnly = maParamType == ParameterSyncType.NotSynced || !config.networkSynced,
                            defaultValue = config.defaultValue,
                            saved = config.saved
                        });
                    }
                }
            }

            return true;
        }
    }
}
#endif
