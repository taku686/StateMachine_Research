using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.MVP.Title;
using UnityEngine.AddressableAssets;

namespace OutGame.States
{
    /// <summary>
    /// タイトル画面のステート
    /// </summary>
    public class TitleState : BaseState
    {
        private readonly StateMachine<OutGameStateKey> _stateMachine;
        private TitlePresenter _presenter;

        public TitleState(StateMachine<OutGameStateKey> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public override async UniTask OnEnter()
        {
            // // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync("TitleView");
            var view = viewObject.GetComponent<TitleView>();

            // // Model を作成
            var model = new TitleModel();

            // // Presenter を作成
            _presenter = new TitlePresenter(view, model);
            await _presenter.Initialize();

            // // イベントをバインド
            _presenter.OnStartRequested += OnStartRequested;
            _presenter.OnSettingsRequested += OnSettingsRequested;

            // // View を表示
            await _presenter.Show();
        }

        public override async UniTask OnExit()
        {
            if (_presenter != null)
            {
                _presenter.OnStartRequested -= OnStartRequested;
                _presenter.OnSettingsRequested -= OnSettingsRequested;

                await _presenter.Hide();
                _presenter.Dispose();
                _presenter = null;
            }
        }

        private void OnStartRequested()
        {
            _stateMachine.ChangeState(OutGameStateKey.Home).Forget();
        }

        private void OnSettingsRequested()
        {
            _stateMachine.ChangeState(OutGameStateKey.Settings).Forget();
        }
    }
}