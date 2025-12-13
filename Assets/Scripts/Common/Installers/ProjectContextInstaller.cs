using Common.Application;
using Common.Application.Bootstrap.Steps;
using Common.Domain.Services;
using Common.Infrastructure.Scene;
using Common.Infrastructure.Services;
using Common.Infrastructure.Transition;
using OutGame.Infrastructure.Views;
using OutGame.Domain.Repositories;
using OutGame.Infrastructure.Repositories;
using UnityEngine;
using Zenject;

namespace Common.Installers
{
    /// <summary>
    /// ProjectContextのInstaller
    /// ゲーム全体で共有されるサービスをバインド
    /// </summary>
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("[ProjectContextInstaller] Installing bindings...");

            // ===== 共有Repository =====
            // UserProfileはOutGameとInGameで共有されるため、ProjectContextでバインド
            Container.Bind<IUserProfileRepository>()
                .To<UserProfileRepositoryImpl>()
                .AsSingle();

            // ===== LoadingView =====
            // Transitionシーンから取得
            Container.Bind<LoadingView>()
                .FromMethod(ctx =>
                {
                    var loadingView = FindFirstObjectByType<LoadingView>();
                    if (loadingView == null)
                    {
                        Debug.LogError("[ProjectContextInstaller] LoadingView not found in hierarchy! Make sure Transition scene is loaded.");
                    }
                    return loadingView;
                })
                .AsSingle()
                .Lazy();

            // ===== サービス層 =====
            Container.Bind<ITransitionAnimator>()
                .To<TransitionAnimatorService>()
                .AsSingle()
                .Lazy();

            Container.Bind<ISceneLoadService>()
                .To<SceneLoadService>()
                .AsSingle();

            Container.Bind<ISceneUnloadService>()
                .To<SceneUnloadService>()
                .AsSingle();

            // ===== オーケストレーター =====
            // SceneTransitionOrchestratorを両方のインターフェースにバインド
            Container.Bind(new[] { typeof(ISceneTransitionOrchestrator), typeof(IInitialSceneSetter) })
                .To<SceneTransitionOrchestrator>()
                .AsSingle();

            // ===== ファサード =====
            Container.Bind<ISceneLoaderService>()
                .To<SceneLoaderService>()
                .AsSingle()
                .Lazy(); // Transitionシーンロード後に初期化されるように変更

            Debug.Log("[ProjectContextInstaller] Bindings completed");
        }
    }
}
