namespace InGame.Domain.Models
{
    /// <summary>
    /// 戦闘ステータス（値オブジェクト）
    /// 不変オブジェクトとして設計
    /// </summary>
    public class CombatStats
    {
        public int MaxHp { get; }
        public int CurrentHp { get; private set; }
        public int AttackPower { get; }
        public int Defense { get; }
        public float AttackSpeed { get; }  // 攻撃間隔（秒）
        public float MoveSpeed { get; }
        public float CriticalRate { get; }  // クリティカル率（0.0～1.0）

        public CombatStats(
            int maxHp,
            int currentHp,
            int attackPower,
            int defense,
            float attackSpeed,
            float moveSpeed,
            float criticalRate)
        {
            MaxHp = maxHp;
            CurrentHp = currentHp;
            AttackPower = attackPower;
            Defense = defense;
            AttackSpeed = attackSpeed;
            MoveSpeed = moveSpeed;
            CriticalRate = criticalRate;
        }

        /// <summary>
        /// CurrentHpを変更した新しいインスタンスを返す
        /// </summary>
        public CombatStats WithCurrentHp(int newHp)
        {
            return new CombatStats(
                MaxHp,
                newHp,
                AttackPower,
                Defense,
                AttackSpeed,
                MoveSpeed,
                CriticalRate
            );
        }

        /// <summary>
        /// 複製を作成
        /// </summary>
        public CombatStats Clone()
        {
            return new CombatStats(
                MaxHp,
                CurrentHp,
                AttackPower,
                Defense,
                AttackSpeed,
                MoveSpeed,
                CriticalRate
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is CombatStats other)
            {
                return MaxHp == other.MaxHp &&
                       CurrentHp == other.CurrentHp &&
                       AttackPower == other.AttackPower &&
                       Defense == other.Defense &&
                       AttackSpeed == other.AttackSpeed &&
                       MoveSpeed == other.MoveSpeed &&
                       CriticalRate == other.CriticalRate;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(MaxHp, CurrentHp, AttackPower, Defense, AttackSpeed, MoveSpeed, CriticalRate);
        }
    }
}

