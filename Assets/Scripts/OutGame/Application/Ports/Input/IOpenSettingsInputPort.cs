using Cysharp.Threading.Tasks;

namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// 設定画面を開くUseCaseのインターフェース
    /// </summary>
    public interface IOpenSettingsInputPort
    {
        UniTask Execute();
    }
}

