using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.Application.UseCases
{
    using InGame.Application.Ports.Input;
    using InGame.Application.Ports.Output;
    using InGame.Domain.Models;
    using InGame.Domain.Services;
    using InGame.Domain.Repositories;

    /// <summary>
    /// バトル完了ユースケース
    /// </summary>
    public class CompleteBattleInteractor : ICompleteBattleInputPort
    {
        private readonly IRewardCalculator rewardCalculator;
        private readonly IPlayerRepository playerRepository;
        private readonly IBattleOutputPort outputPort;
        private readonly IRewardOutputPort rewardOutputPort;

        private BattleContext battleContext;

        [Inject]
        public CompleteBattleInteractor(
            IRewardCalculator rewardCalculator,
            IPlayerRepository playerRepository,
            IBattleOutputPort outputPort,
            IRewardOutputPort rewardOutputPort)
        {
            this.rewardCalculator = rewardCalculator;
            this.playerRepository = playerRepository;
            this.outputPort = outputPort;
            this.rewardOutputPort = rewardOutputPort;
        }

        public void SetBattleContext(BattleContext context)
        {
            this.battleContext = context;
        }

        public async UniTask Execute(bool isCleared)
        {
            if (battleContext == null)
            {
                Debug.LogError("BattleContext is null!");
                return;
            }

            if (isCleared)
            {
                battleContext.MarkAsCleared();

                // ステージクリアボーナス計算
                var clearBonus = rewardCalculator.CalculateStageClearBonus(
                    battleContext.StageId,
                    battleContext.Player.Level
                );

                battleContext.AddReward(clearBonus);

                Debug.Log($"Battle Cleared! Total Reward: {battleContext.TotalReward}");
            }
            else
            {
                battleContext.MarkAsFailed();
                Debug.Log("Battle Failed!");
            }

            // 報酬通知
            if (isCleared)
            {
                await rewardOutputPort.OnRewardAcquired(battleContext.TotalReward);
            }

            // バトル完了通知
            await outputPort.OnBattleCompleted(battleContext);

            // プレイヤーデータ保存
            if (isCleared)
            {
                battleContext.Player.AddExperience(battleContext.TotalReward.Experience);
                await playerRepository.SavePlayer(battleContext.Player);
            }
        }
    }
}

