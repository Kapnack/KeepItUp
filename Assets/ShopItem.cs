using System;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public delegate void TryBuySkin(PlayerShopSkins skin, Action callback);

public delegate void TryEquipSkin(PlayerShopSkins skin, Action callback);

public class ShopItem : MonoBehaviour
{
    [SerializeField] private PlayerShopSkins skin;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private SpriteAtlas buttonAtlas;

    private CentralizedEventSystem _eventSystem;

    private const string BuyText = "Buy";
    private const string EquipText = "Equip";
    private const string EquippedText = "Equipped";

    private void Start()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        text.text = skin.unlocked ? EquipText : BuyText;

        button.onClick.AddListener(OnButtonClicked);

        icon.sprite = atlas.GetSprite(Enum.GetName(typeof(Playerskins), skin.skinName));

        if (skin.unlocked)
            UpdateWhenBought();
        else if (skin.equipped)
            UpdateWhenEquipped();
        else
            UpdateWhenLocked();

        _eventSystem.AddListener<SkinChanged>(PlayerChangedSkin);
    }

    private void OnButtonClicked()
    {
        if (skin.unlocked)
            _eventSystem.Get<TryEquipSkin>()?.Invoke(skin, UpdateWhenEquipped);
        else
            _eventSystem.Get<TryBuySkin>()?.Invoke(skin, UpdateWhenBought);
    }

    private void UpdateWhenLocked()
    {
        icon.color = Color.gray;
        button.image.sprite = buttonAtlas.GetSprite("Red");
        text.text = BuyText;
    }

    private void UpdateWhenBought()
    {
        icon.color = Color.white;
        button.image.sprite = buttonAtlas.GetSprite("Green");
        text.text = EquipText;
    }

    private void UpdateWhenEquipped()
    {
        icon.color = Color.white;
        button.image.sprite = buttonAtlas.GetSprite("Blue");
        text.text = EquippedText;
    }

    private void PlayerChangedSkin(PlayerShopSkins skin)
    {
        if (!this.skin.unlocked)
            return;

        if (this.skin == skin)
            UpdateWhenEquipped();
        else
            UpdateWhenBought();
    }
}