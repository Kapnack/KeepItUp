using UnityEngine;

namespace ScriptableObjects.PlayerSkins
{
    [CreateAssetMenu(fileName = "PlayerCurrentSkin", menuName = "ScriptableObjects/PlayerCurrentSkin")]
    public class PlayerCurrentSkin : ScriptableObject
    {
        [field: SerializeField] public PlayerShopSkins currentSkin;
    }
}