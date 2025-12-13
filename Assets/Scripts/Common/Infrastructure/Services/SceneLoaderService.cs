using System;
using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Common.Infrastructure.Services
{
    /// <summary>
    /// シーン遷移のファサード
    /// オーケストレーターへの委譲のみ
    /// </summary>
    public class SceneLoaderService : ISceneLoaderService
    {
        private const string INGAME_SCENE = "InGame";
        private const string OUTGAME_SCENE = "OutGame";

        private readonly ISceneTransitionOrchestrator orchestrator;

        [Inject]
        public SceneLoaderService(ISceneTransitionOrchestrator orchestrator)
        {
            this.orchestrator = orchestrator;
        }

        public async UniTask TransitionToSceneAsync(
            string sceneName,
            Action<float> onProgress = null)
        {
            await orchestrator.TransitionToSceneAsync(sceneName, onProgress);
        }

        public async UniTask LoadInGameSceneAsync()
        {
            await TransitionToSceneAsync(INGAME_SCENE);
        }

        public async UniTask LoadOutGameSceneAsync()
        {
            await TransitionToSceneAsync(OUTGAME_SCENE);
        }
    }
}
