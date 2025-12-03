using OutGame.Application.Ports.Input;
using OutGame.Infrastructure.Views;
using R3;
using UnityEngine;
using Zenject;

namespace OutGame.Presentation.Controllers
{
    /// <summary>
    /// ホーム画面のController
    /// Viewからの入力を受け取り、UseCaseに渡す
    /// </summary>
    public class HomeController
    {
        private readonly IOpenSettingsInputPort openSettingsUseCase;
        private readonly CompositeDisposable disposables = new();

        [Inject]
        public HomeController(IOpenSettingsInputPort openSettingsUseCase)
        {
            this.openSettingsUseCase = openSettingsUseCase;
        }

        /// <summary>
        /// Viewと接続してイベントを購読する
        /// </summary>
        public void Initialize(HomeView view)
        {
            // クエストボタンイベントを購読
            view.OnQuestButtonClicked
                .Subscribe(_ => OnQuestButtonClicked())
                .AddTo(disposables);

            // ショップボタンイベントを購読
            view.OnShopButtonClicked
                .Subscribe(_ => OnShopButtonClicked())
                .AddTo(disposables);

            // 設定ボタンイベントを購読
            view.OnSettingsButtonClicked
                .SubscribeAwait(async (_, ct) => await OnSettingsButtonClicked())
                .AddTo(disposables);
        }

        /// <summary>
        /// クエストボタンが押されたときの処理
        /// </summary>
        private void OnQuestButtonClicked()
        {
            // TODO: クエスト画面への遷移UseCaseを実装
            Debug.Log("Quest button clicked");
        }

        /// <summary>
        /// ショップボタンが押されたときの処理
        /// </summary>
        private void OnShopButtonClicked()
        {
            // TODO: ショップ画面への遷移UseCaseを実装
            Debug.Log("Shop button clicked");
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

