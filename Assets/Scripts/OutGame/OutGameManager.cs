using Cysharp.Threading.Tasks;
using OutGame.StateMachine;
using OutGame.States;
using UnityEngine;
using Zenject;

namespace OutGame
{
    /// <summary>
    /// アウトゲーム全体を管理するマネージャー
    /// クリーンアーキテクチャに基づいた実装
    /// </summary>
    public class OutGameManager : MonoBehaviour
    {
        private StateMachine<OutGameStateKey> _stateMachine;
        private TitleState _titleState;
        private HomeState _homeState;
        private SettingsState _settingsState;

        [Inject]
        public void Construct(
            StateMachine<OutGameStateKey> stateMachine,
            TitleState titleState,
            HomeState homeState,
            SettingsState settingsState)
        {
            _stateMachine = stateMachine;
            _titleState = titleState;
            _homeState = homeState;
            _settingsState = settingsState;

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
            // 各ステートを登録（Zenjectで注入されたインスタンスを使用）
            // isRootState = true: 履歴をクリアする基準画面（Title, Home）
            _stateMachine.RegisterState(OutGameStateKey.Title, _titleState, isRootState: true);
            _stateMachine.RegisterState(OutGameStateKey.Home, _homeState, isRootState: true);
            _stateMachine.RegisterState(OutGameStateKey.Settings, _settingsState);
        }
    }
}

