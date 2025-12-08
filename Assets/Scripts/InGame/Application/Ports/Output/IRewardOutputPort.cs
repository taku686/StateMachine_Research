using Cysharp.Threading.Tasks;

namespace InGame.Application.Ports.Output
{
    using InGame.Domain.Models;

    /// <summary>
    /// 報酬イベントの出力ポート
    /// </summary>
    public interface IRewardOutputPort
    {
        /// <summary>
        /// 報酬獲得時
        /// </summary>
        UniTask OnRewardAcquired(BattleReward reward);
    }
}

