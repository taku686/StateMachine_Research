using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.States
{
    using InGame.Application.Ports.Input;
    using InGame.StateMachine;
    using OutGame.Domain.Repositories;

    /// <summary>
    /// バトル準備State
    /// プレイヤー/敵の初期配置を行う
    /// </summary>
    public class BattlePreparationState : BaseInGameState
    {
        private readonly IStartBattleInputPort startBattleInteractor;
        private readonly InGameStateMachine stateMachine;
        private readonly IUserProfileRepository userProfileRepository;

        [Inject]
        public BattlePreparationState(
            IStartBattleInputPort startBattleInteractor,
            InGameStateMachine stateMachine,
            IUserProfileRepository userProfileRepository)
        {
            this.startBattleInteractor = startBattleInteractor;
            this.stateMachine = stateMachine;
            this.userProfileRepository = userProfileRepository;
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("[BattlePreparationState] Battle Preparation Started");

            // OutGameで設定されたステージIDを取得
            var profile = userProfileRepository.Load();
            var stageId = profile.CurrentStageId;

            Debug.Log($"[BattlePreparationState] Loading stage: {stageId}");

            // バトル開始処理
            // TODO: ステージIDに応じた敵データをロード
            await startBattleInteractor.Execute($"Stage_{stageId}");

            // 準備完了後、バトル開始
            await stateMachine.ChangeState(InGameStateKey.Battle);
        }
    }
}

