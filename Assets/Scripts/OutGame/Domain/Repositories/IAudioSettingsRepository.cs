using OutGame.Domain.Models;

namespace OutGame.Domain.Repositories
{
    /// <summary>
    /// オーディオ設定のリポジトリインターフェース
    /// </summary>
    public interface IAudioSettingsRepository
    {
        /// <summary>
        /// オーディオ設定を読み込む
        /// </summary>
        AudioSettings Load();

        /// <summary>
        /// オーディオ設定を保存する
        /// </summary>
        void Save(AudioSettings settings);
    }
}

