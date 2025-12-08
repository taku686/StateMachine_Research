using Cysharp.Threading.Tasks;

namespace InGame.Domain.Repositories
{
    using InGame.Domain.Models;

    /// <summary>
    /// プレイヤーリポジトリのインターフェース
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// プレイヤーを取得（OutGameから）
        /// </summary>
        UniTask<Player> GetPlayer();

        /// <summary>
        /// プレイヤーを保存（OutGameへ）
        /// </summary>
        UniTask SavePlayer(Player player);
    }
}

