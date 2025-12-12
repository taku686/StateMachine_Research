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
        private readonly IStartBattleInputPort startBattleUseCase;
        private readonly CompositeDisposable disposables = new();

        [Inject]
        public HomeController(
            IOpenSettingsInputPort openSettingsUseCase,
            IStartBattleInputPort startBattleUseCase)
        {
            this.openSettingsUseCase = openSettingsUseCase;
            this.startBattleUseCase = startBattleUseCase;
        }

        /// <summary>
        /// Viewと接続してイベントを購読する
        /// </summary>
        public void Initialize(HomeView view)
        {
            // クエストボタンイベントを購読
            view.OnQuestButtonClicked
                .SubscribeAwait(async (_, ct) => await OnQuestButtonClicked())
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
        private async Cysharp.Threading.Tasks.UniTask OnQuestButtonClicked()
        {
            // バトル開始UseCase呼び出し
            // TODO: 将来的にはステージ選択UIを実装
            int selectedStageId = 1; // 仮でステージ1を指定
            await startBattleUseCase.Execute(selectedStageId);
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

