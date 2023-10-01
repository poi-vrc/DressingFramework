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
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingFramework.Wearable.Modules;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    /// <summary>
    /// DK Editor Utilities
    /// </summary>
    internal static class DKEditorUtils
    {
        public static List<string> FindUnknownWearableModuleNames(List<WearableModule> modules)
        {
            var list = new List<string>();
            foreach (var module in modules)
            {
                var provider = PluginManager.Instance.GetWearableModuleProvider(module.moduleName);
                if (provider == null)
                {
                    list.Add(module.moduleName);
                }
            }
            return list;
        }

        public static void ApplyWearableTransforms(AvatarConfig avatarConfig, GameObject targetAvatar, GameObject targetWearable)
        {
            // check position delta and adjust
            {
                var wearableWorldPos = avatarConfig.worldPosition.ToVector3();
                if (targetWearable.transform.position - targetAvatar.transform.position != wearableWorldPos)
                {
                    Debug.LogFormat("[DressingTools] [AddCabinetWearable] Moved wearable world pos: {0}", wearableWorldPos.ToString());
                    targetWearable.transform.position += wearableWorldPos;
                }
            }

            // check rotation delta and adjust
            {
                var wearableWorldRot = avatarConfig.worldRotation.ToQuaternion();
                if (targetWearable.transform.rotation * Quaternion.Inverse(targetAvatar.transform.rotation) != wearableWorldRot)
                {
                    Debug.LogFormat("[DressingTools] [AddCabinetWearable] Moved wearable world rotation: {0}", wearableWorldRot.ToString());
                    targetWearable.transform.rotation *= wearableWorldRot;
                }
            }

            // apply avatar scale
            var lastAvatarParent = targetAvatar.transform.parent;
            var lastAvatarScale = Vector3.zero + targetAvatar.transform.localScale;
            if (lastAvatarParent != null)
            {
                // tricky workaround to apply lossy world scale is to unparent
                targetAvatar.transform.SetParent(null);
            }

            var avatarScaleVec = avatarConfig.avatarLossyScale.ToVector3();
            if (targetAvatar.transform.localScale != avatarScaleVec)
            {
                Debug.LogFormat("[DressingTools] [AddCabinetWearable] Adjusted avatar scale: {0}", avatarScaleVec.ToString());
                targetAvatar.transform.localScale = avatarScaleVec;
            }

            // apply wearable scale
            var lastWearableParent = targetWearable.transform.parent;
            var lastWearableScale = Vector3.zero + targetWearable.transform.localScale;
            if (lastWearableParent != null)
            {
                // tricky workaround to apply lossy world scale is to unparent
                targetWearable.transform.SetParent(null);
            }

            var wearableScaleVec = avatarConfig.wearableLossyScale.ToVector3();
            if (targetWearable.transform.localScale != wearableScaleVec)
            {
                Debug.LogFormat("[DressingTools] [AddCabinetWearable] Adjusted wearable scale: {0}", wearableScaleVec.ToString());
                targetWearable.transform.localScale = wearableScaleVec;
            }

            // restore avatar scale
            if (lastAvatarParent != null)
            {
                targetAvatar.transform.SetParent(lastAvatarParent);
            }
            targetAvatar.transform.localScale = lastAvatarScale;

            // restore wearable scale
            if (lastWearableParent != null)
            {
                targetWearable.transform.SetParent(lastWearableParent);
            }
            targetWearable.transform.localScale = lastWearableScale;
        }

        public static bool AddCabinetWearable(CabinetConfig cabinetConfig, GameObject avatarGameObject, WearableConfig wearableConfig, GameObject wearableGameObject)
        {
            var cabinetWearable = DKRuntimeUtils.GetCabinetWearable(wearableGameObject);

            // if not exist, create a new component
            if (cabinetWearable == null)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(wearableGameObject) && PrefabUtility.GetPrefabInstanceStatus(wearableGameObject) == PrefabInstanceStatus.NotAPrefab)
                {
                    // if not in scene, we instantiate it with a prefab connection
                    wearableGameObject = (GameObject)PrefabUtility.InstantiatePrefab(wearableGameObject);
                }

                // parent to avatar if haven't yet
                if (!DKRuntimeUtils.IsGrandParent(avatarGameObject.transform, wearableGameObject.transform))
                {
                    wearableGameObject.transform.SetParent(avatarGameObject.transform);
                }

                // applying scalings
                ApplyWearableTransforms(wearableConfig.avatarConfig, avatarGameObject, wearableGameObject);

                // add cabinet wearable component
                cabinetWearable = wearableGameObject.AddComponent<DTWearable>();
                cabinetWearable.WearableGameObject = wearableGameObject;
            }

            wearableConfig.info.RefreshUpdatedTime();
            cabinetWearable.ConfigJson = WearableConfigUtility.Serialize(wearableConfig);

            // TODO: check config valid

            var handlers = PluginManager.Instance.GetAllFrameworkEventHandlers();
            foreach (var handler in handlers)
            {
                // TODO: dependency graph
                handler.OnAddWearableToCabinet(cabinetConfig, avatarGameObject, wearableConfig, wearableGameObject);
            }

            return true;
        }

        public static void RemoveCabinetWearable(DTCabinet cabinet, DTWearable wearable)
        {
            var cabinetWearables = cabinet.AvatarGameObject.GetComponentsInChildren<DTWearable>();
            foreach (var cabinetWearable in cabinetWearables)
            {
                if (cabinetWearable == wearable)
                {
                    if (!PrefabUtility.IsOutermostPrefabInstanceRoot(cabinetWearable.gameObject))
                    {
                        Debug.Log("Prefab is not outermost. Aborting");
                        return;
                    }
                    Object.DestroyImmediate(cabinetWearable.gameObject);
                    break;
                }
            }
        }
    }
}
