using UnityEngine;
using UnityEngine.UI;
using R3;

namespace OutGame.MVP.Settings
{
    /// <summary>
    /// 設定画面のView
    /// </summary>
    public class SettingsView : BaseView
    {
        [SerializeField] private Slider _bgmVolumeSlider;
        [SerializeField] private Slider _seVolumeSlider;
        [SerializeField] private Button _backButton;

        public Observable<float> OnBgmVolumeChanged => 
            _bgmVolumeSlider.OnValueChangedAsObservable();

        public Observable<float> OnSeVolumeChanged => 
            _seVolumeSlider.OnValueChangedAsObservable();

        public Observable<Unit> OnBackButtonClicked => 
            _backButton.OnClickAsObservable();

        public void SetBgmVolume(float volume)
        {
            _bgmVolumeSlider.value = volume;
        }

        public void SetSeVolume(float volume)
        {
            _seVolumeSlider.value = volume;
        }
    }
}

