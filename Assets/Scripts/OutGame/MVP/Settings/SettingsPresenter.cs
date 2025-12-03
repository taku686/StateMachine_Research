using System;
using Cysharp.Threading.Tasks;
using R3;

namespace OutGame.MVP.Settings
{
    /// <summary>
    /// 設定画面のPresenter
    /// </summary>
    public class SettingsPresenter : BasePresenter<SettingsView, SettingsModel>
    {
        public event Action OnBackRequested;

        public SettingsPresenter(SettingsView view, SettingsModel model) : base(view, model)
        {
        }

        public override async UniTask Initialize()
        {
            await base.Initialize();

            // 初期値の設定
            View.SetBgmVolume(Model.BgmVolume.CurrentValue);
            View.SetSeVolume(Model.SeVolume.CurrentValue);

            // スライダーイベントのバインド
            View.OnBgmVolumeChanged
                .Subscribe(volume => Model.SetBgmVolume(volume))
                .AddTo(Disposables);

            View.OnSeVolumeChanged
                .Subscribe(volume => Model.SetSeVolume(volume))
                .AddTo(Disposables);

            View.OnBackButtonClicked
                .Subscribe(_ => OnBackRequested?.Invoke())
                .AddTo(Disposables);
        }
    }
}

