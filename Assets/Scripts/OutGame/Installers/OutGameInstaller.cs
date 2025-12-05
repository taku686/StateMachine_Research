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
            // ===== 基盤層 =====
            // StateMachine（他のクラスに依存しない基盤）
            Container.Bind<StateMachine.StateMachine<OutGameStateKey>>()
                .AsSingle();

            // ===== Infrastructure層（Repository実装） =====
            // データ永続化の実装
            Container.Bind<IAudioSettingsRepository>()
                .To<AudioSettingsRepositoryImpl>()
                .AsSingle();

            Container.Bind<IUserProfileRepository>()
                .To<UserProfileRepositoryImpl>()
                .AsSingle();

            // ===== Presentation層（Presenter） =====
            // OutputPortの実装（Interactorから呼び出される）
            // VolumePresenter: インターフェースと実装クラスの両方でバインド（SettingsStateで直接注入されるため）
            Container.BindInterfacesAndSelfTo<VolumePresenter>()
                .AsSingle();

            // NavigationPresenter: インターフェースと実装クラスの両方でバインド（一貫性のため）
            Container.BindInterfacesAndSelfTo<NavigationPresenter>()
                .AsSingle();

            // ===== Application層（UseCase - Interactor） =====
            // InputPortの実装（Controllerから呼び出される）
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

            // ===== Presentation層（Controller） =====
            // Viewからの入力を受け取り、UseCaseに渡す
            Container.Bind<TitleController>().AsSingle();
            Container.Bind<HomeController>().AsSingle();
            Container.Bind<SettingsController>().AsSingle();

            // ===== State層 =====
            // 画面遷移のライフサイクル管理
            Container.Bind<TitleState>().AsSingle();
            Container.Bind<HomeState>().AsSingle();
            Container.Bind<SettingsState>().AsSingle();

            // ===== 管理層 =====
            // OutGameManager（すべての依存関係が解決された後にバインド）
            Container.Bind<OutGameManager>()
                .FromComponentInNewPrefab(_outGameManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}

