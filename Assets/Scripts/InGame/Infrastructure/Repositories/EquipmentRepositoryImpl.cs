using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace InGame.Infrastructure.Repositories
{
    using InGame.Domain.Models;
    using InGame.Domain.Repositories;

    /// <summary>
    /// 装備リポジトリ実装
    /// TODO: OutGameの装備システム実装後に接続
    /// </summary>
    public class EquipmentRepositoryImpl : IEquipmentRepository
    {
        public async UniTask<List<Equipment>> GetEquippedItems()
        {
            // 現在は仮実装（空リスト）
            // 将来的にはOutGameのEquipmentRepositoryから取得
            await UniTask.CompletedTask;
            return new List<Equipment>();
        }
    }
}

