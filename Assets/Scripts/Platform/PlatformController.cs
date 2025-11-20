using System.Collections.Generic;
using Platform.States;
using UnityEngine;

namespace Platform
{
    public class PlatformController : MonoBehaviour
    {
        private readonly List<IPlatformState> _activeStates = new();

        private float _maxAngleRotation = 0;
        private float _stateSpeed = 1;
        private Vector2 _minSize;
        private Vector2 _maxSize;
        
        private void Awake()
        {
            AddState(new RotatePlatform(_maxAngleRotation, _stateSpeed));
            AddState(new ResizePlatform(new Vector2(3f, 1f), new Vector3(5f, 1f), _stateSpeed));
            AddState(new MovePlatform(_stateSpeed));
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
                    0 => new RotatePlatform(_maxAngleRotation, _stateSpeed),
                    1 => new ResizePlatform(new Vector2(3f, 1f), new Vector3(5f, 1f), _stateSpeed),
                    2 => new MovePlatform(_stateSpeed),
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