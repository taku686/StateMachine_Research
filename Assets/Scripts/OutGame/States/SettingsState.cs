using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Infrastructure.Factories;
using OutGame.Presentation.Controllers;
using OutGame.Presentation.Presenters;
using OutGame.Domain.Repositories;
using OutGame.Domain.Models;
using Zenject;
using UnityEngine;

namespace OutGame.States
{
    /// <summary>
    /// 設定画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// アセットのプリロード/クリーンアップは基底クラスで自動処理
    /// データロードで設定情報を事前取得
    /// </summary>
    public class SettingsState : BaseState
    {
        private readonly SettingsController controller;
        private readonly VolumePresenter volumePresenter;
        private readonly IAudioSettingsRepository audioSettingsRepository;
        private readonly IViewFactory<SettingsView> viewFactory;
        private SettingsView view;
        private Domain.Models.AudioSettings audioSettings; // ロードしたデータを保持

        [Inject]
        public SettingsState(
            SettingsController controller,
            VolumePresenter volumePresenter,
            IAudioSettingsRepository audioSettingsRepository,
            IViewFactory<SettingsView> viewFactory,
            AddressableViewFactory<SettingsView> addressableFactory)
        {
            this.controller = controller;
            this.volumePresenter = volumePresenter;
            this.audioSettingsRepository = audioSettingsRepository;
            this.viewFactory = viewFactory;

            // Viewアセットをプリロード対象として登録
            RegisterPreloadableAsset(addressableFactory);
        }

        /// <summary>
        /// データのロード
        /// オーディオ設定を事前に取得
        /// </summary>
        protected override async UniTask OnLoadDataAsync()
        {
            Debug.Log("[SettingsState] Loading audio settings data...");

            // Repositoryから設定をロード
            audioSettings = audioSettingsRepository.Load();

            Debug.Log($"[SettingsState] Audio settings loaded: BGM={audioSettings.BgmVolume}, SE={audioSettings.SeVolume}");

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// ステートに入る
        /// アセットとデータは既にロード済み
        /// </summary>
        public override async UniTask OnEnter()
        {
            Debug.Log("[SettingsState] Entering state...");

            // アセットは既にプリロード済みなので即座に作成
            view = await viewFactory.CreateAsync();

            // PresenterにViewを設定
            volumePresenter.SetView(view);

            // Controllerを初期化
            controller.Initialize(view);

            // ロード済みのデータをViewに設定
            view.SetBgmVolume(audioSettings.BgmVolume);
            view.SetSeVolume(audioSettings.SeVolume);

            // Viewを表示
            await view.Show();

            Debug.Log("[SettingsState] State entered!");
        }

        /// <summary>
        /// ステートから出る
        /// </summary>
        public override async UniTask OnExit()
        {
            Debug.Log("[SettingsState] Exiting state...");

            controller?.Dispose();

            if (view != null)
            {
                await view.Hide();
                view.Dispose();
                view = null;
            }

            Debug.Log("[SettingsState] State exited!");
        }

        // アセットのプリロード/クリーンアップは基底クラスが自動処理
    }
}