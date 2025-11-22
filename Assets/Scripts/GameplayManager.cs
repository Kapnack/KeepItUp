using System;
using System.Collections;
using System.Collections.Generic;
using Platform;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using Systems.GooglePlay;
using Systems.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    [Header("Game Time")] [SerializeField] private float gameTime = 60;

    [Header("Player Settings")] [SerializeField]
    private Currency currentCurrency;

    [SerializeField] private SpriteAtlas ballsSpriteAtlas;
    [SerializeField] private PlayerCurrentSkin skinUse;
    [SerializeField] private AssetReference prefab;
    [SerializeField] private AssetReference difficultyHUDPrefab;
    [SerializeField] private AssetReference platformPrefab;
    [SerializeField] private TMP_Text scoreText;
    private string _scoreTextFormat;
    private Transform _playerPos;
    private GameObject _playerGo;
    private GameObject _difficultyHUDGo;
    private GameObject _platformGo;

    [SerializeField] private List<GameObject> pointsObject;
    private Pool<IPointsSetUp> _pool;

    private Camera _mainCam;
    private CentralizedEventSystem _eventSystem;
    private bool _playerDied;

    [SerializeField] private int minPointsRequired = 20;
    private int _currentPoints;

    private AsyncOperationHandle<GameObject> _difficultySelectorHandle;
    private AsyncOperationHandle<GameObject> _playerAsyncHandle;
    private AsyncOperationHandle<GameObject> _platformAsyncHandle;

    private void Awake()
    {
        _scoreTextFormat = scoreText.text;
        scoreText.text = string.Format(_scoreTextFormat, _currentPoints, minPointsRequired);

        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
        _mainCam = Camera.main;

        _pool = new Pool<IPointsSetUp>(pointsObject, transform);
        _pool.InitializeAll();

        _eventSystem.AddListener<AddPoint>(OnAddPoints);

        StartCoroutine(DifficultyHUD());
    }

    private IEnumerator DifficultyHUD()
    {
        _difficultySelectorHandle = Addressables.LoadAssetAsync<GameObject>(difficultyHUDPrefab);
        yield return _difficultySelectorHandle;

        if (_difficultySelectorHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _difficultyHUDGo = Instantiate(_difficultySelectorHandle.Result);

        _difficultyHUDGo.GetComponent<DifficultyManager>().Callback += StartGameplay;

        SceneManager.MoveGameObjectToScene(_difficultyHUDGo, gameObject.scene);

        _difficultyHUDGo.transform.position = Vector3.zero;
    }

    private void StartGameplay(PlatformSettings difficultyManager)
    {
        StartCoroutine(GameplayStarting(difficultyManager));
    }

    private IEnumerator GameplayStarting(PlatformSettings difficultyManager)
    {
        Time.timeScale = 0.0f;

        Coroutine routine = StartCoroutine(CreatePlatform(difficultyManager));

        yield return routine;

        routine = StartCoroutine(CreatePlayer());

        yield return routine;

        Destroy(_difficultyHUDGo);
        Addressables.ReleaseInstance(_difficultySelectorHandle);

        Time.timeScale = 1.0f;

        ITimer timer = ServiceProvider.GetService<ITimer>();

        timer.SetUpTimer(gameTime);
        timer.TimeIsUp += OnTimeEnded;
    }

    private IEnumerator CreatePlayer()
    {
        _playerAsyncHandle = Addressables.LoadAssetAsync<GameObject>(prefab);
        yield return _playerAsyncHandle;

        if (_playerAsyncHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _playerGo = Instantiate(_playerAsyncHandle.Result);

        SceneManager.MoveGameObjectToScene(_playerGo, gameObject.scene);

        _playerPos = _playerGo.transform;
        _playerGo.transform.position = Vector3.zero;

        PlayerAssetCreator creator = _playerGo.GetComponent<PlayerAssetCreator>();

        string skinName = Enum.GetName(typeof(Playerskins), skinUse.currentSkin.skinName);
        creator.BallSprite.sprite = ballsSpriteAtlas.GetSprite(skinName);

        Destroy(creator);
    }

    private IEnumerator CreatePlatform(PlatformSettings difficultyManager)
    {
        _platformAsyncHandle = Addressables.LoadAssetAsync<GameObject>(platformPrefab);
        yield return _platformAsyncHandle;

        if (_platformAsyncHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _platformGo = Instantiate(_platformAsyncHandle.Result);

        PlatformController platformController = _platformGo.GetComponent<PlatformController>();

        platformController.platformSettings = difficultyManager;
        platformController.SetUp();

        SceneManager.MoveGameObjectToScene(_platformGo, gameObject.scene);

        _platformGo.transform.position = new Vector3(0.0f, -3.0f, 0.0f);
    }

    private void Start()
    {
        SpawnPoint();
    }

    private void Update()
    {
        if (!_playerPos)
            return;

        Vector3 viewportPos = _mainCam.WorldToViewportPoint(_playerPos.position);
        _playerDied = viewportPos.y < 0f;

        if (!_playerDied)
            return;

        SavePoints(_currentPoints);
        _eventSystem?.Get<LoadGameOver>()?.Invoke();
    }

    private void OnDestroy()
    {
        if (_playerGo != null)
            Destroy(_playerGo);

        if (_platformGo != null)
            Destroy(_platformGo);

        if (_playerAsyncHandle.IsDone && _playerAsyncHandle.Result != null)
        {
            Addressables.ReleaseInstance(_playerAsyncHandle.Result);
            _playerAsyncHandle = default; // optional, clears the handle
        }


        if (_platformAsyncHandle.IsDone && _platformAsyncHandle.Result != null)
        {
            Addressables.ReleaseInstance(_platformAsyncHandle.Result);
            _platformAsyncHandle = default;
        }
    }

    private void OnAddPoints(int points)
    {
        _currentPoints += points;
        scoreText.text = string.Format(_scoreTextFormat, _currentPoints, minPointsRequired);
    }

    private void SavePoints(int points)
    {
        ServiceProvider.GetService<GooglePlayServiceManager>().AddPointsToRanking(points);
        currentCurrency.CurrentPoints += points;
        Debug.Log("Saved points: " + points);
    }

    private async void SpawnPoint()
    {
        try
        {
            PoolData<IPointsSetUp> pointGo = await _pool.Get();

            pointGo.Component.SetUp(() => ReturnToPool(pointGo));

            Vector2 spawnPoint = new
            (
                Random.Range(0.3f, 0.6f),
                Random.Range(0.5f, 0.7f)
            );

            spawnPoint = _mainCam.ViewportToWorldPoint(spawnPoint);
            pointGo.Obj.transform.position = spawnPoint;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void ReturnToPool(PoolData<IPointsSetUp> point)
    {
        _pool.Return(point);
        SpawnPoint();
    }

    private void OnTimeEnded()
    {
        SavePoints(_currentPoints);

        if (_currentPoints < minPointsRequired)
            _eventSystem.Get<LoadGameOver>()?.Invoke();
        else
            _eventSystem.Get<LoadWinScene>()?.Invoke();
    }
}