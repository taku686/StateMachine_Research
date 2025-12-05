using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Presentation.Controllers;
using UnityEngine.AddressableAssets;
using Zenject;

namespace OutGame.States
{
    /// <summary>
    /// タイトル画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// </summary>
    public class TitleState : BaseState
    {
        private readonly TitleController controller;
        private TitleView view;

        [Inject]
        public TitleState(TitleController controller)
        {
            this.controller = controller;
        }

        public override async UniTask OnEnter()
        {
            // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync(nameof(TitleView));
            view = viewObject.GetComponent<TitleView>();

            // Controllerを初期化（ViewとUseCaseを接続）
            controller.Initialize(view);

            // View を表示
            await view.Show();
        }

        public override async UniTask OnExit()
        {
            controller?.Dispose();

            if (view != null)
            {
                await view.Hide();
                view.Dispose();
                view = null;
            }
        }
    }
}