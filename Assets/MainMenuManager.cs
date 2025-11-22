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

    [Header("Shop Menu")] [SerializeField] private AssetReference shopMenu;
    [SerializeField] private ShopVariables shopVariables;
    [SerializeField] private PlayerCurrentSkin playerCurrentSkin;

    private AsyncOperationHandle<GameObject> _shopMenuAsyncHandle;
    private GameObject _shopMenuGo;


    [SerializeField] private GameObject pluginTest;

    [Header("Credits")] [SerializeField] private AssetReference creditsPrefab;
    private AsyncOperationHandle<GameObject> _creditsAsyncHandle;
    private GameObject _creditsGo;

    private CentralizedEventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        pluginTest.SetActive(false);

        playButton.onClick.AddListener(() => _eventSystem.Get<LoadGameplayScene>()?.Invoke());
        shopButton.onClick.AddListener(OpenShopMenu);
#if !UNITY_EDITOR && UNITY_ANDROID
        achievementsButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowAchievements);
        leaderboardButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowRanking);
#endif
        creditsButton.onClick.AddListener(OpenCredits);
        logsButton.onClick.AddListener(OpenLogs);
        exitButton.onClick.AddListener(ExitGame);

        StartCoroutine(CreateShopMenu());
        StartCoroutine(CreateCredits());
    }

    private void OnDisable()
    {
        if (_shopMenuGo != null)
            Destroy(_shopMenuGo);

        if (_creditsGo != null)
            Destroy(_creditsGo);

        if (_shopMenuAsyncHandle.IsDone && _shopMenuAsyncHandle.Result != null)
            Addressables.ReleaseInstance(_shopMenuAsyncHandle);

        if (_creditsAsyncHandle.IsDone && _creditsAsyncHandle.Result != null)
            Addressables.ReleaseInstance(_creditsAsyncHandle);
    }

    private IEnumerator CreateShopMenu()
    {
        _shopMenuAsyncHandle =
            Addressables.LoadAssetAsync<GameObject>(shopMenu);

        yield return _shopMenuAsyncHandle;

        if (_shopMenuAsyncHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _shopMenuGo = Instantiate(_shopMenuAsyncHandle.Result);

        _shopMenuGo.GetComponent<ShopManager>().ShopVariables = shopVariables;
        _shopMenuGo.GetComponent<ShopManager>().playerCurrentSkin = playerCurrentSkin;
        _shopMenuGo.SetActive(false);

        SceneManager.MoveGameObjectToScene(_shopMenuGo, gameObject.scene);

        _shopMenuGo.transform.position = Vector3.zero;
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

    private void OpenShopMenu()
    {
        if (_shopMenuGo != null)
            _shopMenuGo.SetActive(true);
    }

    private void OpenCredits()
    {
        _creditsGo.SetActive(true);
    }

    private void OpenLogs()
    {
        pluginTest.SetActive(true);
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