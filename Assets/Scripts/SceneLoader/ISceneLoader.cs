using System.Threading.Tasks;
using SceneLoader;
using UnityEngine.SceneManagement;

namespace Systems.SceneLoader
{
    public interface ISceneLoader
    {
        public bool IsSceneLoaded(SceneRef sceneRef);

        /// <summary>
        /// Loads the Scene and wait until it finish loading.
        /// After that it saves it in a list so it can be unloaded later with the other scenes on the list.
        /// </summary>
        /// <param name="sceneRef"></param>
        /// <param name="mode"></param>
        public Task LoadSceneAsync(SceneRef sceneRef, LoadSceneMode mode = LoadSceneMode.Additive);

        /// <summary>
        /// Loads the Scenes and waits until they finish loading.
        /// After that it saves them in a list so they can be unloaded later.
        /// </summary>
        /// <param name="sceneRef"></param>
        /// <param name="mode"></param>
        public Task LoadSceneAsync(SceneRef[] sceneRef);

        /// <summary>
        /// Unloads all active scene saved in the list inside the class.
        /// </summary>
        public Task UnloadAll();

        /// <summary>
        /// Unloads all active scene saved in the list inside the class.
        /// Except the one you declare as exceptions
        /// </summary>
        /// <param name="exception"></param>
        public Task UnloadAll(SceneRef exception = null);

        /// <summary>
        /// Unloads all active scene saved in the list inside the class.
        /// Except the ones you declare as exceptions
        /// </summary>
        /// <param name="exeptions"></param>
        public Task UnloadAll(SceneRef[] exceptions = null);
    }
}
