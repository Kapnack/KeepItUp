using System;
using System.Collections.Generic;
using Platform.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Platform
{
    public class PlatformController : MonoBehaviour
    {
        private readonly List<IPlatformState> _activeStates = new();

        [NonSerialized] public PlatformSettings PlatformSettings;

        public void SetUp()
        {
            AddState(new RotatePlatform(PlatformSettings.MaxAngleRotation, PlatformSettings.StateSpeed));
            AddState(new ResizePlatform(new Vector2(3f, 1f), new Vector3(5f, 1f), PlatformSettings.StateSpeed));
            AddState(new MovePlatform(PlatformSettings.StateSpeed));
        }

        private void ChangeRandomState()
        {
            int randomIndex = Random.Range(0, _activeStates.Count);

            FinishState(_activeStates[randomIndex], randomIndex);
        }

        private void FinishState(IPlatformState state, int index)
        {
            state.Exit(_ => ReplaceState(state, index));
        }

        private void ReplaceState(IPlatformState state, int index)
        {
            IPlatformState newState = null;

            while (_activeStates[index] == state || newState == null)
            {
                int randomIndex = Random.Range(0, _activeStates.Count);

                newState = randomIndex switch
                {
                    0 => new RotatePlatform(PlatformSettings.MaxAngleRotation, PlatformSettings.StateSpeed),
                    1 => new ResizePlatform(new Vector2(3f, 1f), new Vector3(5f, 1f), PlatformSettings.StateSpeed),
                    2 => new MovePlatform(PlatformSettings.StateSpeed),
                    _ => null
                };
            }

            _activeStates[index] = newState;
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