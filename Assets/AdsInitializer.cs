using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    private const string AndroidGameId = "6003273";
    private const string IOSGameId = "6003272";

    [SerializeField] private bool testMode = true;
    private string _gameId;

    private void Awake()
    {
        InitializeAds();
    }

    private void InitializeAds()
    {
#if UNITY_IOS
    _gameId = IOSGameId;
#elif UNITY_ANDROID
        _gameId = AndroidGameId;
#elif UNITY_EDITOR
    _gameId = AndroidGameId; //Only for testing the functionality in the Editor
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, testMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}