using Cysharp.Threading.Tasks;
using OutGame.Application.Ports.Input;
using OutGame.Application.Ports.Output;
using OutGame.StateMachine;
using Zenject;

namespace OutGame.Application.UseCases
{
    /// <summary>
    /// ホーム画面に遷移するInteractor
    /// </summary>
    public class NavigateToHomeInteractor : INavigateToHomeInputPort
    {
        private readonly INavigationOutputPort outputPort;
        private readonly StateMachine<OutGameStateKey> stateMachine;

        [Inject]
        public NavigateToHomeInteractor(
            INavigationOutputPort outputPort,
            StateMachine<OutGameStateKey> stateMachine)
        {
            this.outputPort = outputPort;
            this.stateMachine = stateMachine;
        }

        public async UniTask Execute()
        {
            // 遷移開始を通知
            outputPort.OnNavigationStarted(OutGameStateKey.Home);

            // 状態遷移を実行
            await stateMachine.ChangeState(OutGameStateKey.Home);

            // 遷移完了を通知
            outputPort.OnNavigationCompleted();
        }
    }
}

