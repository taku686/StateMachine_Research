namespace InGame.Domain.Models
{
    /// <summary>
    /// 戦闘参加者の抽象基底クラス
    /// PlayerとEnemyの共通基底
    /// </summary>
    public abstract class Combatant
    {
        public string Id { get; protected set; }
        public CombatStats Stats { get; protected set; }

        public bool IsAlive => Stats.CurrentHp > 0;

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        public void TakeDamage(int damage)
        {
            int newHp = System.Math.Max(0, Stats.CurrentHp - damage);
            Stats = Stats.WithCurrentHp(newHp);
        }

        /// <summary>
        /// 回復する
        /// </summary>
        public void Heal(int amount)
        {
            int newHp = System.Math.Min(Stats.MaxHp, Stats.CurrentHp + amount);
            Stats = Stats.WithCurrentHp(newHp);
        }

        /// <summary>
        /// HPを全回復
        /// </summary>
        public void FullHeal()
        {
            Stats = Stats.WithCurrentHp(Stats.MaxHp);
        }
    }
}

