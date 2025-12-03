using UnityEngine;
using UnityEngine.UI;
using R3;

namespace OutGame.MVP.Home
{
    /// <summary>
    /// ホーム画面のView
    /// </summary>
    public class HomeView : BaseView
    {
        [SerializeField] private Button _questButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _settingsButton;

        public Observable<Unit> OnQuestButtonClicked => 
            _questButton.OnClickAsObservable();

        public Observable<Unit> OnShopButtonClicked => 
            _shopButton.OnClickAsObservable();

        public Observable<Unit> OnSettingsButtonClicked => 
            _settingsButton.OnClickAsObservable();
    }
}

