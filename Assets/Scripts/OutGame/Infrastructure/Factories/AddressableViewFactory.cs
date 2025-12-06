using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using OutGame.Infrastructure.Addressables;
using OutGame.Infrastructure.Views;
using Zenject;

namespace OutGame.Infrastructure.Factories
{
    /// <summary>
    /// Addressablesからロードしたプレハブを使用してViewを生成するカスタムファクトリー
    /// Installerから使用され、実際のインスタンス化を担当
    /// Zenjectのコンテナを使用してDI対応のインスタンス化を実現
    /// IAssetPreloadableを実装し、BaseStateで自動的にプリロード/アンロード
    /// </summary>
    /// <typeparam name="TView">作成するViewの型</typeparam>
    public class AddressableViewFactory<TView> : IFactory<TView>, IAssetPreloadable
        where TView : Component, IView
    {
        private readonly DiContainer container;
        private readonly IAddressableAssetProvider assetProvider;
        private readonly Canvas canvas;
        private readonly string assetKey;
        private GameObject cachedPrefab;

        public AddressableViewFactory(
            DiContainer container,
            IAddressableAssetProvider assetProvider,
            Canvas canvas,
            string assetKey)
        {
            this.container = container;
            this.assetProvider = assetProvider;
            this.canvas = canvas;
            this.assetKey = assetKey;
        }

        public TView Create()
        {
            // プレハブがまだロードされていない場合はエラー
            if (cachedPrefab == null)
            {
                Debug.LogError($"Prefab {assetKey} not preloaded. Call PreloadAsync first.");
                throw new InvalidOperationException($"Prefab {assetKey} not preloaded");
            }

            // ZenjectのInstantiatePrefabForComponentを使用してDI対応インスタンス化
            var instance = container.InstantiatePrefabForComponent<TView>(
                cachedPrefab,
                canvas.transform);

            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate component {typeof(TView).Name} from {assetKey}");
                throw new InvalidOperationException($"Failed to instantiate {typeof(TView).Name}");
            }

            Debug.Log($"Successfully created view: {typeof(TView).Name} from {assetKey}");
            return instance;
        }

        /// <summary>
        /// 事前にプレハブをロードしてキャッシュ
        /// Installerの初期化時に呼び出すことを想定
        /// </summary>
        public async UniTask PreloadAsync()
        {
            if (cachedPrefab == null)
            {
                cachedPrefab = await assetProvider.LoadPrefabAsync(assetKey);
                Debug.Log($"Preloaded prefab: {assetKey}");
            }
        }

        /// <summary>
        /// キャッシュをクリアしてアセットをアンロード
        /// </summary>
        public void Unload()
        {
            if (cachedPrefab != null)
            {
                assetProvider.ReleaseAsset(cachedPrefab);
                cachedPrefab = null;
                Debug.Log($"Unloaded prefab: {assetKey}");
            }
        }
    }
}

