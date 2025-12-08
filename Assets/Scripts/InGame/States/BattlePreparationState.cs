using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.States
{
    using InGame.Application.Ports.Input;
    using InGame.StateMachine;

    /// <summary>
    /// バトル準備State
    /// プレイヤー/敵の初期配置を行う
    /// </summary>
    public class BattlePreparationState : BaseInGameState
    {
        private readonly IStartBattleInputPort startBattleInteractor;
        private readonly InGameStateMachine stateMachine;

        [Inject]
        public BattlePreparationState(
            IStartBattleInputPort startBattleInteractor,
            InGameStateMachine stateMachine)
        {
            this.startBattleInteractor = startBattleInteractor;
            this.stateMachine = stateMachine;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("Battle Preparation Started");

            // バトル開始処理
            await startBattleInteractor.Execute("Stage_1");

            // 準備完了後、バトル開始
            await stateMachine.ChangeState(InGameStateKey.Battle);
        }
    }
}

