using UnityEngine;
using UnityEngine.UI;
using R3;

namespace OutGame.Infrastructure.Views
{
    /// <summary>
    /// ホーム画面のView
    /// </summary>
    public class HomeView : BaseView
    {
        [SerializeField] private Button _questButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Text _playerLevelText;
        [SerializeField] private Text _playerGoldText;

        public Observable<Unit> OnQuestButtonClicked =>
            _questButton.OnClickAsObservable();

        public Observable<Unit> OnShopButtonClicked =>
            _shopButton.OnClickAsObservable();

        public Observable<Unit> OnSettingsButtonClicked =>
            _settingsButton.OnClickAsObservable();

        /// <summary>
        /// プレイヤーレベルを設定
        /// </summary>
        public void SetPlayerLevel(int level)
        {
            if (_playerLevelText != null)
            {
                _playerLevelText.text = $"Lv.{level}";
            }
        }

        /// <summary>
        /// プレイヤーのゴールドを設定
        /// </summary>
        public void SetPlayerGold(int gold)
        {
            if (_playerGoldText != null)
            {
                _playerGoldText.text = gold.ToString("N0");
            }
        }
    }
}

