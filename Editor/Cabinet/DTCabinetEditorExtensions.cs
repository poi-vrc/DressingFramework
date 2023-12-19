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

using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingTools.Api.Cabinet;
using Chocopoi.DressingTools.Api.Wearable;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework.Cabinet
{
    public static class DTCabinetEditorExtensions
    {
        public static bool AddWearable(this DTCabinet cabinet, WearableConfig wearableConfig, GameObject wearableGameObject)
        {
            var cabinetConfig = CabinetConfigUtility.Deserialize(cabinet.ConfigJson);
            var cabinetWearable = DKEditorUtils.GetCabinetWearable(wearableGameObject);

            // if not exist, create a new component
            if (cabinetWearable == null)
            {
                if (PrefabUtility.IsPartOfAnyPrefab(wearableGameObject) && PrefabUtility.GetPrefabInstanceStatus(wearableGameObject) == PrefabInstanceStatus.NotAPrefab)
                {
                    // if not in scene, we instantiate it with a prefab connection
                    wearableGameObject = (GameObject)PrefabUtility.InstantiatePrefab(wearableGameObject);
                }

                // parent to avatar if haven't yet
                if (!DKEditorUtils.IsGrandParent(cabinet.AvatarGameObject.transform, wearableGameObject.transform))
                {
                    wearableGameObject.transform.SetParent(cabinet.AvatarGameObject.transform);
                }

                // applying scalings
                if (!PrefabUtility.IsPartOfAnyPrefab(wearableGameObject))
                {
                    wearableConfig.ApplyAvatarConfigTransforms(cabinet.AvatarGameObject, wearableGameObject);
                }

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
                handler.OnAddWearableToCabinet(cabinetConfig, cabinet.AvatarGameObject, wearableConfig, wearableGameObject);
            }

            return true;
        }

        public static void RemoveWearable(this DTCabinet cabinet, DTWearable wearable)
        {
            var cabinetWearables = cabinet.AvatarGameObject.GetComponentsInChildren<DTWearable>();
            foreach (var cabinetWearable in cabinetWearables)
            {
                if (cabinetWearable == wearable)
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(cabinetWearable.gameObject))
                    {
                        Debug.Log("[DressingFramework] Wearable is part of a prefab. Only the component is removed.");
                        Object.DestroyImmediate(cabinetWearable);
                    }
                    else
                    {
                        Object.DestroyImmediate(cabinetWearable.gameObject);
                    }
                    break;
                }
            }
        }
    }
}
