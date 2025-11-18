using System;
using UnityEngine;

namespace ScriptableObjects.PlayerSkins
{
    [Serializable]
    public enum Playerskins
    {
        DefaultSkin = 0,
        YellowSkin,
        PurpleSkin,
        GreenSkin,
        BiSkin,
        FaceBall
    }
    
    [CreateAssetMenu(fileName = "PlayerSkin", menuName = "ScriptableObjects/PlayerSkin")]
    public class PlayerShopSkins : ScriptableObject
    {
        [field: SerializeField] public Playerskins skinName;
        [NonSerialized] public bool unlocked = false;
        [NonSerialized] public bool equipped = false;
    }
}
