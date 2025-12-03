using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.States;
using UnityEngine;
using Zenject;

namespace OutGame
{
    /// <summary>
    /// アウトゲーム全体を管理するマネージャー
    /// </summary>
    public class OutGameManager : MonoBehaviour
    {
        private StateMachine<OutGameStateKey> _stateMachine;

        [Inject]
        public void Construct()
        {
            InitializeStateMachine();
        }

        private void Start()
        {
            // 初期ステートをタイトルに設定
            _stateMachine.ChangeState(OutGameStateKey.Title).Forget();
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine<OutGameStateKey>();

            // 各ステートを登録
            _stateMachine.RegisterState(OutGameStateKey.Title, new TitleState(_stateMachine));
            _stateMachine.RegisterState(OutGameStateKey.Home, new HomeState(_stateMachine));
            _stateMachine.RegisterState(OutGameStateKey.Settings, new SettingsState(_stateMachine, OutGameStateKey.Title));
        }
    }
}

