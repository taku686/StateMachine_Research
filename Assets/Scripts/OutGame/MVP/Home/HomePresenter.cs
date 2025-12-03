using System;
using Cysharp.Threading.Tasks;
using R3;

namespace OutGame.MVP.Home
{
    /// <summary>
    /// ホーム画面のPresenter
    /// </summary>
    public class HomePresenter : BasePresenter<HomeView, HomeModel>
    {
        public event Action OnQuestRequested;
        public event Action OnShopRequested;
        public event Action OnSettingsRequested;

        public HomePresenter(HomeView view, HomeModel model) : base(view, model)
        {
        }

        public override async UniTask Initialize()
        {
            await base.Initialize();

            // ボタンイベントのバインド
            View.OnQuestButtonClicked
                .Subscribe(_ => OnQuestRequested?.Invoke())
                .AddTo(Disposables);

            View.OnShopButtonClicked
                .Subscribe(_ => OnShopRequested?.Invoke())
                .AddTo(Disposables);

            View.OnSettingsButtonClicked
                .Subscribe(_ => OnSettingsRequested?.Invoke())
                .AddTo(Disposables);
        }
    }
}

