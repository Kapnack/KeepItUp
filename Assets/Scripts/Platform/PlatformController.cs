using System;
using System.Collections.Generic;
using Platform.States;
using Systems.EventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Platform
{
    public class PlatformController : MonoBehaviour
    {
        private readonly List<IPlatformState> _activeStates = new();

        [NonSerialized] 
        public PlatformSettings platformSettings;

        // valores base
        private float baseAngle;
        private Vector2 baseMinSize;
        private Vector2 baseMaxSize;

        // progresión suavizada
        private float difficulty = 1f;

        private void Start()
        {
            foreach (IPlatformState state in _activeStates)
                state.Enter(gameObject);

            ServiceProvider
                .GetService<CentralizedEventSystem>()
                .AddListener<IncreaseDifficultyTime>(IncreaseDifficulty);
        }

        public void SetUp()
        {
            baseAngle = platformSettings.MaxAngleRotation;
            baseMinSize = platformSettings.MinSize;
            baseMaxSize = platformSettings.MaxSize;

            AddState(new RotatePlatform(baseAngle, platformSettings.StateSpeed));
            AddState(new ResizePlatform(baseMinSize, baseMaxSize, platformSettings.StateSpeed));
            AddState(new MovePlatform(platformSettings.StateSpeed));
        }

        private void Update()
        {
            for (int i = 0; i < _activeStates.Count; i++)
                _activeStates[i].Update();
        }

        private void IncreaseDifficulty()
        {
            difficulty += 0.02f;
            ChangeRandomState();
        }

        private void ChangeRandomState()
        {
            int index = Random.Range(0, _activeStates.Count);
            _activeStates[index].Exit(_ => ReplaceState(index));
        }

        private void ReplaceState(int index)
        {
            IPlatformState newState;

            switch (_activeStates[index])
            {
                case RotatePlatform:
                    
                    float newMaxAngle = baseAngle * difficulty;
                    newState = new RotatePlatform(newMaxAngle, platformSettings.StateSpeed);
                    break;
                
                case ResizePlatform:
                
                    Vector2 min = baseMinSize * (1f / difficulty);
                    Vector2 max = baseMaxSize * (1f / difficulty);
                    newState = new ResizePlatform(min, max, platformSettings.StateSpeed);
                    break;
                
                default:
                    newState = new MovePlatform(platformSettings.StateSpeed * difficulty);
                    break;
            }

            newState.Enter(gameObject);
            _activeStates[index] = newState;
        }

        private void AddState(IPlatformState state)
        {
            if (!_activeStates.Contains(state))
            {
                _activeStates.Add(state);
                state.Enter(gameObject);
            }
        }
    }
}
