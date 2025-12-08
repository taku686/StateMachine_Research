using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.Application.UseCases
{
    using InGame.Application.Ports.Input;
    using InGame.Application.Ports.Output;
    using InGame.Domain.Models;
    using InGame.Domain.Services;

    /// <summary>
    /// 戦闘処理ユースケース
    /// </summary>
    public class ProcessCombatInteractor : IAttackEnemyInputPort
    {
        private readonly IDamageCalculator damageCalculator;
        private readonly IBattleOutputPort outputPort;

        [Inject]
        public ProcessCombatInteractor(
            IDamageCalculator damageCalculator,
            IBattleOutputPort outputPort)
        {
            this.damageCalculator = damageCalculator;
            this.outputPort = outputPort;
        }

        public async UniTask Execute(Combatant attacker, Combatant defender)
        {
            if (!attacker.IsAlive || !defender.IsAlive)
            {
                return;
            }

            // ダメージ計算
            int damage = damageCalculator.Calculate(attacker, defender);
            bool isCritical = damageCalculator.IsCriticalHit(attacker);

            // ダメージ適用
            defender.TakeDamage(damage);

            // Presenterに通知
            await outputPort.OnDamageDealt(attacker, defender, damage, isCritical);

            // 撃破判定
            if (!defender.IsAlive)
            {
                Debug.Log($"{defender.Id} defeated!");
                await outputPort.OnCombatantDefeated(defender);
            }
        }
    }
}

