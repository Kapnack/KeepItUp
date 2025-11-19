#if UNITY_ANDROID || UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Systems.EventSystem;
#endif
using UnityEngine;

namespace Systems.GooglePlay
{
    public delegate void AchievementUnlocked(string achievementId, float percentageProgress);

    public class GooglePlayServiceManager : MonoBehaviour
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private const string ScoreBoardID = "CgkI3Licy5ILEAIQAQ";
#endif

        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

            eventSystem.AddListener<AchievementUnlocked>(UpdateAchievement);
#endif
        }
        
        private void Start()
        {
            TryLogin();
        }

        private void TryLogin()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            PlayGamesPlatform.Activate();

            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                if (status == SignInStatus.Success)
                    Debug.Log("Google Play Games: Login Successful");
                else
                    Debug.LogError("Google Play Games: Login Failed - " + status);
            });
#endif
        }

        public void AddPointsToRanking(long points)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (PlayGamesPlatform.Instance.IsAuthenticated())
            {
                PlayGamesPlatform.Instance.ReportScore(points, ScoreBoardID, (success) =>
                {
                    Debug.Log("ReportScore: " + success);
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

        private void UpdateAchievement(string achievementId, float percentageProgress)
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