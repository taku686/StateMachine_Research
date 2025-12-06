using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Infrastructure.Factories;
using OutGame.Presentation.Controllers;
using Zenject;
using UnityEngine;

namespace OutGame.States
{
    /// <summary>
    /// タイトル画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// アセットのプリロード/クリーンアップは基底クラスで自動処理
    /// </summary>
    public class TitleState : BaseState
    {
        private readonly TitleController controller;
        private readonly IViewFactory<TitleView> viewFactory;
        private TitleView view;

        [Inject]
        public TitleState(
            TitleController controller,
            IViewFactory<TitleView> viewFactory,
            AddressableViewFactory<TitleView> addressableFactory)
        {
            this.controller = controller;
            this.viewFactory = viewFactory;

            // Viewアセットをプリロード対象として登録
            RegisterPreloadableAsset(addressableFactory);
        }

        /// <summary>
        /// ステートに入る
        /// アセットは既にプリロード済み
        /// </summary>
        public override async UniTask OnEnter()
        {
            Debug.Log("[TitleState] Entering state...");

            // アセットは既にプリロード済みなので即座に作成
            view = await viewFactory.CreateAsync();

            // Controllerを初期化
            controller.Initialize(view);

            // Viewを表示
            await view.Show();

            Debug.Log("[TitleState] State entered!");
        }

        /// <summary>
        /// ステートから出る
        /// </summary>
        public override async UniTask OnExit()
        {
            Debug.Log("[TitleState] Exiting state...");

            controller?.Dispose();

            if (view != null)
            {
                await view.Hide();
                view.Dispose();
                view = null;
            }

            Debug.Log("[TitleState] State exited!");
        }

        // データロードは不要なのでOnLoadDataAsyncはオーバーライドしない
        // アセットのプリロード/クリーンアップは基底クラスが自動処理
    }
}