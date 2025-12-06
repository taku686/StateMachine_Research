using Cysharp.Threading.Tasks;
using OutGame.Infrastructure.Views;

namespace OutGame.Infrastructure.Factories
{
    /// <summary>
    /// View作成用の非同期ファクトリーインターフェース
    /// 型安全性を提供し、各View専用のファクトリーを定義
    /// </summary>
    /// <typeparam name="TView">作成するViewの型</typeparam>
    public interface IViewFactory<TView> where TView : IView
    {
        /// <summary>
        /// Viewを非同期で作成
        /// 実際のインスタンス化処理はInstallerで設定される
        /// </summary>
        /// <returns>作成されたViewインスタンス</returns>
        UniTask<TView> CreateAsync();
    }
}

