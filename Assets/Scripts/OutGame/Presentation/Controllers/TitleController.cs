using OutGame.Application.Ports.Input;
using OutGame.Infrastructure.Views;
using R3;
using Zenject;

namespace OutGame.Presentation.Controllers
{
    /// <summary>
    /// タイトル画面のController
    /// Viewからの入力を受け取り、UseCaseに渡す
    /// </summary>
    public class TitleController
    {
        private readonly INavigateToHomeInputPort navigateToHomeUseCase;
        private readonly IOpenSettingsInputPort openSettingsUseCase;
        private readonly CompositeDisposable disposables = new();

        [Inject]
        public TitleController(
            INavigateToHomeInputPort navigateToHomeUseCase,
            IOpenSettingsInputPort openSettingsUseCase)
        {
            this.navigateToHomeUseCase = navigateToHomeUseCase;
            this.openSettingsUseCase = openSettingsUseCase;
        }

        /// <summary>
        /// Viewと接続してイベントを購読する
        /// </summary>
        public void Initialize(TitleView view)
        {
            // スタートボタンイベントを購読
            view.OnStartButtonClicked
                .SubscribeAwait(async (_, ct) => await OnStartButtonClicked())
                .AddTo(disposables);

            // 設定ボタンイベントを購読
            view.OnSettingsButtonClicked
                .SubscribeAwait(async (_, ct) => await OnSettingsButtonClicked())
                .AddTo(disposables);
        }

        /// <summary>
        /// スタートボタンが押されたときの処理
        /// </summary>
        private async Cysharp.Threading.Tasks.UniTask OnStartButtonClicked()
        {
            // UseCaseを呼び出す
            await navigateToHomeUseCase.Execute();
        }

        /// <summary>
        /// 設定ボタンが押されたときの処理
        /// </summary>
        private async Cysharp.Threading.Tasks.UniTask OnSettingsButtonClicked()
        {
            // UseCaseを呼び出す
            await openSettingsUseCase.Execute();
        }

        public void Dispose()
        {
            disposables?.Dispose();
        }
    }
}

