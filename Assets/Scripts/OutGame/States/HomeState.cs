using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.MVP.Home;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OutGame.States
{
    /// <summary>
    /// ホーム画面のステート
    /// </summary>
    public class HomeState : BaseState
    {
        private readonly StateMachine<OutGameStateKey> _stateMachine;
        private HomePresenter _presenter;

        public HomeState(StateMachine<OutGameStateKey> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override async UniTask OnEnter()
        {
            // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync("HomeView");
            var view = viewObject.GetComponent<HomeView>();

            // Model を作成
            var model = new HomeModel();

            // Presenter を作成
            _presenter = new HomePresenter(view, model);
            await _presenter.Initialize();

            // イベントをバインド
            _presenter.OnQuestRequested += OnQuestRequested;
            _presenter.OnShopRequested += OnShopRequested;
            _presenter.OnSettingsRequested += OnSettingsRequested;

            // View を表示
            await _presenter.Show();
        }

        public override async UniTask OnExit()
        {
            if (_presenter != null)
            {
                _presenter.OnQuestRequested -= OnQuestRequested;
                _presenter.OnShopRequested -= OnShopRequested;
                _presenter.OnSettingsRequested -= OnSettingsRequested;

                await _presenter.Hide();
                _presenter.Dispose();
                _presenter = null;
            }
        }

        private void OnQuestRequested()
        {
            Debug.Log("Quest requested");
            // TODO: クエスト画面への遷移
        }

        private void OnShopRequested()
        {
            Debug.Log("Shop requested");
            // TODO: ショップ画面への遷移
        }

        private void OnSettingsRequested()
        {
            _stateMachine.ChangeState(OutGameStateKey.Settings).Forget();
        }
    }
}

