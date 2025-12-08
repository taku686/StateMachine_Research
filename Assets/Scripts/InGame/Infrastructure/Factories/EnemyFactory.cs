using Zenject;

namespace InGame.Infrastructure.Factories
{
    using InGame.Domain.Models;
    using InGame.Infrastructure.Data;

    /// <summary>
    /// 敵ファクトリー（EnemyData → Enemy変換）
    /// Zenject Factoryを使用
    /// </summary>
    public class EnemyFactory : IFactory<EnemyData, Enemy>
    {
        private readonly Enemy.Factory enemyDomainFactory;
        private int enemyCounter = 0;

        [Inject]
        public EnemyFactory(Enemy.Factory enemyDomainFactory)
        {
            this.enemyDomainFactory = enemyDomainFactory;
        }

        public Enemy Create(EnemyData data)
        {
            // ステータス作成（値オブジェクト）
            var stats = new CombatStats(
                data.maxHp,
                data.maxHp,
                data.attackPower,
                data.defense,
                data.attackSpeed,
                data.moveSpeed,
                0f  // 敵のクリティカル率は0
            );

            // 報酬作成（値オブジェクト）
            var reward = new BattleReward(
                data.goldReward,
                data.expReward
            );

            // Zenject FactoryでEnemyエンティティ生成
            var enemy = enemyDomainFactory.Create(
                $"{data.enemyId}_{enemyCounter++}",
                data.enemyType,
                stats,
                reward
            );

            return enemy;
        }
    }
}

