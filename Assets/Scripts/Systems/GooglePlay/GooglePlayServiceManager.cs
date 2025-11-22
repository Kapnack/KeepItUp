#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Systems.EventSystem;
using GooglePlayGames.BasicApi;
#endif
using System;
using Systems.EventSystem;
using UnityEngine;

namespace Systems.GooglePlay
{
    public delegate void AchievementUnlocked(string achievementId, float percentageProgress);
    public delegate void ScoreBoardPoints(long score);

    public class GooglePlayServiceManager : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string ScoreBoardID = "CgkI3Licy5ILEAIQAQ";
#endif

        private bool _firstLoginManual = true;
        
        private void Awake()
        {
            ServiceProvider.SetService(this);
        }

        private void Start()
        {
            CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

#if UNITY_ANDROID && !UNITY_EDITOR
            PlayGamesPlatform.Activate();
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Instance.Authenticate(TryLogin);

            eventSystem.AddListener<AchievementUnlocked>(UpdateAchievement);
            eventSystem.AddListener<ScoreBoardPoints>(AddPointsToRanking);
#endif
        }

#if UNITY_ANDROID
        internal void TryLogin(SignInStatus status)
        {
            switch (status)
            {
                case SignInStatus.Success:
                    Debug.Log("User Name: " + PlayGamesPlatform.Instance.GetUserDisplayName());
                    Debug.Log("User ID: " + PlayGamesPlatform.Instance.GetUserId());
                    ServiceProvider.GetService<GooglePlayAchievementManager>().FirstBoot();
                    break;

                case SignInStatus.Canceled:
                    if (_firstLoginManual)
                    {
                        PlayGamesPlatform.Instance.ManuallyAuthenticate(TryLogin);
                        _firstLoginManual = false;
                    }
                    break;

                case SignInStatus.InternalError:
                    Debug.Log("Google Play Games: Login Failed - " + status);
                    break;
            }
        }
# endif

        public void AddPointsToRanking(long points)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ReportScore(points, ScoreBoardID, (success) =>
                {
                    if (success)
                    {
                        Debug.Log("Score reported successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to report score.");
                    }
                });
            }
#endif
        }

        public void ShowRanking()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGamesPlatform.Instance.IsAuthenticated())
                PlayGamesPlatform.Instance.ShowLeaderboardUI(ScoreBoardID);
#endif
        }

        public void ShowAchievements()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGamesPlatform.Instance.IsAuthenticated())
                PlayGamesPlatform.Instance.ShowAchievementsUI();
#endif
        }

        public void UpdateAchievement(string achievementId, float percentageProgress)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ReportProgress(
                    achievementId,
                    percentageProgress,
                    (success) => Debug.Log("Achievement Unlock: " + success));
            }
#endif
        }
    }
}