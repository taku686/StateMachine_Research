using Cysharp.Threading.Tasks;

namespace InGame.Application.Ports.Input
{
    /// <summary>
    /// バトル開始ユースケースの入力ポート
    /// </summary>
    public interface IStartBattleInputPort
    {
        UniTask Execute(string stageId);
    }
}

