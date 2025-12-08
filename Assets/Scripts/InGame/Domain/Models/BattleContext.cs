using System.Collections.Generic;

namespace InGame.Domain.Models
{
    /// <summary>
    /// バトル全体の状態を管理
    /// </summary>
    public class BattleContext
    {
        public string StageId { get; private set; }
        public Player Player { get; private set; }
        public List<Wave> Waves { get; private set; }
        public int CurrentWaveIndex { get; private set; }
        public BattleReward TotalReward { get; private set; }
        public BattleStatus Status { get; private set; }

        public Wave CurrentWave => CurrentWaveIndex < Waves.Count ? Waves[CurrentWaveIndex] : null;
        public bool HasNextWave => CurrentWaveIndex < Waves.Count - 1;
        public bool IsAllWavesCleared => Waves.TrueForAll(w => w.IsCleared);

        public BattleContext(string stageId, Player player, List<Wave> waves)
        {
            StageId = stageId;
            Player = player;
            Waves = waves;
            CurrentWaveIndex = 0;
            TotalReward = new BattleReward(0, 0);
            Status = BattleStatus.InProgress;
        }

        /// <summary>
        /// 次のWaveに進む
        /// </summary>
        public bool AdvanceToNextWave()
        {
            if (!CurrentWave.IsCleared)
            {
                return false;
            }

            if (HasNextWave)
            {
                CurrentWaveIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 報酬を加算
        /// </summary>
        public void AddReward(BattleReward reward)
        {
            TotalReward = TotalReward.Add(reward);
        }

        /// <summary>
        /// バトルをクリア状態にする
        /// </summary>
        public void MarkAsCleared()
        {
            Status = BattleStatus.Cleared;
        }

        /// <summary>
        /// バトルを失敗状態にする
        /// </summary>
        public void MarkAsFailed()
        {
            Status = BattleStatus.Failed;
        }
    }

    /// <summary>
    /// バトルの状態
    /// </summary>
    public enum BattleStatus
    {
        InProgress,  // 進行中
        Cleared,     // クリア
        Failed       // 失敗
    }
}

