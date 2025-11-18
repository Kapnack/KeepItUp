using System;
using Systems.EventSystem;
using UnityEngine;

namespace Systems.GooglePlay
{
    public class GooglePlayAchievementManager : MonoBehaviour
    {
        private const string AchievementFirstSkin = "CgkI3Licy5ILEAIQAg";
        private const string AchievementFirstBoot = "CgkI3Licy5ILEAIQAw";

        private CentralizedEventSystem _eventSystem;

        private void Awake()
        {
            ServiceProvider.SetService(this);
        }

        private void Start()
        {
            _eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();
        }

        public void FirstBoot()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _eventSystem.Get<AchievementUnlocked>()?.Invoke(AchievementFirstBoot, 100f);
#endif
        }

        public void FirstSkin()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _eventSystem.Get<AchievementUnlocked>()?.Invoke(AchievementFirstSkin, 100f);
#endif
        }
    }
}