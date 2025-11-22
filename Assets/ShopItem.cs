using System;
using ScriptableObjects;
using ScriptableObjects.PlayerSkins;
using Systems.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public delegate void TryBuySkin(PlayerShopSkins skin, Action callback);

public delegate void TryEquipSkin(PlayerShopSkins skin, Action callback);

[Serializable]
public class ShopItem : MonoBehaviour
{
    public PlayerShopSkins skin;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private SpriteAtlas atlas;
    [SerializeField] private SpriteAtlas buttonAtlas;
    [SerializeField] private ShopVariables shopVariables;
    private CentralizedEventSystem _eventSystem;

    private const string BuyText = "Buy";
    private const string EquipText = "Equip";
    private const string EquippedText = "Equipped";

    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

        text.text = skin.unlocked ? EquipText : BuyText;

        button.onClick.AddListener(OnButtonClicked);

        icon.sprite = atlas.GetSprite(Enum.GetName(typeof(Playerskins), skin.skinName));
    }

    private void Start()
    {
        _eventSystem.AddListener<SkinChanged>(PlayerChangedSkin);
        
        if (skin.unlocked)
        {
            if (skin.equipped)
                UpdateWhenEquipped();
            else
                UpdateWhenBought();
        }
        else
            UpdateWhenLocked();
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
        skin.unlocked = false;
        skin.equipped = false;
    }

    private void UpdateWhenBought()
    {
        icon.color = Color.white;
        button.image.sprite = buttonAtlas.GetSprite("Green");
        text.text = EquipText;
        skin.unlocked = true;
        skin.equipped = false;
    }

    public void UpdateWhenEquipped()
    {
        icon.color = Color.white;
        button.image.sprite = buttonAtlas.GetSprite("Blue");
        text.text = EquippedText;
        skin.equipped = true;
    }

    private void PlayerChangedSkin(PlayerShopSkins skin)
    {
        if (this.skin == skin)
        {
            UpdateWhenEquipped();
            skin.equipped = true;
        }
        else if (this.skin.unlocked)
            UpdateWhenBought();
        else
            UpdateWhenLocked();
    }
}