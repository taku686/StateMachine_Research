using Cysharp.Threading.Tasks;

namespace InGame.Application.Ports.Input
{
    /// <summary>
    /// バトル完了ユースケースの入力ポート
    /// </summary>
    public interface ICompleteBattleInputPort
    {
        UniTask Execute(bool isCleared);
    }
}

