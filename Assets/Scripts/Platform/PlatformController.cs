using System.Collections.Generic;
using Platform.States;
using UnityEngine;

namespace Platform
{
    public class PlatformController : MonoBehaviour
    {
        private readonly List<IPlatformState> _activeStates = new();

        private void Awake()
        {
            AddState(new RotatePlatform(60f, 1.5f));
            AddState(new ResizePlatform(new Vector2(3f, 1f), new Vector3(5f, 1f), 1.5f));
            AddState(new MovePlatform(2.5f));
        }

        private void Start()
        {
            foreach (IPlatformState state in _activeStates)
                state.Enter(gameObject);
        }

        private void Update()
        {
            foreach (IPlatformState state in _activeStates)
                state.Update();
        }

        private void AddState(IPlatformState state)
        {
            if (_activeStates.Contains(state))
                return;

            _activeStates.Add(state);
            state.Enter(gameObject);
        }

        private void ExitAState(IPlatformState state) => state.Exit(RemoveState);

        private void RemoveState(IPlatformState state) => _activeStates.Remove(state);
    }
}