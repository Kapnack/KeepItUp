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
    private GameObject _shopMenuGo;
    private AsyncOperationHandle<GameObject> _shopMenuAsyncHandle;
    [SerializeField] private GameObject pluginTest;
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
        creditsButton.onClick.AddListener(() => _eventSystem.Get<LoadGameplayScene>()?.Invoke());
        logsButton.onClick.AddListener(OpenLogs);
        exitButton.onClick.AddListener(ExitGame);

        StartCoroutine(CreateShopMenu());
    }

    private void Start()
    {
        ServiceProvider.GetService<GooglePlayAchievementManager>().FirstBoot();
    }

    private void OnDisable()
    {
        if (_shopMenuGo != null)
            Destroy(_shopMenuGo);

        if (_shopMenuAsyncHandle.IsDone && _shopMenuAsyncHandle.Result != null)
            Addressables.ReleaseInstance(_shopMenuAsyncHandle);
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

    private void OpenShopMenu()
    {
        if (_shopMenuGo != null)
            _shopMenuGo.SetActive(true);
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