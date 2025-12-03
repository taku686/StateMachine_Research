using System;
using Cysharp.Threading.Tasks;
using R3;

namespace OutGame.MVP.Title
{
    /// <summary>
    /// タイトル画面のPresenter
    /// </summary>
    public class TitlePresenter : BasePresenter<TitleView, TitleModel>
    {
        public event Action OnStartRequested;
        public event Action OnSettingsRequested;

        public TitlePresenter(TitleView view, TitleModel model) : base(view, model)
        {
        }

        public override async UniTask Initialize()
        {
            await base.Initialize();

            // ボタンイベントのバインド
            View.OnStartButtonClicked
                .Subscribe(_ => OnStartRequested?.Invoke())
                .AddTo(Disposables);

            View.OnSettingsButtonClicked
                .Subscribe(_ => OnSettingsRequested?.Invoke())
                .AddTo(Disposables);
        }
    }
}

