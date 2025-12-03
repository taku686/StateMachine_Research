using Cysharp.Threading.Tasks;
using OutGame.Application.Ports.Input;
using OutGame.Application.Ports.Output;
using OutGame.StateMachine;
using Zenject;

namespace OutGame.Application.UseCases
{
    /// <summary>
    /// 前の画面に戻るInteractor
    /// </summary>
    public class NavigateBackInteractor : INavigateBackInputPort
    {
        private readonly INavigationOutputPort outputPort;
        private readonly StateMachine<OutGameStateKey> stateMachine;

        [Inject]
        public NavigateBackInteractor(
            INavigationOutputPort outputPort,
            StateMachine<OutGameStateKey> stateMachine)
        {
            this.outputPort = outputPort;
            this.stateMachine = stateMachine;
        }

        public async UniTask Execute()
        {
            // 戻る前の状態を取得（ログ用）
            var currentState = stateMachine.CurrentStateKey;

            // 前の画面に戻る
            await stateMachine.GoBack();

            // 遷移完了を通知
            outputPort.OnNavigationCompleted();
        }
    }
}

