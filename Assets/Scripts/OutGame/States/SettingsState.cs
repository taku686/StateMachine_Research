using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Presentation.Controllers;
using OutGame.Presentation.Presenters;
using OutGame.Domain.Repositories;
using UnityEngine.AddressableAssets;
using Zenject;

namespace OutGame.States
{
    /// <summary>
    /// 設定画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// </summary>
    public class SettingsState : BaseState
    {
        private readonly SettingsController controller;
        private readonly VolumePresenter volumePresenter;
        private readonly IAudioSettingsRepository audioSettingsRepository;
        private SettingsView view;

        [Inject]
        public SettingsState(
            SettingsController controller,
            VolumePresenter volumePresenter,
            IAudioSettingsRepository audioSettingsRepository)
        {
            this.controller = controller;
            this.volumePresenter = volumePresenter;
            this.audioSettingsRepository = audioSettingsRepository;
        }

        public override async UniTask OnEnter()
        {
            // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync("SettingsView");
            view = viewObject.GetComponent<SettingsView>();

            // PresenterにViewを設定
            volumePresenter.SetView(view);

            // Controllerを初期化（ViewとUseCaseを接続）
            controller.Initialize(view);

            // 初期値を設定（Repositoryから読み込み）
            var settings = audioSettingsRepository.Load();
            view.SetBgmVolume(settings.BgmVolume);
            view.SetSeVolume(settings.SeVolume);

            // View を表示
            await view.Show();
        }

        public override async UniTask OnExit()
        {
            controller?.Dispose();

            if (view != null)
            {
                await view.Hide();
                view.Dispose();
                view = null;
            }
        }
    }
}