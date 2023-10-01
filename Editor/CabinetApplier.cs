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
using System.Collections.ObjectModel;
using Chocopoi.DressingFramework.Cabinet;
using Chocopoi.DressingFramework.Cabinet.Modules;
using Chocopoi.DressingFramework.Context;
using Chocopoi.DressingFramework.Extensibility.Plugin;
using Chocopoi.DressingFramework.Extensibility.Sequencing;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingFramework.Logging;
using Chocopoi.DressingFramework.Serialization;
using Chocopoi.DressingFramework.Wearable;
using Chocopoi.DressingFramework.Wearable.Modules;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingFramework
{
    public class CabinetApplier
    {
        private static readonly I18nTranslator t = I18nManager.Instance.FrameworkTranslator;

        public const string LogLabel = "CabinetApplier";

        public static class MessageCode
        {
            // Error
            public const string UnableToDeserializeCabinetConfig = "cabinet.applier.msgCode.error.unableToDeserializeCabinetConfig";
            public const string UnableToDeserializeWearableConfig = "cabinet.applier.msgCode.error.unableToDeserializeWearableConfig";
            public const string CabinetHookHasErrors = "cabinet.applier.msgCode.error.cabinetHookHasErrors";
            public const string WearableHookHasErrors = "cabinet.applier.msgCode.error.wearableHookHasErrors";
            public const string UnresolvedDependencies = "cabinet.applier.msgCode.error.unresolvedDependencies";
            public const string CyclicDependencies = "cabinet.applier.msgCode.error.cyclicDependencies";
            public const string ModuleHasNoProviderAvailable = "cabinet.applier.msgCode.error.moduleHasNoProviderAvailable";
        }

        private ApplyCabinetContext _cabCtx;
        private ICabinet _cabinet;

        public CabinetApplier(DKReport report, ICabinet cabinet)
        {
            if (PrefabUtility.GetPrefabAssetType(cabinet.AvatarGameObject) != PrefabAssetType.NotAPrefab)
            {
                throw new System.Exception("A prefab is passed through cabinet applier! Aborting as this will modify the prefab directly!");
            }

            _cabinet = cabinet;
            _cabCtx = new ApplyCabinetContext()
            {
                report = report,
                avatarGameObject = cabinet.AvatarGameObject
            };
        }

        private void SetUp()
        {
            // remove previous generated files
            AssetDatabase.DeleteAsset(ApplyCabinetContext.GeneratedAssetsPath);
            AssetDatabase.CreateFolder("Assets", ApplyCabinetContext.GeneratedAssetsFolderName);

            // attempt to deserialize cabinet config
            try
            {
                _cabCtx.cabinetConfig = CabinetConfigUtility.Deserialize(_cabinet.ConfigJson);
            }
            catch (System.Exception ex)
            {
                _cabCtx.report.LogExceptionLocalized(t, LogLabel, ex);
                _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.UnableToDeserializeCabinetConfig, _cabCtx.avatarGameObject.gameObject.name);
                return;
            }

            var wearables = DKRuntimeUtils.GetCabinetWearables(_cabCtx.avatarGameObject);

            foreach (var wearable in wearables)
            {

                // deserialize the config
                WearableConfig wearableConfig = null;
                try
                {
                    wearableConfig = WearableConfigUtility.Deserialize(wearable.ConfigJson);
                }
                catch (System.Exception ex)
                {
                    _cabCtx.report.LogExceptionLocalized(t, LogLabel, ex);
                    _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.UnableToDeserializeWearableConfig, wearable.WearableGameObject.name);
                    return;
                }

                if (wearableConfig == null)
                {
                    _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.UnableToDeserializeWearableConfig, wearable.WearableGameObject.name);
                    return;
                }

                // detect unknown modules and report them
                var unknownModules = DKEditorUtils.FindUnknownWearableModuleNames(wearableConfig.modules);
                if (unknownModules.Count > 0)
                {
                    foreach (var name in unknownModules)
                    {
                        _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.ModuleHasNoProviderAvailable, name);
                    }
                    return;
                }

                // clone if needed
                GameObject wearableObj;
                if (DKRuntimeUtils.IsGrandParent(_cabCtx.avatarGameObject.transform, wearable.WearableGameObject.transform))
                {
                    wearableObj = wearable.WearableGameObject;

                    if (PrefabUtility.GetPrefabAssetType(wearableObj) != PrefabAssetType.NotAPrefab)
                    {
                        throw new System.Exception("A wearable prefab is passed through cabinet applier!");
                    }
                }
                else
                {
                    // instantiate wearable prefab and parent to avatar
                    wearableObj = Object.Instantiate(wearable.WearableGameObject, _cabCtx.avatarGameObject.transform);
                }

                var wearCtx = new ApplyWearableContext()
                {
                    wearableConfig = wearableConfig,
                    wearableGameObject = wearableObj
                };
                _cabCtx.wearableContexts[wearable] = wearCtx;
            }
        }

        private void TearDown()
        {
            // remove all DT components
            var dtComps = _cabCtx.avatarGameObject.GetComponentsInChildren<DKBaseComponent>();
            foreach (var comp in dtComps)
            {
                Object.DestroyImmediate(comp);
            }
        }

        private bool RunCabinetHooksAtStage(CabinetApplyStage stage, CabinetHookStageRunOrder order)
        {
            var hooks = new List<ICabinetHook>();
            hooks.AddRange(PluginManager.Instance.GetCabinetHooksAtStage(stage, order));
            hooks.AddRange(PluginManager.Instance.GetCabinetModulesAtStage(stage, order));

            // build graph
            var graph = new DependencyGraph();
            foreach (var hook in hooks)
            {
                var ds = GetDependencySource(hook);
                if (ds == null)
                {
                    Debug.Log("[DressingFramework] Unknown dependency source hook: " + hook.Identifier);
                    return false;
                }
                graph.Add(ds.Value, hook.Identifier, hook.Constraint);
            }

            if (!graph.IsResolved())
            {
                _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.UnresolvedDependencies);
                return false;
            }

            var topOrder = graph.Sort();
            if (topOrder == null)
            {
                _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.CyclicDependencies);
                return false;
            }

            foreach (var tuple in topOrder)
            {
                var hook = GetHookByTuple(hooks, tuple);

                bool result = false;

                if (hook is CabinetHookBase wearableHook)
                {
                    result = wearableHook.Invoke(_cabCtx);
                }
                else if (hook is CabinetModuleProviderBase moduleProvider)
                {
                    result = moduleProvider.Invoke(
                        _cabCtx,
                        new ReadOnlyCollection<CabinetModule>(DKRuntimeUtils.GetCabinetModulesByName(_cabCtx.cabinetConfig, moduleProvider.Identifier)),
                        false);
                }
                else
                {
                    Debug.Log("[DressingFramework] Unknown hook type: " + hook.Identifier);
                    return false;
                }

                if (!result)
                {
                    _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.CabinetHookHasErrors, hook.Identifier);
                    return false;
                }
            }

            return true;
        }

        private static DependencySource? GetDependencySource(IHook hook)
        {
            if (hook is CabinetHookBase)
            {
                return DependencySource.CabinetHook;
            }
            else if (hook is CabinetModuleProviderBase)
            {
                return DependencySource.CabinetModule;
            }
            else if (hook is WearableHookBase)
            {
                return DependencySource.WearableHook;
            }
            else if (hook is WearableModuleProviderBase)
            {
                return DependencySource.WearableModule;
            }

            return null;
        }

        private static T GetHookByTuple<T>(List<T> hooks, System.Tuple<DependencySource, string> tuple) where T : IHook
        {
            foreach (var hook in hooks)
            {
                if (GetDependencySource(hook).Value == tuple.Item1 && tuple.Item2 == hook.Identifier)
                {
                    return hook;
                }
            }
            return default;
        }

        private bool RunWearableHooksAtStage(CabinetApplyStage stage)
        {
            var hooks = new List<IWearableHook>();
            hooks.AddRange(PluginManager.Instance.GetWearableHooksAtStage(stage));
            hooks.AddRange(PluginManager.Instance.GetWearableModulesAtStage(stage));

            // build graph
            var graph = new DependencyGraph();
            foreach (var hook in hooks)
            {
                var ds = GetDependencySource(hook);
                if (ds == null)
                {
                    Debug.Log("[DressingFramework] Unknown dependency source hook: " + hook.Identifier);
                    return false;
                }
                graph.Add(ds.Value, hook.Identifier, hook.Constraint);
            }

            if (!graph.IsResolved())
            {
                _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.UnresolvedDependencies);
                return false;
            }

            var topOrder = graph.Sort();
            if (topOrder == null)
            {
                _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.CyclicDependencies);
                return false;
            }

            foreach (var wearable in _cabCtx.wearableContexts.Keys)
            {
                var wearCtx = _cabCtx.wearableContexts[wearable];

                foreach (var tuple in topOrder)
                {
                    var hook = GetHookByTuple(hooks, tuple);

                    bool result;
                    if (hook is WearableHookBase wearableHook)
                    {
                        result = wearableHook.Invoke(_cabCtx, wearCtx);
                    }
                    else if (hook is WearableModuleProviderBase moduleProvider)
                    {
                        result = moduleProvider.Invoke(
                            _cabCtx,
                            wearCtx,
                            new ReadOnlyCollection<WearableModule>(DKRuntimeUtils.GetWearableModulesByName(wearCtx.wearableConfig, moduleProvider.Identifier)),
                            false);
                    }
                    else
                    {
                        Debug.Log("[DressingFramework] Unknown hook type: " + hook.Identifier);
                        return false;
                    }

                    if (!result)
                    {
                        _cabCtx.report.LogErrorLocalized(t, LogLabel, MessageCode.WearableHookHasErrors, hook.Identifier);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool RunHooksAtStage(CabinetApplyStage stage)
        {
            if (!RunCabinetHooksAtStage(stage, CabinetHookStageRunOrder.Before)) return false;
            if (!RunWearableHooksAtStage(stage)) return false;
            if (!RunCabinetHooksAtStage(stage, CabinetHookStageRunOrder.After)) return false;
            return true;
        }

        public void RunStages(CabinetApplyStage beginStage = CabinetApplyStage.Pre, CabinetApplyStage endStage = CabinetApplyStage.Post)
        {
            for (var stage = beginStage; stage <= endStage; stage++)
            {
                switch (stage)
                {
                    case CabinetApplyStage.Pre:
                        SetUp();
                        if (!RunHooksAtStage(CabinetApplyStage.Pre)) return;
                        break;
                    case CabinetApplyStage.Analyzing:
                    case CabinetApplyStage.Transpose:
                    case CabinetApplyStage.Integration:
                    case CabinetApplyStage.Optimization:
                        if (!RunHooksAtStage(stage)) return;
                        break;
                    case CabinetApplyStage.Post:
                        if (!RunHooksAtStage(CabinetApplyStage.Post)) return;
                        TearDown();
                        break;
                }
            }
        }
    }
}
