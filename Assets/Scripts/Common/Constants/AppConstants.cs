namespace Common.Constants
{
    /// <summary>
    /// アプリケーション全体で使用される定数
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// アニメーション・フェード関連の定数
        /// </summary>
        public static class Animation
        {
            /// <summary>デフォルトのフェード時間（秒）</summary>
            public const float DefaultFadeDuration = 1f;

            /// <summary>スピナーのフレームレート（フレーム/秒）</summary>
            public const float DefaultSpinnerFrameRate = 12f;
        }

        /// <summary>
        /// シーンロード関連の定数
        /// </summary>
        public static class Scene
        {
            /// <summary>ロード進捗の閾値（Unity's AsyncOperation.progress）</summary>
            public const float LoadProgressThreshold = 0.9f;

            /// <summary>ロード進捗の手動インクリメント値</summary>
            public const float LoadProgressIncrement = 0.01f;

            /// <summary>ロード進捗更新間隔（ミリ秒）約60fps</summary>
            public const int LoadProgressUpdateInterval = 16;
        }

        /// <summary>
        /// 進捗値範囲の定数
        /// </summary>
        public static class Progress
        {
            /// <summary>進捗の最小値</summary>
            public const float Min = 0f;

            /// <summary>進捗の最大値</summary>
            public const float Max = 1f;
        }
    }
}
