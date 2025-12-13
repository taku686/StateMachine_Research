namespace InGame.Constants
{
    /// <summary>
    /// InGameシーン固有の定数
    /// </summary>
    public static class InGameConstants
    {
        /// <summary>
        /// 戦闘関連の定数
        /// </summary>
        public static class Combat
        {
            /// <summary>クリティカルヒット時のダメージ倍率</summary>
            public const float CriticalMultiplier = 1.5f;

            /// <summary>デフォルトのクリティカル率</summary>
            public const float DefaultCriticalRate = 0.1f;

            /// <summary>最低保証ダメージ</summary>
            public const int MinDamage = 1;
        }

        /// <summary>
        /// UI配置・アニメーション関連の定数
        /// </summary>
        public static class UI
        {
            /// <summary>ヘルスバーアンカーの高さ（ローカル座標Y）</summary>
            public const float HealthBarAnchorHeight = 1.5f;

            /// <summary>ヘルスバーのアニメーション時間（秒）</summary>
            public const float HealthBarAnimationDuration = 0.3f;

            /// <summary>ヘルスバーグラデーション - 低HP閾値（赤）</summary>
            public const float HealthBarGradientLowThreshold = 0f;

            /// <summary>ヘルスバーグラデーション - 中HP閾値（黄）</summary>
            public const float HealthBarGradientMidThreshold = 0.5f;

            /// <summary>ヘルスバーグラデーション - 高HP閾値（緑）</summary>
            public const float HealthBarGradientHighThreshold = 1f;
        }

        /// <summary>
        /// プレイヤー初期ステータス関連の定数
        /// </summary>
        public static class Player
        {
            /// <summary>基礎HP</summary>
            public const int BaseHp = 100;

            /// <summary>レベル毎のHP増加量</summary>
            public const int HpPerLevel = 10;

            /// <summary>基礎攻撃力</summary>
            public const int BaseAttackPower = 10;

            /// <summary>レベル毎の攻撃力増加量</summary>
            public const int AttackPowerPerLevel = 2;

            /// <summary>基礎防御力</summary>
            public const int BaseDefense = 5;

            /// <summary>レベル毎の防御力増加量</summary>
            public const int DefensePerLevel = 1;

            /// <summary>デフォルトの攻撃速度</summary>
            public const float DefaultAttackSpeed = 1f;

            /// <summary>デフォルトの移動速度</summary>
            public const float DefaultMoveSpeed = 3f;
        }
    }
}
