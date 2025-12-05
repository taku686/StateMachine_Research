using OutGame.Domain.Models;
using OutGame.Domain.Repositories;
using UnityEngine;

namespace OutGame.Infrastructure.Repositories
{
    /// <summary>
    /// ユーザープロフィールリポジトリの実装
    /// PlayerPrefsを使用してデータを永続化
    /// </summary>
    public class UserProfileRepositoryImpl : IUserProfileRepository
    {
        private const string KEY_USER_ID = "UserProfile_UserId";
        private const string KEY_USER_NAME = "UserProfile_UserName";
        private const string KEY_LEVEL = "UserProfile_Level";
        private const string KEY_GOLD = "UserProfile_Gold";

        public UserProfile Load()
        {
            string userId = PlayerPrefs.GetString(KEY_USER_ID, "user_001");
            string userName = PlayerPrefs.GetString(KEY_USER_NAME, "Player");
            int level = PlayerPrefs.GetInt(KEY_LEVEL, 1);
            int gold = PlayerPrefs.GetInt(KEY_GOLD, 1000);

            return new UserProfile(userId, userName, level, gold);
        }

        public void Save(UserProfile profile)
        {
            PlayerPrefs.SetString(KEY_USER_ID, profile.UserId);
            PlayerPrefs.SetString(KEY_USER_NAME, profile.UserName);
            PlayerPrefs.SetInt(KEY_LEVEL, profile.Level);
            PlayerPrefs.SetInt(KEY_GOLD, profile.Gold);
            PlayerPrefs.Save();
        }
    }
}

