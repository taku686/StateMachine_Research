using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace InGame.States
{
    using InGame.Application.Ports.Input;
    using InGame.Presentation.Controllers;
    using InGame.Presentation.Presenters;
    using InGame.StateMachine;

    /// <summary>
    /// バトル中State
    /// Update()でController呼び出し
    /// </summary>
    public class BattleState : BaseInGameState
    {
        private readonly BattlePresenter battlePresenter;
        private readonly List<EnemyViewController> enemyControllers;
        private readonly ICompleteBattleInputPort completeBattleInteractor;
        private readonly InGameStateMachine stateMachine;

        [Inject]
        public BattleState(
            BattlePresenter battlePresenter,
            List<EnemyViewController> enemyControllers,
            ICompleteBattleInputPort completeBattleInteractor,
            InGameStateMachine stateMachine)
        {
            this.battlePresenter = battlePresenter;
            this.enemyControllers = enemyControllers;
            this.completeBattleInteractor = completeBattleInteractor;
            this.stateMachine = stateMachine;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("Battle Started");
            await UniTask.CompletedTask;
        }

        public override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            var playerController = battlePresenter.PlayerController;

            // プレイヤー更新
            if (playerController != null)
            {
                playerController.Update(deltaTime);

                // 敵をターゲット設定（最も近い生きている敵）
                var aliveEnemy = FindNearestAliveEnemy();
                playerController.SetTarget(aliveEnemy);
            }

            // 全敵更新
            foreach (var enemyController in enemyControllers)
            {
                if (enemyController != null)
                {
                    enemyController.Update(deltaTime);
                }
            }

            // バトル終了判定
            CheckBattleEnd().Forget();
        }

        private Domain.Models.Enemy FindNearestAliveEnemy()
        {
            foreach (var controller in enemyControllers)
            {
                if (controller != null && controller.IsAlive)
                {
                    return controller.Model;
                }
            }
            return null;
        }

        private async UniTaskVoid CheckBattleEnd()
        {
            var playerController = battlePresenter.PlayerController;

            // 全敵撃破チェック
            bool allEnemiesDefeated = true;
            foreach (var controller in enemyControllers)
            {
                if (controller != null && controller.IsAlive)
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }

            if (allEnemiesDefeated)
            {
                Debug.Log("All enemies defeated!");
                await completeBattleInteractor.Execute(true);
                await stateMachine.ChangeState(InGameStateKey.Result);
                return;
            }

            // プレイヤー死亡チェック
            if (playerController != null && !playerController.Model.IsAlive)
            {
                Debug.Log("Player defeated!");
                await completeBattleInteractor.Execute(false);
                await stateMachine.ChangeState(InGameStateKey.Result);
            }
        }
    }
}

