using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace OutGame.Infrastructure.Addressables
{
    /// <summary>
    /// Addressablesからアセットを取得する実装
    /// すべてのAddressables.LoadAssetAsync呼び出しを一元管理
    /// DRY原則に基づき、ロード処理を集約
    /// </summary>
    public class AddressableAssetProvider : IAddressableAssetProvider
    {
        public async UniTask<GameObject> LoadPrefabAsync(string key)
        {
            try
            {
                var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(key);
                var prefab = await handle.ToUniTask();

                if (prefab == null)
                {
                    Debug.LogError($"Failed to load prefab: {key}");
                    throw new InvalidOperationException($"Prefab not found: {key}");
                }

                Debug.Log($"Successfully loaded prefab: {key}");
                return prefab;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading prefab {key}: {ex.Message}");
                throw;
            }
        }

        public void ReleaseAsset(GameObject asset)
        {
            if (asset != null)
            {
                UnityEngine.AddressableAssets.Addressables.Release(asset);
                Debug.Log($"Released asset: {asset.name}");
            }
        }
    }
}

