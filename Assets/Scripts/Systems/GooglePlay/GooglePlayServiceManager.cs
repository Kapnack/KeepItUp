#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Systems.EventSystem;
#endif
using System;
using GooglePlayGames.BasicApi;
using Systems.EventSystem;
using UnityEngine;

namespace Systems.GooglePlay
{
    public delegate void AchievementUnlocked(string achievementId, float percentageProgress);
    public delegate void ScoreBoardPoints(long score);

    public class GooglePlayServiceManager : MonoBehaviour
    {
//#if UNITY_ANDROID && !UNITY_EDITOR
        private const string ScoreBoardID = "CgkI3Licy5ILEAIQAQ";
//#endif

        private void Awake()
        {
            ServiceProvider.SetService(this);
        }

        private void Start()
        {
            CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

//#if UNITY_ANDROID && !UNITY_EDITOR
            eventSystem.AddListener<AchievementUnlocked>(UpdateAchievement);
            eventSystem.AddListener<ScoreBoardPoints>(AddPointsToRanking);

            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Instance.Authenticate(TryLogin);
//#endif
        }

        [Obsolete("Obsolete")]
        internal void TryLogin(SignInStatus status)
        {
//#if UNITY_ANDROID
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Activate();
                Social.localUser.Authenticate((bool success) =>
                {
                    if (success) Debug.Log("Google Play Games: Login Successful");
                });

                Debug.Log("User Name: " + PlayGamesPlatform.Instance.GetUserDisplayName());
                Debug.Log("User ID: " + PlayGamesPlatform.Instance.GetUserId());
            }
            else
                Debug.Log("Google Play Games: Login Failed - " + status);
//# endif
        }

        public void AddPointsToRanking(long points)
        {
//#if UNITY_ANDROID && !UNITY_EDITOR
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
//#endif
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