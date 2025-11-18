using System;
using UnityEngine;

namespace ScriptableObjects
{
    public class Currency : ScriptableObject
    {
       [field: SerializeField] public int CurrentPoints { get; set; }
    }
}