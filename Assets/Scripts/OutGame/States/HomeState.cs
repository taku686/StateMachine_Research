using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.Infrastructure.Views;
using OutGame.Presentation.Controllers;
using OutGame.Domain.Repositories;
using UnityEngine.AddressableAssets;
using Zenject;

namespace OutGame.States
{
    /// <summary>
    /// ホーム画面のステート
    /// クリーンアーキテクチャに基づいた実装
    /// </summary>
    public class HomeState : BaseState
    {
        private readonly HomeController controller;
        private readonly IUserProfileRepository userProfileRepository;
        private HomeView view;

        [Inject]
        public HomeState(
            HomeController controller,
            IUserProfileRepository userProfileRepository)
        {
            this.controller = controller;
            this.userProfileRepository = userProfileRepository;
        }

        public override async UniTask OnEnter()
        {
            // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync("HomeView");
            view = viewObject.GetComponent<HomeView>();

            // Controllerを初期化（ViewとUseCaseを接続）
            controller.Initialize(view);

            // TODO: ユーザープロフィールを読み込んで表示
            // var userProfile = userProfileRepository.Load();
            // view.SetPlayerLevel(userProfile.Level);
            // view.SetPlayerGold(userProfile.Gold);

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

