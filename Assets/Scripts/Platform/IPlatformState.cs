using System;
using UnityEngine;

namespace Platform
{
    public interface IPlatformState
    {
        void Enter(GameObject owner);
        void Update();
        void Exit(Action<IPlatformState> callback);
    }
}