using Cysharp.Threading.Tasks;

namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// ホーム画面に遷移するUseCaseのインターフェース
    /// </summary>
    public interface INavigateToHomeInputPort
    {
        UniTask Execute();
    }
}

