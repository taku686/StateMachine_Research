using Cysharp.Threading.Tasks;
using OutGame.Application.Ports.Input;
using OutGame.Application.Ports.Output;
using OutGame.StateMachine;
using Zenject;

namespace OutGame.Application.UseCases
{
    /// <summary>
    /// 設定画面を開くInteractor
    /// </summary>
    public class OpenSettingsInteractor : IOpenSettingsInputPort
    {
        private readonly INavigationOutputPort outputPort;
        private readonly StateMachine<OutGameStateKey> stateMachine;

        [Inject]
        public OpenSettingsInteractor(
            INavigationOutputPort outputPort,
            StateMachine<OutGameStateKey> stateMachine)
        {
            this.outputPort = outputPort;
            this.stateMachine = stateMachine;
        }

        public async UniTask Execute()
        {
            // 遷移開始を通知
            outputPort.OnNavigationStarted(OutGameStateKey.Settings);

            // 状態遷移を実行
            await stateMachine.ChangeState(OutGameStateKey.Settings);

            // 遷移完了を通知
            outputPort.OnNavigationCompleted();
        }
    }
}

