namespace OutGame.Domain.Models
{
    /// <summary>
    /// オーディオ設定のエンティティ
    /// ビジネスルールを持つドメインモデル
    /// </summary>
    public class AudioSettings
    {
        public float BgmVolume { get; private set; }
        public float SeVolume { get; private set; }
        public bool HasAdjustedVolumeOnce { get; private set; }

        public AudioSettings(float bgmVolume = 0.8f, float seVolume = 0.8f, bool hasAdjustedVolumeOnce = false)
        {
            BgmVolume = bgmVolume;
            SeVolume = seVolume;
            HasAdjustedVolumeOnce = hasAdjustedVolumeOnce;
        }

        /// <summary>
        /// BGM音量を更新
        /// </summary>
        public void UpdateBgmVolume(float volume)
        {
            if (IsValidVolume(volume))
            {
                BgmVolume = volume;
            }
        }

        /// <summary>
        /// SE音量を更新
        /// </summary>
        public void UpdateSeVolume(float volume)
        {
            if (IsValidVolume(volume))
            {
                SeVolume = volume;
            }
        }

        /// <summary>
        /// 音量調整済みフラグを立てる
        /// </summary>
        public void MarkVolumeAdjusted()
        {
            HasAdjustedVolumeOnce = true;
        }

        /// <summary>
        /// 音量の妥当性チェック（ビジネスルール）
        /// </summary>
        public bool IsValidVolume(float volume)
        {
            return volume >= 0f && volume <= 1f;
        }
    }
}

