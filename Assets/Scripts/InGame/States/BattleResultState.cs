using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.States
{
    using InGame.StateMachine;

    /// <summary>
    /// リザルトState
    /// バトル結果を表示
    /// </summary>
    public class BattleResultState : BaseInGameState
    {
        [Inject]
        public BattleResultState()
        {
        }

        public override async UniTask OnEnter()
        {
            Debug.Log("Battle Result");

            // TODO: リザルトUI表示
            // TODO: OutGameへ遷移する処理

            await UniTask.Delay(System.TimeSpan.FromSeconds(3));

            Debug.Log("Returning to OutGame...");
        }
    }
}

