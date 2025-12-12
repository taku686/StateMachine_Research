using Cysharp.Threading.Tasks;
using InGame.StateMachine;
using InGame.States;
using UnityEngine;
using Zenject;

namespace InGame
{
    /// <summary>
    /// インゲーム全体を管理するマネージャー
    /// クリーンアーキテクチャに基づいた実装
    /// </summary>
    public class InGameManager : MonoBehaviour
    {
        private InGameStateMachine _stateMachine;
        private BattlePreparationState _preparationState;
        private BattleState _battleState;
        private BattleResultState _resultState;

        [Inject]
        public void Construct(
            InGameStateMachine stateMachine,
            BattlePreparationState preparationState,
            BattleState battleState,
            BattleResultState resultState)
        {
            _stateMachine = stateMachine;
            _preparationState = preparationState;
            _battleState = battleState;
            _resultState = resultState;

            InitializeStateMachine();
        }

        private void Start()
        {
            Debug.Log("[InGameManager] Starting InGame...");
            
            // バトル準備ステートから開始
            _stateMachine.ChangeState(InGameStateKey.Preparation).Forget();
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void InitializeStateMachine()
        {
            Debug.Log("[InGameManager] Initializing StateMachine...");
            
            // 各ステートを登録
            _stateMachine.RegisterState(InGameStateKey.Preparation, _preparationState);
            _stateMachine.RegisterState(InGameStateKey.Battle, _battleState);
            _stateMachine.RegisterState(InGameStateKey.Result, _resultState);
            
            Debug.Log("[InGameManager] StateMachine initialized");
        }
    }
}
