using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Common.Domain.Services
{
    /// <summary>
    /// シーン読み込みを担当
    /// </summary>
    public interface ISceneLoadService
    {
        /// <summary>
        /// シーンをAdditive読み込み
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="onProgress">進捗コールバック (0.0 - 1.0)</param>
        /// <returns>読み込まれたシーン</returns>
        UniTask<Scene> LoadSceneAdditiveAsync(string sceneName, Action<float> onProgress = null);
    }
}
