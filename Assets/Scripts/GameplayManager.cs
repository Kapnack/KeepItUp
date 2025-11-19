using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using Systems.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    [Header("Game Time")]
    [SerializeField] private float gameTime = 60;
    
    [Header("Player Settings")] [SerializeField]
    private Currency currentCurrency;

    [SerializeField] private SpriteAtlas ballsSpriteAtlas;
    [SerializeField] private PlayerCurrentSkin skinUse;
    [SerializeField] private AssetReference prefab;
    [SerializeField] private TMP_Text scoreText;
    private string _scoreTextFormat;
    private Transform _playerPos;
    private GameObject _playerGo;

    [SerializeField] private List<GameObject> pointsObject;
    private Pool<IPointsSetUp> _pool;

    private Camera _mainCam;
    private CentralizedEventSystem _eventSystem;
    private bool _playerDied;


    [SerializeField] private int minPointsRequired = 20;
    private int _currentPoints;

    private void Awake()
    {
        _scoreTextFormat = scoreText.text;
        scoreText.text = string.Format(_scoreTextFormat, _currentPoints, minPointsRequired);
        
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        _mainCam = Camera.main;

        _pool = new Pool<IPointsSetUp>(pointsObject, transform);
        _pool.InitializeAll();

        StartCoroutine(CreatePlayer());
    }

    private IEnumerator CreatePlayer()
    {
        AsyncOperationHandle<GameObject> operation =
            Addressables.LoadAssetAsync<GameObject>(prefab);

        yield return operation;

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load player prefab.");
            yield break;
        }

        _playerGo = Instantiate(operation.Result);

        SceneManager.MoveGameObjectToScene(_playerGo, gameObject.scene);

        _playerPos = _playerGo.transform;
        _playerGo.transform.position = Vector3.zero;

        PlayerAssetCreator creator = _playerGo.GetComponent<PlayerAssetCreator>();

        string skinName = Enum.GetName(typeof(Playerskins), skinUse.currentSkin.skinName);

        creator.BallSprite.sprite =
            ballsSpriteAtlas.GetSprite(skinName);

        _eventSystem.AddListener<AddPoint>(OnAddPoints);
        
        Destroy(creator);
    }

    private void Start()
    {
        ITimer timer = ServiceProvider.GetService<ITimer>();

        timer.SetUpTimer(gameTime);
        timer.TimeIsUp += OnTimeEnded;

        SpawnPoint();
    }

    private void Update()
    {
        if (!_playerPos)
            return;

        Vector3 viewportPos = _mainCam.WorldToViewportPoint(_playerPos.position);

        _playerDied = viewportPos.y < 0f;

        if (_playerDied)
            _eventSystem?.Get<LoadGameOver>()?.Invoke();
    }

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(_playerGo);
    }

    private void OnAddPoints(int points)
    {
        _currentPoints += points;
        scoreText.text = string.Format(_scoreTextFormat, _currentPoints, minPointsRequired);
    }
    
    private void SavePoints(int points)
    {
        currentCurrency.CurrentPoints += points;
    }
    
    private async void SpawnPoint()
    {
        try
        {
            PoolData<IPointsSetUp> pointGo = await _pool.Get();

            pointGo.Component.SetUp(() => ReturnToPool(pointGo));

            Vector2 spawnPoint = new(Random.Range(0.3f, 0.6f), Random.Range(0.5f, 1f));

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