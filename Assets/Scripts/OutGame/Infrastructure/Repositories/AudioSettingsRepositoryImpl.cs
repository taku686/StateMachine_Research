using OutGame.Domain.Models;
using OutGame.Domain.Repositories;
using UnityEngine;

namespace OutGame.Infrastructure.Repositories
{
    /// <summary>
    /// オーディオ設定リポジトリの実装
    /// PlayerPrefsを使用してデータを永続化
    /// </summary>
    public class AudioSettingsRepositoryImpl : IAudioSettingsRepository
    {
        private const string KEY_BGM_VOLUME = "AudioSettings_BgmVolume";
        private const string KEY_SE_VOLUME = "AudioSettings_SeVolume";
        private const string KEY_HAS_ADJUSTED = "AudioSettings_HasAdjusted";

        public Domain.Models.AudioSettings Load()
        {
            float bgmVolume = PlayerPrefs.GetFloat(KEY_BGM_VOLUME, 0.8f);
            float seVolume = PlayerPrefs.GetFloat(KEY_SE_VOLUME, 0.8f);
            bool hasAdjusted = PlayerPrefs.GetInt(KEY_HAS_ADJUSTED, 0) == 1;

            return new Domain.Models.AudioSettings(bgmVolume, seVolume, hasAdjusted);
        }

        public void Save(Domain.Models.AudioSettings settings)
        {
            PlayerPrefs.SetFloat(KEY_BGM_VOLUME, settings.BgmVolume);
            PlayerPrefs.SetFloat(KEY_SE_VOLUME, settings.SeVolume);
            PlayerPrefs.SetInt(KEY_HAS_ADJUSTED, settings.HasAdjustedVolumeOnce ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}

