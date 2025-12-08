using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace InGame.Domain.Repositories
{
    using InGame.Domain.Models;

    /// <summary>
    /// 敵リポジトリのインターフェース
    /// </summary>
    public interface IEnemyRepository
    {
        /// <summary>
        /// ステージの全Wave構成を生成
        /// </summary>
        UniTask<List<Wave>> GenerateWaves(string stageId, int waveCount);

        /// <summary>
        /// 特定のWaveの敵を生成
        /// </summary>
        UniTask<Wave> GenerateWave(string stageId, int waveNumber);
    }
}

