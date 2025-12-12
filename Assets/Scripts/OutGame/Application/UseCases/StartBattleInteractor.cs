using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using OutGame.Application.Ports.Input;
using OutGame.Domain.Repositories;
using UnityEngine;
using Zenject;

namespace OutGame.Application.UseCases
{
    /// <summary>
    /// バトル開始処理
    /// </summary>
    public class StartBattleInteractor : IStartBattleInputPort
    {
        private readonly ISceneLoaderService sceneLoader;
        private readonly IUserProfileRepository userProfileRepository;

        [Inject]
        public StartBattleInteractor(
            ISceneLoaderService sceneLoader,
            IUserProfileRepository userProfileRepository)
        {
            this.sceneLoader = sceneLoader;
            this.userProfileRepository = userProfileRepository;
        }

        public async UniTask Execute(int stageId)
        {
            Debug.Log($"[StartBattleInteractor] Starting battle for stage {stageId}");

            try
            {
                // 1. ユーザープロフィールにステージIDを保存
                var profile = userProfileRepository.Load();
                profile.SetCurrentStageId(stageId);
                userProfileRepository.Save(profile);

                Debug.Log($"[StartBattleInteractor] CurrentStageId saved: {stageId}");

                // 2. InGameシーンへ遷移
                await sceneLoader.LoadInGameSceneAsync();

                Debug.Log("[StartBattleInteractor] Transition to InGame complete");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[StartBattleInteractor] Failed to start battle: {ex.Message}");
                throw;
            }
        }
    }
}
