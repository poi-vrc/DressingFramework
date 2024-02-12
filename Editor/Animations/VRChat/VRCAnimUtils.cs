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

#if DK_VRCSDK3A
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Chocopoi.DressingFramework.Animations.VRChat
{
    internal class VRCAnimUtils
    {
        public static AnimatorController DeepCopyAnimator(Context ctx, AnimatorController source)
        {
            return GenericAnimatorDeepCopy(ctx.AssetContainer, source);
        }

        private static T GenericAnimatorDeepCopy<T>(Object assetContainer, T originalObject, System.Func<Object, Object> genericCopyFunc = null, Dictionary<Object, Object> copyCache = null) where T : Object
        {
            if (copyCache == null)
            {
                copyCache = new Dictionary<Object, Object>();
            }

            if (originalObject == null)
            {
                return null;
            }

            var originalObjectType = originalObject.GetType();

            // do not copy these types and return original
            if (originalObject is MonoScript ||
                originalObject is ScriptableObject ||
                originalObject is Texture ||
                originalObject is Material ||
                originalObject is AvatarMask)
            {
                return originalObject;
            }

            // only copy known types
            if (!(originalObject is Motion ||
                originalObject is AnimatorController ||
                originalObject is AnimatorStateMachine ||
                originalObject is StateMachineBehaviour ||
                originalObject is AnimatorState ||
                originalObject is AnimatorTransitionBase))
            {
                throw new System.Exception(string.Format("Unknown type detected while animator merging: {0}", originalObjectType.FullName));
            }

            // try obtain from cache
            if (copyCache.TryGetValue(originalObject, out var obj))
            {
                return (T)obj;
            }

            Object newObj;

            // attempt to copy with generic copy function
            if (genericCopyFunc != null)
            {
                newObj = genericCopyFunc(originalObject);
                if (newObj != null)
                {
                    return (T)newObj;
                }
            }

            // initialize a new object in a generic way
            var constructor = originalObjectType.GetConstructor(System.Type.EmptyTypes);
            if (constructor != null && !(originalObject is ScriptableObject))
            {
                newObj = (T)System.Activator.CreateInstance(originalObjectType);
                // copy serialized properties
                EditorUtility.CopySerialized(originalObject, newObj);
            }
            else
            {
                newObj = Object.Instantiate(originalObject);
            }
            copyCache[originalObject] = newObj;

            AssetDatabase.AddObjectToAsset(newObj, assetContainer);

            // deep copy serialized properties
            var serializedObj = new SerializedObject(newObj);
            var it = serializedObj.GetIterator();

            bool traverseDown = true;
            while (it.Next(traverseDown))
            {
                // reset
                traverseDown = true;

                if (it.propertyType == SerializedPropertyType.String)
                {
                    // disable traversal
                    traverseDown = false;
                }
                else if (it.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (it.objectReferenceValue == newObj)
                    {
                        // skip self-reference
                        continue;
                    }
                    // recursively perform deep copy
                    it.objectReferenceValue = GenericAnimatorDeepCopy(newObj, it.objectReferenceValue, genericCopyFunc, copyCache);
                }
            }

            // apply changes
            serializedObj.ApplyModifiedPropertiesWithoutUndo();

            return (T)newObj;
        }

        public static void FindAnimLayerArrayAndIndex(VRCAvatarDescriptor avatarDescriptor, VRCAvatarDescriptor.AnimLayerType animLayerType, out VRCAvatarDescriptor.CustomAnimLayer[] layers, out int customAnimLayerIndex)
        {
            layers = null;
            customAnimLayerIndex = -1;

            // try find in base anim layers
            for (var i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
            {
                if (avatarDescriptor.baseAnimationLayers[i].type == animLayerType)
                {
                    layers = avatarDescriptor.baseAnimationLayers;
                    customAnimLayerIndex = i;
                    break;
                }
            }

            // try find in special layers
            if (layers == null)
            {
                for (var i = 0; i < avatarDescriptor.specialAnimationLayers.Length; i++)
                {
                    if (avatarDescriptor.specialAnimationLayers[i].type == animLayerType)
                    {
                        layers = avatarDescriptor.specialAnimationLayers;
                        customAnimLayerIndex = i;
                        break;
                    }
                }

                // not found
            }
        }

        public static AnimatorController GetAvatarLayerAnimator(VRCAvatarDescriptor avatarDescriptor, VRCAvatarDescriptor.AnimLayerType animLayerType)
        {
            FindAnimLayerArrayAndIndex(avatarDescriptor, animLayerType, out var layers, out var index);
            if (layers == null || index == -1)
            {
                return null;
            }
            return GetCustomAnimLayerAnimator(layers[index]);
        }


        public static AnimatorController GetCustomAnimLayerAnimator(VRCAvatarDescriptor.CustomAnimLayer animLayer)
        {
            return !animLayer.isDefault && animLayer.animatorController != null && animLayer.animatorController is AnimatorController controller ?
                 controller :
                 GetDefaultLayerAnimator(animLayer.type);
        }

        public static AnimatorController GetDefaultLayerAnimator(VRCAvatarDescriptor.AnimLayerType animLayerType)
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

        public static bool IsProxyAnimation(Motion m)
        {
            if (m == null)
            {
                return false;
            }
            var animPath = AssetDatabase.GetAssetPath(m);
            return animPath != null && animPath != "" && animPath.Contains("/Animation/ProxyAnim/proxy");
        }
    }
}
#endif
