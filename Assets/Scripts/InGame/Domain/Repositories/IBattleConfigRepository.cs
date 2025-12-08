using Cysharp.Threading.Tasks;

namespace InGame.Domain.Repositories
{
    /// <summary>
    /// バトル設定リポジトリのインターフェース
    /// </summary>
    public interface IBattleConfigRepository
    {
        /// <summary>
        /// ステージのWave数を取得
        /// </summary>
        UniTask<int> GetWaveCount(string stageId);

        /// <summary>
        /// ステージの難易度係数を取得
        /// </summary>
        UniTask<float> GetDifficultyMultiplier(string stageId);
    }
}

