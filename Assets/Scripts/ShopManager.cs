using System;
using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using Systems.GooglePlay;
using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.UI;

public delegate void SkinChanged(PlayerShopSkins skinEquipped);

public class ShopManager : MonoBehaviour
{
    [HideInInspector] public PlayerCurrentSkin playerCurrentSkin;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text currentPointsText;
    [SerializeField] private List<ShopItem> itemsInShop;

    private string _currentMoneyTextFormat;

    private CentralizedEventSystem _eventSystem;

    private GooglePlayAchievementManager _googlePlayAchievementManager;
    
    private ShopVariables _shopVariables;
    
    private RewardedAdsButton _rewardedAdsButton;
    
    private AnalyticsManager _analyticsManager;
    
    public ShopVariables ShopVariables
    {
        get => _shopVariables;
        set
        {
            _shopVariables = value;
            
            currentPointsText.text = string.Format(_currentMoneyTextFormat, ShopVariables.CurrentPoints);
        }
    }
    
    private void Awake()
    {
        _currentMoneyTextFormat = currentPointsText.text;

        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
        _googlePlayAchievementManager = ServiceProvider.GetService<GooglePlayAchievementManager>();
        returnButton.onClick.AddListener(OnReturn);

        _eventSystem.AddListener<TryBuySkin>(TryBuySkin);
        _eventSystem.AddListener<TryEquipSkin>(TryEquipSkin);
        
        playerCurrentSkin.currentSkin.equipped = true;
        playerCurrentSkin.currentSkin.unlocked = true;
        
        _rewardedAdsButton = GetComponent<RewardedAdsButton>();
        
        _analyticsManager = ServiceProvider.GetService<AnalyticsManager>();
    }

    private void Start()
    {
        foreach (ShopItem item in itemsInShop)
        {
            if (item.skin != playerCurrentSkin.currentSkin) 
                continue;
            
            item.UpdateWhenEquipped();
            break;
        }
        
        _rewardedAdsButton.LoadAd();
        
        _eventSystem.AddListener<AdCompleted>(AddPointsAfterAdd);
    }

    private void AddPointsAfterAdd()
    {
        _shopVariables.CurrentPoints += 20;
        currentPointsText.text = string.Format(_currentMoneyTextFormat, ShopVariables.CurrentPoints);
        
        _rewardedAdsButton.LoadAd();
    }

    private void OnDestroy()
    {
        _eventSystem.Unregister<TryBuySkin>();
        _eventSystem.Unregister<TryEquipSkin>();
        _eventSystem.Unregister<SkinChanged>();
    }

    private void TryBuySkin(PlayerShopSkins skin, Action callback)
    {
        if (ShopVariables.CurrentPoints < ShopVariables.BallsPrice)
            return;

        _googlePlayAchievementManager.FirstSkin();
        ShopVariables.CurrentPoints -= ShopVariables.BallsPrice;
        currentPointsText.text = string.Format(_currentMoneyTextFormat, ShopVariables.CurrentPoints);
        skin.unlocked = true;
        callback?.Invoke();

        ++_shopVariables.SkinsBought;

        CustomEvent skinsBought = new("userBoughtSkin")
        {
            { "SkinsBought", _shopVariables.SkinsBought }
            
        };
        
        _analyticsManager.SendData(skinsBought);

        skinsBought = new CustomEvent("skinBought")
        {
            { "skinName", Enum.GetName(typeof(Playerskins), skin.skinName.ToString()) }
        };
        
        _analyticsManager.SendData(skinsBought);
    }

    private void TryEquipSkin(PlayerShopSkins skin, Action callback)
    {
        if (!skin.unlocked)
            return;

        if (playerCurrentSkin.currentSkin != null)
            playerCurrentSkin.currentSkin.equipped = false;

        playerCurrentSkin.currentSkin = skin;

        _eventSystem.Get<SkinChanged>()?.Invoke(skin);

        callback?.Invoke();
    }

    private void OnReturn()
    {
        gameObject.SetActive(false);
        _eventSystem.Unregister<AdCompleted>();
    }
}