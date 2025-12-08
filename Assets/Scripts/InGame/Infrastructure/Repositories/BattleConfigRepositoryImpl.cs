using Cysharp.Threading.Tasks;

namespace InGame.Infrastructure.Repositories
{
    using InGame.Domain.Repositories;

    /// <summary>
    /// バトル設定リポジトリ実装
    /// TODO: ScriptableObjectから読み込むように変更
    /// </summary>
    public class BattleConfigRepositoryImpl : IBattleConfigRepository
    {
        public async UniTask<int> GetWaveCount(string stageId)
        {
            // 現在は固定値
            await UniTask.CompletedTask;
            return 3;  // 全ステージ3Wave
        }

        public async UniTask<float> GetDifficultyMultiplier(string stageId)
        {
            // 現在は固定値
            await UniTask.CompletedTask;
            return 1.0f;
        }
    }
}

