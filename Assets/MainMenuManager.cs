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
    [SerializeField] private GameObject pluginTest;

    private void Awake()
    {
        CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        pluginTest.SetActive(false);

        playButton.onClick.AddListener(() => eventSystem.Get<LoadGameplayScene>()?.Invoke());
        shopButton.onClick.AddListener(OpenShopMenu);
#if !UNITY_EDITOR && UNITY_ANDROID
        achievementsButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowAchievements);
        leaderboardButton.onClick.AddListener(ServiceProvider.GetService<GooglePlayServiceManager>().ShowRanking);
#endif
        creditsButton.onClick.AddListener(() => eventSystem.Get<LoadGameplayScene>()?.Invoke());
        logsButton.onClick.AddListener(OpenLogs);
        exitButton.onClick.AddListener(ExitGame);

        StartCoroutine(CreateShopMenu());
    }

    private void OnDisable()
    {
        Addressables.ReleaseInstance(_shopMenuGo);
    }

    private IEnumerator CreateShopMenu()
    {
        AsyncOperationHandle<GameObject> operation =
            Addressables.LoadAssetAsync<GameObject>(shopMenu);

        yield return operation;

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _shopMenuGo = Instantiate(operation.Result);

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