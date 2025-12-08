using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace InGame.Domain.Repositories
{
    using InGame.Domain.Models;

    /// <summary>
    /// 装備リポジトリのインターフェース
    /// </summary>
    public interface IEquipmentRepository
    {
        /// <summary>
        /// 装備中のアイテムを取得（OutGameから）
        /// </summary>
        UniTask<List<Equipment>> GetEquippedItems();
    }
}

