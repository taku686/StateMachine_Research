using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Common.Application.Bootstrap.Steps
{
    /// <summary>
    /// 最初のゲームシーン（OutGame）を Additive ロードするステップ
    /// </summary>
    public class LoadFirstGameSceneStep : IBootstrapStep
    {
        private const string FIRST_SCENE_NAME = "OutGame";

        private readonly ISceneTransitionOrchestrator orchestrator;

        [Inject]
        public LoadFirstGameSceneStep(ISceneTransitionOrchestrator orchestrator)
        {
            this.orchestrator = orchestrator;
        }

        public async UniTask ExecuteAsync()
        {
            Debug.Log($"[Bootstrap] Loading {FIRST_SCENE_NAME} scene...");
            
            var operation = SceneManager.LoadSceneAsync(FIRST_SCENE_NAME, LoadSceneMode.Additive);
            await operation;

            // TransitionOrchestratorに初期シーンを設定
            orchestrator.SetInitialScene(FIRST_SCENE_NAME);

            // アクティブシーンに設定
            var firstScene = SceneManager.GetSceneByName(FIRST_SCENE_NAME);
            if (firstScene.IsValid())
            {
                SceneManager.SetActiveScene(firstScene);
                Debug.Log($"[Bootstrap] {FIRST_SCENE_NAME} scene loaded and set as active");
            }
            else
            {
                Debug.LogError($"[Bootstrap] Failed to get {FIRST_SCENE_NAME} scene");
            }
        }
    }
}
