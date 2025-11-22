using System;
using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using TMPro;
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

    private ShopVariables _shopVariables;
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

        returnButton.onClick.AddListener(OnReturn);

        _eventSystem.AddListener<TryBuySkin>(TryBuySkin);
        _eventSystem.AddListener<TryEquipSkin>(TryEquipSkin);
        
        playerCurrentSkin.currentSkin.equipped = true;
        playerCurrentSkin.currentSkin.unlocked = true;
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

        ShopVariables.CurrentPoints -= ShopVariables.BallsPrice;
        currentPointsText.text = string.Format(_currentMoneyTextFormat, ShopVariables.CurrentPoints);
        skin.unlocked = true;
        callback?.Invoke();
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
    }
}