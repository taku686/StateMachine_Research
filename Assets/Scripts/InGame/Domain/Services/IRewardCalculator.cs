namespace InGame.Domain.Services
{
    using InGame.Domain.Models;

    /// <summary>
    /// 報酬計算サービスのインターフェース
    /// </summary>
    public interface IRewardCalculator
    {
        /// <summary>
        /// ステージクリア報酬を計算
        /// </summary>
        BattleReward CalculateStageClearBonus(string stageId, int playerLevel);

        /// <summary>
        /// Wave完了ボーナスを計算
        /// </summary>
        BattleReward CalculateWaveClearBonus(int waveNumber);
    }
}

