using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Shop Variables", menuName = "ScriptableObjects/Shop Variables")]
    public class ShopVariables : Currency
    {
        [field: SerializeField] public int BallsPrice { get; private set; }
    }
}