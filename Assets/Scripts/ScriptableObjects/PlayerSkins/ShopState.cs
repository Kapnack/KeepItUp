using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.PlayerSkins
{
    [CreateAssetMenu(fileName = "ShopState", menuName = "ScriptableObjects/ShopState", order = 1)]
    public class ShopState : ScriptableObject
    {
        [field: SerializeField] public List<PlayerShopSkins> assetsToBuy;
        [field: SerializeField] public ShopVariables variables;
        [field: SerializeField] public PlayerCurrentSkin playerCurrentSkin;
    }
}
