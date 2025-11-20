using System;
using UnityEngine;

namespace ScriptableObjects
{
    public class Currency : ScriptableObject
    {
       [field: NonSerialized] public int CurrentPoints { get; set; }
    }
}