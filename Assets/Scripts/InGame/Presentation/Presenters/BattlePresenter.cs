using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace InGame.Presentation.Presenters
{
    using InGame.Application.Ports.Output;
    using InGame.Domain.Models;
    using InGame.Presentation.Controllers;

    /// <summary>
    /// バトルプレゼンター（IBattleOutputPort実装）
    /// UseCaseからのイベントを受け取りViewControllerを更新
    /// </summary>
    public class BattlePresenter : IBattleOutputPort, IRewardOutputPort
    {
        private readonly BattleHUDController hudController;
        private PlayerViewController playerController;
        private List<EnemyViewController> enemyControllers = new List<EnemyViewController>();

        [Inject]
        public BattlePresenter(BattleHUDController hudController)
        {
            this.hudController = hudController;
        }

        public void SetPlayerController(PlayerViewController controller)
        {
            this.playerController = controller;
        }

        public void SetEnemyControllers(List<EnemyViewController> controllers)
        {
            this.enemyControllers = controllers;
        }

        public async UniTask OnBattleStarted(BattleContext context)
        {
            Debug.Log($"Battle Started: {context.StageId}");
            hudController.SetBattleContext(context);
            await UniTask.CompletedTask;
        }

        public async UniTask OnWaveStarted(Wave wave)
        {
            Debug.Log($"Wave {wave.WaveNumber} Started with {wave.Enemies.Count} enemies");
            hudController.UpdateWaveInfo();
            await UniTask.CompletedTask;
        }

        public async UniTask OnDamageDealt(Combatant attacker, Combatant defender, int damage, bool isCritical)
        {
            string criticalText = isCritical ? " CRITICAL!" : "";
            Debug.Log($"{attacker.Id} dealt {damage} damage to {defender.Id}{criticalText}");

            // HPバー更新
            if (defender is Player && playerController != null)
            {
                playerController.UpdateHealthBar();
                hudController.UpdatePlayerHp(defender.Stats.CurrentHp, defender.Stats.MaxHp);
            }
            else if (defender is Enemy)
            {
                var enemyController = enemyControllers.Find(ec => ec.Model == defender);
                enemyController?.UpdateHealthBar();
            }

            await UniTask.CompletedTask;
        }

        public async UniTask OnCombatantDefeated(Combatant defeated)
        {
            Debug.Log($"{defeated.Id} was defeated!");

            if (defeated is Player && playerController != null)
            {
                playerController.OnDeath();
            }
            else if (defeated is Enemy enemy)
            {
                var enemyController = enemyControllers.Find(ec => ec.Model == enemy);
                if (enemyController != null)
                {
                    enemyController.OnDeath();

                    // 報酬を追加
                    hudController.AddReward(enemy.Reward);
                }
            }

            await UniTask.CompletedTask;
        }

        public async UniTask OnWaveCompleted(Wave wave, BattleReward reward)
        {
            Debug.Log($"Wave {wave.WaveNumber} Completed! Reward: {reward}");
            hudController.AddReward(reward);
            await UniTask.CompletedTask;
        }

        public async UniTask OnBattleCompleted(BattleContext context)
        {
            Debug.Log($"Battle Completed! Status: {context.Status}");
            Debug.Log($"Total Reward: {context.TotalReward}");
            await UniTask.CompletedTask;
        }

        public async UniTask OnRewardAcquired(BattleReward reward)
        {
            Debug.Log($"Reward Acquired: {reward}");
            await UniTask.CompletedTask;
        }
    }
}

