using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace InGame.Infrastructure.Repositories
{
    using InGame.Domain.Models;
    using InGame.Domain.Repositories;
    using InGame.Infrastructure.Data;

    /// <summary>
    /// 敵リポジトリ実装
    /// ScriptableObjectからマスターデータを読み込み
    /// </summary>
    public class EnemyRepositoryImpl : IEnemyRepository
    {
        private readonly IFactory<EnemyData, Enemy> enemyFactory;
        private readonly Dictionary<string, BattleConfig> configCache;

        [Inject]
        public EnemyRepositoryImpl(IFactory<EnemyData, Enemy> enemyFactory)
        {
            this.enemyFactory = enemyFactory;
            this.configCache = new Dictionary<string, BattleConfig>();
        }

        public async UniTask<List<Wave>> GenerateWaves(string stageId, int waveCount)
        {
            var waves = new List<Wave>();

            for (int i = 0; i < waveCount; i++)
            {
                var wave = await GenerateWave(stageId, i + 1);
                waves.Add(wave);
            }

            return waves;
        }

        public async UniTask<Wave> GenerateWave(string stageId, int waveNumber)
        {
            var wave = new Wave(waveNumber);

            // TODO: BattleConfigから敵パターンを取得
            // 現在は仮実装：Wave番号に応じて敵を生成
            var enemyDataList = GetEnemyDataForWave(waveNumber);

            foreach (var enemyData in enemyDataList)
            {
                var enemy = enemyFactory.Create(enemyData);
                wave.AddEnemy(enemy);
            }

            await UniTask.CompletedTask;
            return wave;
        }

        /// <summary>
        /// Wave番号に応じた敵データを取得（仮実装）
        /// TODO: ScriptableObjectから読み込むように変更
        /// </summary>
        private List<EnemyData> GetEnemyDataForWave(int waveNumber)
        {
            var list = new List<EnemyData>();

            // Wave1: 雑魚3体
            if (waveNumber == 1)
            {
                list.Add(CreateDefaultEnemyData("Slime", EnemyType.Normal, 50, 8, 3));
                list.Add(CreateDefaultEnemyData("Slime", EnemyType.Normal, 50, 8, 3));
                list.Add(CreateDefaultEnemyData("Slime", EnemyType.Normal, 50, 8, 3));
            }
            // Wave2: エリート2体
            else if (waveNumber == 2)
            {
                list.Add(CreateDefaultEnemyData("Goblin", EnemyType.Elite, 100, 15, 8));
                list.Add(CreateDefaultEnemyData("Goblin", EnemyType.Elite, 100, 15, 8));
            }
            // Wave3: ボス1体
            else if (waveNumber == 3)
            {
                list.Add(CreateDefaultEnemyData("Dragon", EnemyType.Boss, 300, 25, 15));
            }

            return list;
        }

        /// <summary>
        /// デフォルトの敵データを作成（仮実装）
        /// </summary>
        private EnemyData CreateDefaultEnemyData(string name, EnemyType type, int hp, int attack, int defense)
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            data.enemyId = name;
            data.enemyName = name;
            data.enemyType = type;
            data.maxHp = hp;
            data.attackPower = attack;
            data.defense = defense;
            data.attackSpeed = 2f;
            data.moveSpeed = 0f;

            // 報酬はタイプに応じて設定
            switch (type)
            {
                case EnemyType.Normal:
                    data.goldReward = 10;
                    data.expReward = 5;
                    break;
                case EnemyType.Elite:
                    data.goldReward = 30;
                    data.expReward = 15;
                    break;
                case EnemyType.Boss:
                    data.goldReward = 100;
                    data.expReward = 50;
                    break;
            }

            return data;
        }
    }
}

