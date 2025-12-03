using Cysharp.Threading.Tasks;

namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// 前の画面に戻るUseCaseのインターフェース
    /// </summary>
    public interface INavigateBackInputPort
    {
        UniTask Execute();
    }
}

