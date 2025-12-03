using OutGame.Domain.Models;

namespace OutGame.Domain.Repositories
{
    /// <summary>
    /// ユーザープロフィールのリポジトリインターフェース
    /// </summary>
    public interface IUserProfileRepository
    {
        /// <summary>
        /// ユーザープロフィールを読み込む
        /// </summary>
        UserProfile Load();

        /// <summary>
        /// ユーザープロフィールを保存する
        /// </summary>
        void Save(UserProfile profile);
    }
}

