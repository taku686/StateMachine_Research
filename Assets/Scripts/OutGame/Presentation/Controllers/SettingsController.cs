using OutGame.Application.Ports.Input;
using OutGame.Infrastructure.Views;
using R3;
using UnityEngine;
using Zenject;

namespace OutGame.Presentation.Controllers
{
    /// <summary>
    /// 設定画面のController
    /// Viewからの入力を受け取り、UseCaseに渡す
    /// </summary>
    public class SettingsController
    {
        private readonly IUpdateBgmVolumeInputPort updateBgmVolumeUseCase;
        private readonly IUpdateSeVolumeInputPort updateSeVolumeUseCase;
        private readonly INavigateBackInputPort navigateBackUseCase;
        private readonly CompositeDisposable disposables = new();

        [Inject]
        public SettingsController(
            IUpdateBgmVolumeInputPort updateBgmVolumeUseCase,
            IUpdateSeVolumeInputPort updateSeVolumeUseCase,
            INavigateBackInputPort navigateBackUseCase)
        {
            this.updateBgmVolumeUseCase = updateBgmVolumeUseCase;
            this.updateSeVolumeUseCase = updateSeVolumeUseCase;
            this.navigateBackUseCase = navigateBackUseCase;
        }

        /// <summary>
        /// Viewと接続してイベントを購読する
        /// </summary>
        public void Initialize(SettingsView view)
        {
            // BGM音量変更イベントを購読
            view.OnBgmVolumeChanged
                .Subscribe(volume => OnBgmVolumeChanged(volume))
                .AddTo(disposables);

            // SE音量変更イベントを購読
            view.OnSeVolumeChanged
                .Subscribe(volume => OnSeVolumeChanged(volume))
                .AddTo(disposables);

            // 戻るボタンイベントを購読
            view.OnBackButtonClicked
                .SubscribeAwait(async (_, ct) => await OnBackButtonClicked())
                .AddTo(disposables);
        }

        /// <summary>
        /// BGM音量が変更されたときの処理
        /// </summary>
        private void OnBgmVolumeChanged(float volume)
        {
            // 入力データの検証・加工
            float clampedVolume = Mathf.Clamp01(volume);

            // UseCaseを呼び出す
            updateBgmVolumeUseCase.Execute(clampedVolume);
        }

        /// <summary>
        /// SE音量が変更されたときの処理
        /// </summary>
        private void OnSeVolumeChanged(float volume)
        {
            // 入力データの検証・加工
            float clampedVolume = Mathf.Clamp01(volume);

            // UseCaseを呼び出す
            updateSeVolumeUseCase.Execute(clampedVolume);
        }

        /// <summary>
        /// 戻るボタンが押されたときの処理
        /// </summary>
        private async Cysharp.Threading.Tasks.UniTask OnBackButtonClicked()
        {
            // UseCaseを呼び出す
            await navigateBackUseCase.Execute();
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}

