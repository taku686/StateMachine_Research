using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGame.Infrastructure.Factories;
using UnityEngine;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートの基底クラス
    /// アセットのプリロード/クリーンアップを自動的に処理
    /// データロードは派生クラスで必要に応じて実装
    /// </summary>
    public abstract class BaseState : IState
    {
        // 各StateでAddressableViewFactoryを登録するためのリスト
        private readonly List<IAssetPreloadable> preloadableAssets = new();

        /// <summary>
        /// プリロード可能なアセット（ファクトリー）を登録
        /// 派生クラスのコンストラクタで呼び出す
        /// </summary>
        /// <param name="asset">プリロード対象のアセット</param>
        protected void RegisterPreloadableAsset(IAssetPreloadable asset)
        {
            if (asset != null && !preloadableAssets.Contains(asset))
            {
                preloadableAssets.Add(asset);
            }
        }

        // ========== アセット関連（基底クラスで自動処理） ==========

        /// <summary>
        /// アセットのプリロード（基底クラスで自動処理）
        /// 登録された全アセットをプリロード + 派生クラス固有の処理
        /// </summary>
        public async UniTask OnPreloadAssets()
        {
            Debug.Log($"[{GetType().Name}] Preloading assets...");
            
            // 登録された全アセットをプリロード
            foreach (var asset in preloadableAssets)
            {
                await asset.PreloadAsync();
            }

            // 派生クラス固有のアセットプリロード処理
            await OnPreloadAssetsAsync();
            
            Debug.Log($"[{GetType().Name}] Assets preloaded!");
        }

        /// <summary>
        /// アセットのクリーンアップ（基底クラスで自動処理）
        /// 派生クラス固有の処理 + 登録された全アセットをアンロード
        /// </summary>
        public async UniTask OnCleanupAssets()
        {
            Debug.Log($"[{GetType().Name}] Cleaning up assets...");
            
            // 派生クラス固有のアセットクリーンアップ処理
            await OnCleanupAssetsAsync();

            // 登録された全アセットをアンロード
            foreach (var asset in preloadableAssets)
            {
                asset.Unload();
            }
            
            Debug.Log($"[{GetType().Name}] Assets cleaned up!");
        }

        /// <summary>
        /// 派生クラス固有のアセットプリロード処理
        /// 特別な処理が必要な場合のみオーバーライド
        /// </summary>
        protected virtual async UniTask OnPreloadAssetsAsync()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 派生クラス固有のアセットクリーンアップ処理
        /// 特別な処理が必要な場合のみオーバーライド
        /// </summary>
        protected virtual async UniTask OnCleanupAssetsAsync()
        {
            await UniTask.CompletedTask;
        }

        // ========== データ関連（派生クラスで実装） ==========

        /// <summary>
        /// データのロード
        /// 派生クラスで必要に応じてOnLoadDataAsyncをオーバーライド
        /// </summary>
        public async UniTask OnLoadData()
        {
            Debug.Log($"[{GetType().Name}] Loading data...");
            
            await OnLoadDataAsync();
            
            Debug.Log($"[{GetType().Name}] Data loaded!");
        }

        /// <summary>
        /// データのクリーンアップ
        /// 派生クラスで必要に応じてOnCleanupDataAsyncをオーバーライド
        /// </summary>
        public async UniTask OnCleanupData()
        {
            Debug.Log($"[{GetType().Name}] Cleaning up data...");
            
            await OnCleanupDataAsync();
            
            Debug.Log($"[{GetType().Name}] Data cleaned up!");
        }

        /// <summary>
        /// 派生クラスでデータロード処理を実装
        /// 例：Repository.Load(), API呼び出しなど
        /// </summary>
        protected virtual async UniTask OnLoadDataAsync()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// 派生クラスでデータクリーンアップ処理を実装
        /// 通常は不要だが、キャッシュクリアなどが必要な場合に使用
        /// </summary>
        protected virtual async UniTask OnCleanupDataAsync()
        {
            await UniTask.CompletedTask;
        }

        // ========== State基本処理 ==========

        /// <summary>
        /// ステートに入る時の処理
        /// アセットとデータは既にロード済み
        /// </summary>
        public virtual async UniTask OnEnter()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// ステートから出る時の処理
        /// </summary>
        public virtual async UniTask OnExit()
        {
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// ステートの更新処理
        /// </summary>
        public virtual void OnUpdate()
        {
        }
    }
}

