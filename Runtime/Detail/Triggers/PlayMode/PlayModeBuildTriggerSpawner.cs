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

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Chocopoi.DressingFramework.Detail.DK.Triggers.PlayMode
{
    /// <summary>
    /// This will be automatically added to open scenes when entering playmode and spawn PlayModeBuildTrigger components to avatars on Awake.
    /// 
    /// This mechanism is referenced from MA/NDMF with modifications.
    /// Copyright (c) 2022 bd_ under MIT License
    /// 
    /// Play mode build triggering mechanisms from VRCFury and MA are researched. This mechanism is referenced instead
    /// since we would need less hacks with Lyuma's Av3Emulator (i.e. restarting?) and other tools later on.
    /// But the disadvantage of this is to have a spawner object present in the scene.
    /// Instead of marking as hidden and always present in the scene, the spawner will be destroyed on play mode update and entering edit mode.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [AddComponentMenu("")]
    [DefaultExecutionOrder(-19999)]
    [DisallowMultipleComponent]
    internal class PlayModeBuildTriggerSpawner : MonoBehaviour
    {
        private const string SpawnerObjectName = "DTPlayModeBuildTriggerSpawner";

        public enum Lifecycle
        {
            Awake,
            Start
        }

        public delegate void OnAvatarLifecycleDelegate(Lifecycle stage, GameObject avatarGameObject);
        public static OnAvatarLifecycleDelegate OnAvatarLifecycle = (stage, avatarGameObject) => { };

#if UNITY_EDITOR
        static PlayModeBuildTriggerSpawner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                CreateSpawnersToOpenScenes();
            }
            else if (change == PlayModeStateChange.EnteredEditMode)
            {
                DestroySpawnersOfOpenScenes();
            }
        }

        private static void CreateSpawnersToOpenScenes()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    CreateSpawnersToScene(scene);
                }
            }
        }

        private static void DestroySpawnersOfOpenScenes()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    DestroySpawnersOfScene(scene);
                }
            }
        }

        private static bool IsInvalidScene(Scene scene)
        {
            return !scene.IsValid() || EditorSceneManager.IsPreviewScene(scene) || EditorApplication.isPlaying;
        }

        private static void CreateSpawnersToScene(Scene scene)
        {
            if (IsInvalidScene(scene))
            {
                return;
            }

            var found = false;
            var rootObjs = scene.GetRootGameObjects();
            foreach (var rootObj in rootObjs)
            {
                if (rootObj.TryGetComponent<PlayModeBuildTriggerSpawner>(out var comp))
                {
                    if (found)
                    {
                        // avoid having more than one spawner
                        DestroyImmediate(comp);
                    }
                    found = true;
                }
            }

            // create spawner if not exist
            if (!found)
            {
                var lastScene = SceneManager.GetActiveScene();

                SceneManager.SetActiveScene(scene);
                var go = new GameObject(SpawnerObjectName);
                go.AddComponent<PlayModeBuildTriggerSpawner>();

                SceneManager.SetActiveScene(lastScene);
            }
        }

        private static void DestroySpawnersOfScene(Scene scene)
        {
            if (IsInvalidScene(scene))
            {
                return;
            }

            var rootObjs = scene.GetRootGameObjects();
            foreach (var rootObj in rootObjs)
            {
                if (rootObj.TryGetComponent<PlayModeBuildTriggerSpawner>(out var comp))
                {
                    DestroyImmediate(comp.gameObject);
                }
            }
        }
#endif

        private void Awake()
        {
            if (DKRuntimeUtils.IsPlaying && this != null)
            {
                // TODO

                var avatars = DKRuntimeUtils.FindSceneAvatars(gameObject.scene);
                foreach (var avatar in avatars)
                {
                    avatar.AddComponent<PlayModeBuildTrigger>();
                }
            }
        }

        private void Update()
        {
            DestroyImmediate(gameObject);
        }
    }

}
