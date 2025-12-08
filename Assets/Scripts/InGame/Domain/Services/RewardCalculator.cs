namespace InGame.Domain.Services
{
    using InGame.Domain.Models;

    /// <summary>
    /// 報酬計算サービス実装
    /// </summary>
    public class RewardCalculator : IRewardCalculator
    {
        /// <summary>
        /// ステージクリア報酬を計算
        /// </summary>
        public BattleReward CalculateStageClearBonus(string stageId, int playerLevel)
        {
            // ステージIDから基礎報酬を決定（簡易実装）
            int stageLevel = ExtractStageLevelFromId(stageId);

            int bonusGold = stageLevel * 100;
            int bonusExp = stageLevel * 50;

            // プレイヤーレベルによるボーナス
            bonusGold += playerLevel * 10;
            bonusExp += playerLevel * 5;

            return new BattleReward(bonusGold, bonusExp);
        }

        /// <summary>
        /// Wave完了ボーナスを計算
        /// </summary>
        public BattleReward CalculateWaveClearBonus(int waveNumber)
        {
            int bonusGold = waveNumber * 20;
            int bonusExp = waveNumber * 10;

            return new BattleReward(bonusGold, bonusExp);
        }

        /// <summary>
        /// ステージIDからレベルを抽出（簡易実装）
        /// 例: "Stage_1" → 1
        /// </summary>
        private int ExtractStageLevelFromId(string stageId)
        {
            if (string.IsNullOrEmpty(stageId))
            {
                return 1;
            }

            // アンダースコア後の数字を抽出
            var parts = stageId.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[1], out int level))
            {
                return level;
            }

            return 1;
        }
    }
}

