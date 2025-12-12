using Cysharp.Threading.Tasks;

namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// バトル開始UseCase
    /// </summary>
    public interface IStartBattleInputPort
    {
        /// <summary>
        /// バトルを開始してInGameシーンへ遷移
        /// </summary>
        /// <param name="stageId">開始するステージID</param>
        UniTask Execute(int stageId);
    }
}
