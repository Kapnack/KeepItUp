using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLoader
{
    public class SceneLoader : ISceneLoader, ILoadingData
    {
        private readonly SceneRef _exclude;

        public SceneLoader(SceneRef exclude)
        {
            _exclude = exclude;
            
            GetActiveScenes();
        }

        private struct SceneLoadingInfo : IEquatable<SceneLoadingInfo>
        {
            public SceneRef SceneRef;
            public AsyncOperation Operation;

            public bool Equals(SceneLoadingInfo other)
            {
                return Equals(SceneRef, other.SceneRef) && Equals(Operation, other.Operation);
            }

            public override bool Equals(object obj)
            {
                return obj is SceneLoadingInfo other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(SceneRef, Operation);
            }
        }

        private readonly List<Scene> _activeScenes = new();
        private readonly List<SceneLoadingInfo> _loadingScenes = new();


        private async Task StartUnloading(Scene activeScenes)
        {
            await SceneManager.UnloadSceneAsync(activeScenes);
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (sceneRef == null)
            {
                Debug.LogWarning("No SceneRef assigned");
                return;
            }

            if (sceneRef.Index < 0)
            {
                Debug.LogWarning($"Not Valid SceneRef. Cause SceneIndex: {sceneRef.Name} is < 0");
                return;
            }

            if (IsSceneLoaded(sceneRef))
            {
                Debug.LogWarning($"Scene: {sceneRef.Name} is already loaded.");
                return;
            }

            if (IsSceneLoading(sceneRef))
            {
                Debug.LogWarning($"Scene: {sceneRef.Name} is already loading.");
                return;
            }

            await UnloadAll();

            AsyncOperation activeScenesTask = SceneManager.LoadSceneAsync(sceneRef.Index, LoadSceneMode.Additive);

            _loadingScenes.Add(new SceneLoadingInfo { SceneRef = sceneRef, Operation = activeScenesTask });

            await activeScenesTask;

            _loadingScenes.Clear();

            _activeScenes.Add(SceneManager.GetSceneByBuildIndex(sceneRef.Index));
        }

        /// <inheritdoc/>
        public async Task LoadSceneAsync(SceneRef[] sceneRef)
        {
            List<Task> activeScenesTask = new();

            foreach (SceneRef t in sceneRef)
                activeScenesTask.Add(LoadSceneAsync(t));

            await Task.WhenAll(activeScenesTask);

            _loadingScenes.Clear();
        }

        public float GetCurrentLoadingProgress()
        {
            float progress = _loadingScenes.Sum(t => t.Operation.progress);

            return _loadingScenes.Count > 0 ? progress / _loadingScenes.Count : -1f;
        }

        /// <inheritdoc/>
        public async Task UnloadAll()
        {
            for (int i = _activeScenes.Count - 1; i >= 0; i--)
            {
                Scene scene = _activeScenes[i];

                _activeScenes.RemoveAt(i);
                await StartUnloading(scene);
            }
        }

        /// <inheritdoc/>
        public async Task UnloadAll(SceneRef exception)
        {
            for (int i = _activeScenes.Count - 1; i >= 0; i--)
            {
                if (_activeScenes[i].buildIndex == exception.Index)
                    continue;

                await StartUnloading(_activeScenes[i]);
                _activeScenes.RemoveAt(i);
            }
        }

        public async Task UnloadAll(SceneRef[] exeptions)
        {
            for (int i = _activeScenes.Count - 1; i >= 0; i--)
            {
                if (IsSceneInExceptionArray(_activeScenes[i].buildIndex, exeptions))
                    continue;

                await StartUnloading(_activeScenes[i]);
                _activeScenes.RemoveAt(i);
            }
        }

        private static bool IsSceneInExceptionArray(int index, SceneRef[] sceneRefs) =>
            sceneRefs.Any(t => index == t.Index);

        public bool IsSceneLoaded(SceneRef sceneRef) => _activeScenes.Any(t => t.buildIndex == sceneRef.Index);

        private bool IsSceneLoading(SceneRef sceneRef) => _loadingScenes.Any(t => t.SceneRef.Index == sceneRef.Index);

        private void GetActiveScenes()
        {
            int loadedScenesCount = SceneManager.sceneCount;

            for (int i = 0; i < loadedScenesCount; i++)
            {
                Scene activeScene = SceneManager.GetSceneAt(i);

                if (IsSceneException(activeScene) || IsSceneLoading(activeScene) || IsSceneLoaded(activeScene))
                    continue;

                _activeScenes.Add(activeScene);
            }

            return;

            bool IsSceneLoaded(Scene activeScene) =>
                _activeScenes.Any(t => t.buildIndex == activeScene.buildIndex);

            bool IsSceneLoading(Scene activeScene) =>
                _loadingScenes.Any(t => t.SceneRef.Index == activeScene.buildIndex);

            bool IsSceneException(Scene activeScene) =>
                _exclude.Index == activeScene.buildIndex;
        }
    }
}