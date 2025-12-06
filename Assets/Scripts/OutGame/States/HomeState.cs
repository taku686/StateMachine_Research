using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Infrastructure.Factories;
using OutGame.Presentation.Controllers;
using OutGame.Domain.Repositories;
using OutGame.Domain.Models;
using Zenject;
using UnityEngine;

namespace OutGame.States
{
    /// <summary>
    /// ホーム画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// アセットのプリロード/クリーンアップは基底クラスで自動処理
    /// </summary>
    public class HomeState : BaseState
    {
        private readonly HomeController controller;
        private readonly IUserProfileRepository userProfileRepository;
        private readonly IViewFactory<HomeView> viewFactory;
        private HomeView view;
        private UserProfile userProfile; // ロードしたデータを保持

        [Inject]
        public HomeState(
            HomeController controller,
            IUserProfileRepository userProfileRepository,
            IViewFactory<HomeView> viewFactory,
            AddressableViewFactory<HomeView> addressableFactory)
        {
            this.controller = controller;
            this.userProfileRepository = userProfileRepository;
            this.viewFactory = viewFactory;

            // Viewアセットをプリロード対象として登録
            // 基底クラスが自動的にプリロード/アンロードを処理
            RegisterPreloadableAsset(addressableFactory);
        }

        /// <summary>
        /// データのロード（アセットプリロードとは別）
        /// OnEnter前に実行され、必要なデータを事前に取得
        /// </summary>
        protected override async UniTask OnLoadDataAsync()
        {
            Debug.Log("[HomeState] Loading user profile data...");

            // Repositoryからユーザープロフィールをロード
            userProfile = userProfileRepository.Load();

            Debug.Log($"[HomeState] User profile loaded: Level={userProfile.Level}, Gold={userProfile.Gold}");

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// ステートに入る
        /// アセットとデータは既にロード済みなので即座に使用可能
        /// </summary>
        public override async UniTask OnEnter()
        {
            Debug.Log("[HomeState] Entering state...");

            // アセットは既にプリロード済みなので即座に作成
            view = await viewFactory.CreateAsync();

            // Controllerを初期化
            controller.Initialize(view);

            // ロード済みのデータをViewに設定
            view.SetPlayerLevel(userProfile.Level);
            view.SetPlayerGold(userProfile.Gold);

            // Viewを表示
            await view.Show();

            Debug.Log("[HomeState] State entered!");
        }

        /// <summary>
        /// ステートから出る
        /// </summary>
        public override async UniTask OnExit()
        {
            Debug.Log("[HomeState] Exiting state...");

            controller?.Dispose();

            if (view != null)
            {
                await view.Hide();
                view.Dispose();
                view = null;
            }

            Debug.Log("[HomeState] State exited!");
        }

        // OnPreloadAssetsAsync や OnCleanupAssetsAsync のオーバーライドは不要
        // 基底クラスが自動的に処理してくれる
    }
}

