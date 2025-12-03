using UnityEngine;
using Zenject;
using OutGame.Domain.Repositories;
using OutGame.Infrastructure.Repositories;
using OutGame.Application.Ports.Input;
using OutGame.Application.Ports.Output;
using OutGame.Application.UseCases;
using OutGame.Presentation.Controllers;
using OutGame.Presentation.Presenters;
using OutGame.States;

namespace OutGame.Installers
{
    /// <summary>
    /// アウトゲーム用のZenject Installer
    /// クリーンアーキテクチャに基づいた依存性注入の設定
    /// </summary>
    public class OutGameInstaller : MonoInstaller
    {
        [SerializeField] private OutGameManager _outGameManagerPrefab;

        public override void InstallBindings()
        {
            // OutGameManager をバインド
            Container.Bind<OutGameManager>()
                .FromComponentInNewPrefab(_outGameManagerPrefab)
                .AsSingle()
                .NonLazy();

            // StateMachine をバインド
            Container.Bind<StateMachine.StateMachine<OutGameStateKey>>()
                .AsSingle();

            // ===== Infrastructure層（Repository実装） =====
            Container.Bind<IAudioSettingsRepository>()
                .To<AudioSettingsRepositoryImpl>()
                .AsSingle();

            Container.Bind<IUserProfileRepository>()
                .To<UserProfileRepositoryImpl>()
                .AsSingle();

            // ===== Application層（UseCase - Interactor） =====
            // 音量更新
            Container.Bind<IUpdateBgmVolumeInputPort>()
                .To<UpdateBgmVolumeInteractor>()
                .AsSingle();

            Container.Bind<IUpdateSeVolumeInputPort>()
                .To<UpdateSeVolumeInteractor>()
                .AsSingle();

            // 画面遷移
            Container.Bind<INavigateToHomeInputPort>()
                .To<NavigateToHomeInteractor>()
                .AsSingle();

            Container.Bind<IOpenSettingsInputPort>()
                .To<OpenSettingsInteractor>()
                .AsSingle();

            Container.Bind<INavigateBackInputPort>()
                .To<NavigateBackInteractor>()
                .AsSingle();

            // ===== Presentation層（Presenter） =====
            Container.Bind<IVolumeUpdateOutputPort>()
                .To<VolumePresenter>()
                .AsSingle();

            Container.Bind<INavigationOutputPort>()
                .To<NavigationPresenter>()
                .AsSingle();

            // VolumePresenterを直接注入できるようにもバインド
            Container.Bind<VolumePresenter>()
                .FromResolve();

            // ===== Presentation層（Controller） =====
            Container.Bind<TitleController>().AsSingle();
            Container.Bind<HomeController>().AsSingle();
            Container.Bind<SettingsController>().AsSingle();

            // ===== State層 =====
            Container.Bind<TitleState>().AsSingle();
            Container.Bind<HomeState>().AsSingle();
            Container.Bind<SettingsState>().AsSingle();
        }
    }
}

