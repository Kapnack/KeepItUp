#if UNITY_ANDROID
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

        private void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CentralizedEventSystem eventSystem = ServiceProvider.GetService<CentralizedEventSystem>();

            eventSystem.AddListener<AchievementUnlocked>(UpdateAchievement);
            
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Instance.Authenticate(TryLogin);
#endif
        }

        internal void TryLogin(SignInStatus status)
        {
#if UNITY_ANDROID
            if (status == SignInStatus.Success)
                {
                    Debug.Log("Google Play Games: Login Successful");
                    
                    Debug.Log("User Name: " + PlayGamesPlatform.Instance.GetUserDisplayName());
                    Debug.Log("User ID: " + PlayGamesPlatform.Instance.GetUserId());
                }
                else
                    Debug.Log("Google Play Games: Login Failed - " + status);
# endif
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