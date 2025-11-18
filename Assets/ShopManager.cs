using System;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public delegate void SkinChanged(PlayerShopSkins skinEquipped);

public class ShopManager : MonoBehaviour
{
    [SerializeField] private PlayerCurrentSkin playerCurrentSkin;
    [SerializeField] private ShopVariables shopVariables;
    [SerializeField] private Button returnButton;

    private CentralizedEventSystem _eventSystem;

    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        returnButton.onClick.AddListener(OnReturn);

        _eventSystem.AddListener<TryBuySkin>(TryBuySkin);
        _eventSystem.AddListener<TryEquipSkin>(TryEquipSkin);
    }

    private void OnDisable()
    {
        if (_eventSystem == null)
            return;
        
        _eventSystem.Unregister<TryBuySkin>();
        _eventSystem.Unregister<TryEquipSkin>();
        _eventSystem.Unregister<SkinChanged>();
    }

    private void TryBuySkin(PlayerShopSkins skin, Action callback)
    {
        if (shopVariables.CurrentPoints < shopVariables.BallsPrice)
            return;

        shopVariables.CurrentPoints -= shopVariables.BallsPrice;
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