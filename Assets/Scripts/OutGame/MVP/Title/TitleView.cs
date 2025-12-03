using UnityEngine;
using UnityEngine.UI;
using R3;

namespace OutGame.MVP.Title
{
    /// <summary>
    /// タイトル画面のView
    /// </summary>
    public class TitleView : BaseView
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;

        public Observable<Unit> OnStartButtonClicked => 
            _startButton.OnClickAsObservable();

        public Observable<Unit> OnSettingsButtonClicked => 
            _settingsButton.OnClickAsObservable();
    }
}

