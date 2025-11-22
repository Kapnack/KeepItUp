using System;
using System.Threading.Tasks;
using SceneLoader;
using Systems.EventSystem;
using Systems.GooglePlay;
using Systems.SceneLoader;
using UnityEngine;

public delegate void LoadMainMenu();

public delegate void LoadGameplayScene();

public delegate void LoadGameOver();

public delegate void LoadWinScene();

public class GameManager : MonoBehaviour
{
    private readonly CentralizedEventSystem _eventSystem = new();

    private ISceneLoader _sceneLoader;

    [SerializeField] private SceneRef mainMenuScene;
    [SerializeField] private SceneRef gameplayScene;
    [SerializeField] private SceneRef gameOverScene;
    [SerializeField] private SceneRef winScene;

    private void Awake()
    {
        _eventSystem.AddListener<LoadMainMenu>(LoadMainMenu);
        _eventSystem.AddListener<LoadGameplayScene>(LoadGameplayScene);
        _eventSystem.AddListener<LoadGameOver>(LoadGameOverScene);
        _eventSystem.AddListener<LoadWinScene>(LoadWinScene);

        ServiceProvider.SetService(_eventSystem);

        _sceneLoader = new SceneLoader.SceneLoader(new SceneRef(gameObject.scene));
    }

    private void Start()
    {
        LoadMainMenu();
    }

    private async void LoadMainMenu()
    {
        try
        {
            await TryLoadScenes(mainMenuScene);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadGameplayScene()
    {
        try
        {
            await TryLoadScenes(gameplayScene);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadGameOverScene()
    {
        try
        {
            await TryLoadScenes(gameOverScene);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void LoadWinScene()
    {
        try
        {
            await TryLoadScenes(winScene);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task TryLoadScenes(SceneRef sceneRefs)
    {
        _eventSystem.Get<OnLoadingStarted>()?.Invoke();

        try
        {
            await _sceneLoader.UnloadAll();
            await _sceneLoader.LoadSceneAsync(sceneRefs);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        _eventSystem.Get<OnLoadingEnding>()?.Invoke();
    }
}