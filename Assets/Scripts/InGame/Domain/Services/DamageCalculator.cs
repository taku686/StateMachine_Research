using InGame.Constants;
using UnityEngine;

namespace InGame.Domain.Services
{
    using InGame.Domain.Models;

    /// <summary>
    /// ダメージ計算サービス実装
    /// </summary>
    public class DamageCalculator : IDamageCalculator
    {
        /// <summary>
        /// ダメージを計算
        /// </summary>
        public int Calculate(Combatant attacker, Combatant defender)
        {
            // 基本ダメージ = 攻撃力 - 防御力
            int baseDamage = attacker.Stats.AttackPower - defender.Stats.Defense;

            // 最低1ダメージは保証
            baseDamage = Mathf.Max(InGameConstants.Combat.MinDamage, baseDamage);

            // クリティカル判定
            if (IsCriticalHit(attacker))
            {
                baseDamage = (int)(baseDamage * InGameConstants.Combat.CriticalMultiplier);
            }

            return baseDamage;
        }

        /// <summary>
        /// クリティカルヒット判定
        /// </summary>
        public bool IsCriticalHit(Combatant attacker)
        {
            return Random.value < attacker.Stats.CriticalRate;
        }
    }
}

