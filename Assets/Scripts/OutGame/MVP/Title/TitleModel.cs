using R3;

namespace OutGame.MVP.Title
{
    /// <summary>
    /// タイトル画面のModel
    /// </summary>
    public class TitleModel : BaseModel
    {
        private readonly ReactiveProperty<string> _gameVersion = new("1.0.0");

        public ReadOnlyReactiveProperty<string> GameVersion => _gameVersion;

        public TitleModel()
        {
            _gameVersion.AddTo(Disposables);
        }
    }
}

