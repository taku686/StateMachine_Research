using Zenject;

namespace InGame.Domain.Models
{
    /// <summary>
    /// 敵エンティティ
    /// Zenject Factoryで生成
    /// </summary>
    public class Enemy : Combatant
    {
        public EnemyType Type { get; private set; }
        public BattleReward Reward { get; private set; }

        // Zenject用デフォルトコンストラクタ
        public Enemy()
        {
        }

        /// <summary>
        /// 初期化メソッド（Factoryから呼ばれる）
        /// </summary>
        public void Initialize(string id, EnemyType type, CombatStats stats, BattleReward reward)
        {
            Id = id;
            Type = type;
            Stats = stats;
            Reward = reward;
        }

        /// <summary>
        /// Zenject PlaceholderFactory定義
        /// </summary>
        public class Factory : PlaceholderFactory<string, EnemyType, CombatStats, BattleReward, Enemy>
        {
        }
    }

    /// <summary>
    /// 敵の種類
    /// </summary>
    public enum EnemyType
    {
        Normal,   // 通常敵
        Elite,    // エリート敵（強い）
        Boss      // ボス敵（最強）
    }
}

