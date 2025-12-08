using UnityEngine;
using Zenject;

namespace InGame.Presentation.Controllers
{
    using InGame.Domain.Models;
    using InGame.Presentation.Views;

    /// <summary>
    /// 敵のViewController
    /// </summary>
    public class EnemyViewController
    {
        private readonly Enemy enemyModel;
        private readonly EnemyView enemyView;
        private readonly HealthBarView healthBarView;

        public Enemy Model => enemyModel;
        public EnemyView View => enemyView;
        public bool IsAlive => enemyModel.IsAlive;

        public EnemyViewController(
            Enemy enemyModel,
            EnemyView enemyView,
            HealthBarView healthBarView)
        {
            this.enemyModel = enemyModel;
            this.enemyView = enemyView;
            this.healthBarView = healthBarView;
        }

        public void Initialize()
        {
            // 初期HP表示
            UpdateHealthBar();
        }

        public void Update(float deltaTime)
        {
            // 現在は動かない敵のみ実装
            // TODO: 移動する敵の実装

            UpdateView();
        }

        private void UpdateView()
        {
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            if (healthBarView != null)
            {
                healthBarView.SetValue(enemyModel.Stats.CurrentHp, enemyModel.Stats.MaxHp);
            }
        }

        public void OnDeath()
        {
            // 死亡演出
            if (enemyView != null)
            {
                enemyView.SetColor(Color.gray);
            }
        }

        public void Destroy()
        {
            if (enemyView != null)
            {
                Object.Destroy(enemyView.gameObject);
            }
        }

        /// <summary>
        /// Factory for creating EnemyViewController with Zenject
        /// </summary>
        public class Factory : PlaceholderFactory<Enemy, EnemyView, HealthBarView, EnemyViewController>
        {
        }
    }
}

