using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.Application.UseCases
{
    using InGame.Application.Ports.Input;
    using InGame.Application.Ports.Output;
    using InGame.Domain.Models;
    using InGame.Domain.Repositories;

    /// <summary>
    /// バトル開始ユースケース
    /// </summary>
    public class StartBattleInteractor : IStartBattleInputPort
    {
        private readonly IPlayerRepository playerRepository;
        private readonly IEnemyRepository enemyRepository;
        private readonly IBattleConfigRepository battleConfigRepository;
        private readonly IBattleOutputPort outputPort;

        private BattleContext currentBattleContext;

        [Inject]
        public StartBattleInteractor(
            IPlayerRepository playerRepository,
            IEnemyRepository enemyRepository,
            IBattleConfigRepository battleConfigRepository,
            IBattleOutputPort outputPort)
        {
            this.playerRepository = playerRepository;
            this.enemyRepository = enemyRepository;
            this.battleConfigRepository = battleConfigRepository;
            this.outputPort = outputPort;
        }

        public async UniTask Execute(string stageId)
        {
            Debug.Log($"Starting battle for stage: {stageId}");

            // プレイヤーデータ取得
            var player = await playerRepository.GetPlayer();
            Debug.Log($"Player loaded: Level {player.Level}, HP {player.Stats.MaxHp}");

            // ステージ設定取得
            var waveCount = await battleConfigRepository.GetWaveCount(stageId);

            // Wave構成を生成
            var waves = await enemyRepository.GenerateWaves(stageId, waveCount);
            Debug.Log($"Generated {waves.Count} waves");

            // バトルコンテキスト作成
            currentBattleContext = new BattleContext(stageId, player, waves);

            // Presenterに通知
            await outputPort.OnBattleStarted(currentBattleContext);

            // 最初のWave開始
            if (currentBattleContext.CurrentWave != null)
            {
                await outputPort.OnWaveStarted(currentBattleContext.CurrentWave);
            }
        }

        public BattleContext GetCurrentBattleContext()
        {
            return currentBattleContext;
        }
    }
}

