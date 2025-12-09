using UnityEngine;
using Zenject;

namespace InGame.Presentation.Controllers
{
    using Cysharp.Threading.Tasks;
    using InGame.Application.Ports.Input;
    using InGame.Domain.Models;
    using InGame.Presentation.Views;

    /// <summary>
    /// プレイヤーのViewController
    /// </summary>
    public class PlayerViewController
    {
        private readonly Player playerModel;
        private readonly PlayerView playerView;
        private readonly HealthBarView healthBarView;
        private readonly IAttackEnemyInputPort attackInteractor;

        private float attackTimer;
        private Enemy currentTarget;

        [Inject]
        public PlayerViewController(
            Player playerModel,
            PlayerView playerView,
            HealthBarView healthBarView,
            IAttackEnemyInputPort attackInteractor)
        {
            this.playerModel = playerModel;
            this.playerView = playerView;
            this.healthBarView = healthBarView;
            this.attackInteractor = attackInteractor;
        }

        public Player Model => playerModel;
        public PlayerView View => playerView;

        public void Initialize()
        {
            // 初期HP表示
            UpdateHealthBar();
        }

        public void Update(float deltaTime)
        {
            if (!playerModel.IsAlive)
            {
                return;
            }

            // 移動処理
            MovePlayer(deltaTime);

            // 攻撃処理
            ProcessAttack(deltaTime);

            // View更新
            UpdateView();
        }

        private void MovePlayer(float deltaTime)
        {
            var currentPos = playerView.transform.position;
            var newPos = currentPos + Vector3.right * playerModel.Stats.MoveSpeed * deltaTime;
            playerView.UpdatePosition(newPos);
            playerView.UpdateMoveAnimation(playerModel.Stats.MoveSpeed);
        }

        private void ProcessAttack(float deltaTime)
        {
            attackTimer += deltaTime;

            if (attackTimer >= playerModel.Stats.AttackSpeed)
            {
                if (currentTarget != null && currentTarget.IsAlive)
                {
                    attackInteractor.Execute(playerModel, currentTarget).Forget();
                    playerView.PlayAttackAnimation();
                    attackTimer = 0;
                }
            }
        }

        public void SetTarget(Enemy target)
        {
            currentTarget = target;
        }

        public void ClearTarget()
        {
            currentTarget = null;
        }

        private void UpdateView()
        {
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            if (healthBarView != null)
            {
                healthBarView.SetValue(playerModel.Stats.CurrentHp, playerModel.Stats.MaxHp);
            }
        }

        public void OnDeath()
        {
            playerView.PlayDeathAnimation();
        }

        /// <summary>
        /// Factory for creating PlayerViewController with Zenject
        /// </summary>
        public class Factory : PlaceholderFactory<Player, PlayerView, HealthBarView, PlayerViewController>
        {
        }
    }
}

