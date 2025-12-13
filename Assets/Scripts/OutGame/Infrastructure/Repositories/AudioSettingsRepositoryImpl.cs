using OutGame.Constants;
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
        public Domain.Models.AudioSettings Load()
        {
            float bgmVolume = PlayerPrefs.GetFloat(OutGameConstants.Audio.KeyBgmVolume, OutGameConstants.Audio.DefaultBgmVolume);
            float seVolume = PlayerPrefs.GetFloat(OutGameConstants.Audio.KeySeVolume, OutGameConstants.Audio.DefaultSeVolume);
            bool hasAdjusted = PlayerPrefs.GetInt(OutGameConstants.Audio.KeyHasAdjusted, 0) == 1;

            return new Domain.Models.AudioSettings(bgmVolume, seVolume, hasAdjusted);
        }

        public void Save(Domain.Models.AudioSettings settings)
        {
            PlayerPrefs.SetFloat(OutGameConstants.Audio.KeyBgmVolume, settings.BgmVolume);
            PlayerPrefs.SetFloat(OutGameConstants.Audio.KeySeVolume, settings.SeVolume);
            PlayerPrefs.SetInt(OutGameConstants.Audio.KeyHasAdjusted, settings.HasAdjustedVolumeOnce ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}

