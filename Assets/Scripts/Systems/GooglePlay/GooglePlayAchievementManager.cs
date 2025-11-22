using System;
using Systems.EventSystem;
using UnityEngine;

namespace Systems.GooglePlay
{
    public class GooglePlayAchievementManager : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string AchievementFirstSkin = "CgkI3Licy5ILEAIQAg";
        private const string AchievementFirstBoot = "CgkI3Licy5ILEAIQAw";
#endif
        private GooglePlayServiceManager _googlePlayServiceManager;

        private void Awake()
        {
            _googlePlayServiceManager = GetComponent<GooglePlayServiceManager>();
            ServiceProvider.SetService(this);
        }

        public void FirstBoot()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
           _googlePlayServiceManager.UpdateAchievement(AchievementFirstBoot, 100f);
#endif
        }

        public void FirstSkin()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _googlePlayServiceManager.UpdateAchievement(AchievementFirstSkin, 100f);
#endif
        }
    }
}