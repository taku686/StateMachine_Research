namespace OutGame.Constants
{
    /// <summary>
    /// OutGameシーン固有の定数
    /// </summary>
    public static class OutGameConstants
    {
        /// <summary>
        /// オーディオ設定関連の定数
        /// </summary>
        public static class Audio
        {
            /// <summary>BGMのデフォルト音量</summary>
            public const float DefaultBgmVolume = 0.8f;

            /// <summary>SEのデフォルト音量</summary>
            public const float DefaultSeVolume = 0.8f;

            /// <summary>PlayerPrefs - BGM音量のキー</summary>
            public const string KeyBgmVolume = "AudioSettings_BgmVolume";

            /// <summary>PlayerPrefs - SE音量のキー</summary>
            public const string KeySeVolume = "AudioSettings_SeVolume";

            /// <summary>PlayerPrefs - 音量調整済みフラグのキー</summary>
            public const string KeyHasAdjusted = "AudioSettings_HasAdjusted";
        }
    }
}
