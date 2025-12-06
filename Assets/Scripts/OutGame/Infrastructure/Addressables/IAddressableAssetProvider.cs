using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.Infrastructure.Addressables
{
    /// <summary>
    /// Addressablesからアセットを取得するプロバイダー
    /// インスタンス化はInstallerに任せ、アセットの取得のみを担当
    /// DRY原則に基づき、すべてのAddressablesロード処理を一元管理
    /// </summary>
    public interface IAddressableAssetProvider
    {
        /// <summary>
        /// Addressablesからプレハブアセットを取得（非インスタンス化）
        /// </summary>
        /// <param name="key">アセットキー</param>
        /// <returns>ロードされたプレハブ</returns>
        UniTask<GameObject> LoadPrefabAsync(string key);

        /// <summary>
        /// アセットをアンロード
        /// </summary>
        /// <param name="asset">アンロードするアセット</param>
        void ReleaseAsset(GameObject asset);
    }
}

