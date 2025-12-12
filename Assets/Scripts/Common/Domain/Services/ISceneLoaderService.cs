using System;
using Cysharp.Threading.Tasks;

namespace Common.Domain.Services
{
    /// <summary>
    /// シーン遷移のファサード
    /// 外部向けの簡潔なインターフェースを提供
    /// </summary>
    public interface ISceneLoaderService
    {
        /// <summary>
        /// 指定したシーンへ遷移
        /// </summary>
        UniTask TransitionToSceneAsync(string sceneName, Action<float> onProgress = null);

        /// <summary>
        /// InGameシーンへ遷移
        /// </summary>
        UniTask LoadInGameSceneAsync();

        /// <summary>
        /// OutGameシーンへ遷移
        /// </summary>
        UniTask LoadOutGameSceneAsync();
    }
}
