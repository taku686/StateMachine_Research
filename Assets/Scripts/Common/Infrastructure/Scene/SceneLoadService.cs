using System;
using Common.Constants;
using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Infrastructure.Scene
{
    /// <summary>
    /// シーン読み込みのみを担当
    /// </summary>
    public class SceneLoadService : ISceneLoadService
    {
        public async UniTask<UnityEngine.SceneManagement.Scene> LoadSceneAdditiveAsync(
            string sceneName,
            Action<float> onProgress = null)
        {
            Debug.Log($"[SceneLoadService] Loading scene: {sceneName}");

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (operation == null)
            {
                Debug.LogError($"[SceneLoadService] Failed to start loading scene: {sceneName}");
                return default;
            }

            // 90%まで自動で進める、最後の10%は手動制御
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                // 進捗を0.0～0.9に正規化
                float progress = Mathf.Clamp01(operation.progress / AppConstants.Scene.LoadProgressThreshold);
                onProgress?.Invoke(progress);

                // 90%に達したら手動で最後の10%を進める
                if (operation.progress >= AppConstants.Scene.LoadProgressThreshold)
                {
                    // 最後の10%を手動で進める（演出用）
                    for (float t = AppConstants.Scene.LoadProgressThreshold; t <= AppConstants.Progress.Max; t += AppConstants.Scene.LoadProgressIncrement)
                    {
                        onProgress?.Invoke(t);
                        await UniTask.Delay(AppConstants.Scene.LoadProgressUpdateInterval); // 約60fps
                    }

                    operation.allowSceneActivation = true;
                    break;
                }

                await UniTask.Yield();
            }

            // 完全に完了するまで待機
            await UniTask.WaitUntil(() => operation.isDone);

            var loadedScene = SceneManager.GetSceneByName(sceneName);
            Debug.Log($"[SceneLoadService] Scene loaded: {sceneName}, IsValid: {loadedScene.IsValid()}");

            return loadedScene;
        }
    }
}
