using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.MVP.Settings;
using UnityEngine.AddressableAssets;

namespace OutGame.States
{
    /// <summary>
    /// 設定画面のステート
    /// </summary>
    public class SettingsState : BaseState
    {
        private readonly StateMachine<OutGameStateKey> _stateMachine;
        private readonly OutGameStateKey _returnStateKey;
        private SettingsPresenter _presenter;

        public SettingsState(StateMachine<OutGameStateKey> stateMachine, OutGameStateKey returnStateKey)
        {
            _stateMachine = stateMachine;
            _returnStateKey = returnStateKey;
        }

        public override async UniTask OnEnter()
        {
            // Addressablesから View をロード
            var viewObject = await Addressables.InstantiateAsync("SettingsView");
            var view = viewObject.GetComponent<SettingsView>();

            // // Model を作成
            var model = new SettingsModel();

            // // Presenter を作成
            _presenter = new SettingsPresenter(view, model);
            await _presenter.Initialize();

            // // イベントをバインド
            _presenter.OnBackRequested += OnBackRequested;

            // View を表示
            await _presenter.Show();
        }

        public override async UniTask OnExit()
        {
            if (_presenter != null)
            {
                _presenter.OnBackRequested -= OnBackRequested;

                await _presenter.Hide();
                _presenter.Dispose();
                _presenter = null;
            }
        }

        private void OnBackRequested()
        {
            _stateMachine.ChangeState(_returnStateKey).Forget();
        }
    }
}