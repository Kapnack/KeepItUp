using System.Collections;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using Systems.GooglePlay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button achievementsButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button logsButton;
    [SerializeField] private Button exitButton;

    [Header("Shop Menu")] [SerializeField] private GameObject shopMenu;
    [SerializeField] private ShopVariables shopVariables;
    [SerializeField] private PlayerCurrentSkin playerCurrentSkin;

    [Header("Credits")] [SerializeField] private AssetReference creditsPrefab;
    private AsyncOperationHandle<GameObject> _creditsAsyncHandle;
    private GameObject _creditsGo;

    [Header("Plugin")] [SerializeField] private AssetReference pluginPrefab;
    private AsyncOperationHandle<GameObject> _pluginTestAsyncHandle;
    private GameObject _pluginTestGo;

    private CentralizedEventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        playButton.onClick.AddListener(() => _eventSystem.Get<LoadGameplayScene>()?.Invoke());
        shopButton.onClick.AddListener(OpenShopMenu);

#if !UNITY_EDITOR && UNITY_ANDROID
        achievementsButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowAchievements);
        leaderboardButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowRanking);
#endif
        creditsButton.onClick.AddListener(OpenCredits);
        logsButton.onClick.AddListener(OpenLogs);
        exitButton.onClick.AddListener(ExitGame);

        StartCoroutine(CreateCredits());
        StartCoroutine(CreateLogs());
    }

    private void Start()
    {
        shopMenu = Instantiate(shopMenu);
        ShopManager shopManager = shopMenu.GetComponent<ShopManager>();

        shopManager.playerCurrentSkin = playerCurrentSkin;
        shopManager.ShopVariables = shopVariables;
        
        shopMenu.SetActive(false);
    }

    private void OnDisable()
    {
        if (shopMenu != null)
            Destroy(shopMenu);

        if (_creditsGo != null)
            Destroy(_creditsGo);

        if (_pluginTestGo != null)
            Destroy(_pluginTestGo);

        if (_creditsAsyncHandle.IsValid())
        {
            if (_creditsAsyncHandle.IsDone)
            {
                Addressables.ReleaseInstance(_creditsAsyncHandle);
            }

            _creditsAsyncHandle = default;
        }

        if (_pluginTestAsyncHandle.IsValid())
        {
            if (_pluginTestAsyncHandle.IsDone)
            {
                Addressables.ReleaseInstance(_pluginTestAsyncHandle);
            }

            _pluginTestAsyncHandle = default;
        }
    }

    private IEnumerator CreateCredits()
    {
        _creditsAsyncHandle =
            Addressables.LoadAssetAsync<GameObject>(creditsPrefab);

        yield return _creditsAsyncHandle;

        if (_creditsAsyncHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _creditsGo = Instantiate(_creditsAsyncHandle.Result);
        _creditsGo.SetActive(false);

        SceneManager.MoveGameObjectToScene(_creditsGo, gameObject.scene);

        _creditsGo.transform.position = Vector3.zero;
    }

    private IEnumerator CreateLogs()
    {
        _pluginTestAsyncHandle =
            Addressables.LoadAssetAsync<GameObject>(pluginPrefab);

        yield return _pluginTestAsyncHandle;

        if (_pluginTestAsyncHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _pluginTestGo = Instantiate(_pluginTestAsyncHandle.Result);

        _pluginTestGo.GetComponent<PluginTest>().SetUp();
        _pluginTestGo.SetActive(false);

        SceneManager.MoveGameObjectToScene(_pluginTestGo, gameObject.scene);

        _pluginTestGo.transform.position = Vector3.zero;
    }

    private void OpenShopMenu()
    {
        if (shopMenu != null)
            shopMenu.SetActive(true);
    }

    private void OpenCredits()
    {
        _creditsGo.SetActive(true);
    }

    private void OpenLogs()
    {
        _pluginTestGo.SetActive(true);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}