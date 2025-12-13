using System;
using Cysharp.Threading.Tasks;

namespace Common.Domain.Services
{
    /// <summary>
    /// シーン遷移全体を統括（オーケストレーション）
    /// 各サービスを組み合わせて、遷移フローを実行
    /// </summary>
    public interface ISceneTransitionOrchestrator
    {
        /// <summary>
        /// シーン遷移を実行
        /// </summary>
        /// <param name="targetSceneName">遷移先のシーン名</param>
        /// <param name="onProgress">進捗コールバック (0.0 - 1.0)</param>
        UniTask TransitionToSceneAsync(string targetSceneName, Action<float> onProgress = null);
    }
}
