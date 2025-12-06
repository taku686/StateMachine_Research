using System;
using Cysharp.Threading.Tasks;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートの基本インターフェース
    /// アセットプリロード/データロード/クリーンアップのライフサイクルを提供
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// アセットのプリロード（Addressables等）
        /// State遷移前に実行され、Viewプレハブなどをメモリにキャッシュ
        /// </summary>
        UniTask OnPreloadAssets();

        /// <summary>
        /// データのロード（Repository等）
        /// アセットプリロード後、OnEnter前に実行
        /// ユーザープロフィール、設定データなどを取得
        /// </summary>
        UniTask OnLoadData();

        /// <summary>
        /// ステートに入る時の処理
        /// アセットとデータは既にロード済み
        /// </summary>
        UniTask OnEnter();

        /// <summary>
        /// ステートから出る時の処理
        /// </summary>
        UniTask OnExit();

        /// <summary>
        /// アセットのクリーンアップ
        /// OnExit後に実行され、Addressablesアセットをアンロード
        /// </summary>
        UniTask OnCleanupAssets();

        /// <summary>
        /// データのクリーンアップ
        /// 通常は不要だが、キャッシュクリアなどが必要な場合に実装
        /// </summary>
        UniTask OnCleanupData();

        /// <summary>
        /// ステートの更新処理
        /// </summary>
        void OnUpdate();
    }
}

