using R3;

namespace OutGame.MVP.Settings
{
    /// <summary>
    /// 設定画面のModel
    /// </summary>
    public class SettingsModel : BaseModel
    {
        private readonly ReactiveProperty<float> _bgmVolume = new(0.8f);
        private readonly ReactiveProperty<float> _seVolume = new(0.8f);

        public ReadOnlyReactiveProperty<float> BgmVolume => _bgmVolume;
        public ReadOnlyReactiveProperty<float> SeVolume => _seVolume;

        public SettingsModel()
        {
            _bgmVolume.AddTo(Disposables);
            _seVolume.AddTo(Disposables);
        }

        public void SetBgmVolume(float volume)
        {
            _bgmVolume.Value = volume;
        }

        public void SetSeVolume(float volume)
        {
            _seVolume.Value = volume;
        }
    }
}

