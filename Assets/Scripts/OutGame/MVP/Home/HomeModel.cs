using R3;

namespace OutGame.MVP.Home
{
    /// <summary>
    /// ホーム画面のModel
    /// </summary>
    public class HomeModel : BaseModel
    {
        private readonly ReactiveProperty<int> _playerLevel = new(1);
        private readonly ReactiveProperty<int> _playerGold = new(1000);

        public ReadOnlyReactiveProperty<int> PlayerLevel => _playerLevel;
        public ReadOnlyReactiveProperty<int> PlayerGold => _playerGold;

        public HomeModel()
        {
            _playerLevel.AddTo(Disposables);
            _playerGold.AddTo(Disposables);
        }

        public void SetPlayerLevel(int level)
        {
            _playerLevel.Value = level;
        }

        public void SetPlayerGold(int gold)
        {
            _playerGold.Value = gold;
        }
    }
}

