using Zenject;

namespace InGame.Presentation.Controllers
{
    using InGame.Domain.Models;
    using InGame.Presentation.Views;

    /// <summary>
    /// バトルHUDのController
    /// </summary>
    public class BattleHUDController
    {
        private readonly BattleHUDView hudView;
        private BattleContext battleContext;

        [Inject]
        public BattleHUDController(BattleHUDView hudView)
        {
            this.hudView = hudView;
        }

        public void SetBattleContext(BattleContext context)
        {
            this.battleContext = context;
            UpdateWaveInfo();
            UpdateRewardInfo();
        }

        public void UpdatePlayerHp(int currentHp, int maxHp)
        {
            hudView.UpdatePlayerHp(currentHp, maxHp);
        }

        public void UpdateWaveInfo()
        {
            if (battleContext != null)
            {
                int currentWave = battleContext.CurrentWaveIndex + 1;
                int totalWaves = battleContext.Waves.Count;
                hudView.UpdateWaveInfo(currentWave, totalWaves);
            }
        }

        public void UpdateRewardInfo()
        {
            if (battleContext != null)
            {
                hudView.UpdateGold(battleContext.TotalReward.Gold);
                hudView.UpdateExp(battleContext.TotalReward.Experience);
            }
        }

        public void AddReward(BattleReward reward)
        {
            if (battleContext != null)
            {
                battleContext.AddReward(reward);
                UpdateRewardInfo();
            }
        }
    }
}

