using System;
using Common.Constants;
using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Common.Application
{
    /// <summary>
    /// シーン遷移の全体フローを統括
    /// 各サービスを組み合わせて、遷移手順を実行
    /// 
    /// 【責務】
    /// - 遷移フローの定義（どの順番で何を呼ぶか）
    /// - 各サービスの連携
    /// - 状態管理（現在のシーン、遷移中フラグ）
    /// </summary>
    public class SceneTransitionOrchestrator : ISceneTransitionOrchestrator, IInitialSceneSetter
    {
        private readonly ITransitionAnimator animator;
        private readonly ISceneLoadService sceneLoader;
        private readonly ISceneUnloadService sceneUnloader;

        private Scene _currentGameScene;
        private bool _isTransitioning;

        [Inject]
        public SceneTransitionOrchestrator(
            ITransitionAnimator animator,
            ISceneLoadService sceneLoader,
            ISceneUnloadService sceneUnloader)
        {
            this.animator = animator;
            this.sceneLoader = sceneLoader;
            this.sceneUnloader = sceneUnloader;
        }

        public async UniTask TransitionToSceneAsync(
            string targetSceneName,
            Action<float> onProgress = null)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[SceneTransitionOrchestrator] Already transitioning!");
                return;
            }

            _isTransitioning = true;

            try
            {
                Debug.Log($"[SceneTransitionOrchestrator] === Starting transition to {targetSceneName} ===");

                // Phase 1: フェードイン（画面を暗くする）
                Debug.Log("[Phase 1] Fade In");
                await animator.FadeInAsync(AppConstants.Animation.DefaultFadeDuration);

                // Phase 2: ローディング表示
                Debug.Log("[Phase 2] Show Loading");
                animator.ShowLoading();

                // Phase 3: 次シーンをAdditive読み込み
                Debug.Log($"[Phase 3] Loading scene: {targetSceneName}");
                var newScene = await sceneLoader.LoadSceneAdditiveAsync(
                    targetSceneName,
                    progress =>
                    {
                        animator.SetProgress(progress);
                        onProgress?.Invoke(progress);
                    });

                // Phase 4: 前シーンをアンロード
                if (_currentGameScene.IsValid() && _currentGameScene.isLoaded)
                {
                    Debug.Log($"[Phase 4] Unloading previous scene: {_currentGameScene.name}");
                    await sceneUnloader.UnloadSceneAsync(_currentGameScene);
                    await sceneUnloader.UnloadUnusedAssetsAsync();
                }

                // Phase 5: 新シーンをアクティブに設定
                if (newScene.IsValid())
                {
                    Debug.Log($"[Phase 5] Setting active scene: {targetSceneName}");
                    SceneManager.SetActiveScene(newScene);
                    _currentGameScene = newScene;
                }
                else
                {
                    Debug.LogError($"[SceneTransitionOrchestrator] Failed to load scene: {targetSceneName}");
                }

                // Phase 6: ローディング非表示
                Debug.Log("[Phase 6] Hide Loading");
                animator.HideLoading();

                // Phase 7: フェードアウト（画面を明るくする）
                Debug.Log("[Phase 7] Fade Out");
                await animator.FadeOutAsync(AppConstants.Animation.DefaultFadeDuration);

                Debug.Log($"[SceneTransitionOrchestrator] === Transition complete: {targetSceneName} ===");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitionOrchestrator] Transition failed: {ex.Message}\n{ex.StackTrace}");

                // エラー時はローディングを非表示にする
                animator.HideLoading();
                await animator.FadeOutAsync(AppConstants.Animation.DefaultFadeDuration);

                throw;
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        public void SetInitialScene(string sceneName)
        {
            _currentGameScene = SceneManager.GetSceneByName(sceneName);
            Debug.Log($"[SceneTransitionOrchestrator] Initial scene set: {sceneName}, IsValid: {_currentGameScene.IsValid()}");
        }
    }
}
