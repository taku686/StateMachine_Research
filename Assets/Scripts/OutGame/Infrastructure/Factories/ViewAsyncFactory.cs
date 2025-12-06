using Cysharp.Threading.Tasks;
using OutGame.Infrastructure.Views;
using Zenject;

namespace OutGame.Infrastructure.Factories
{
    /// <summary>
    /// View作成用の非同期ファクトリー実装
    /// ZenjectのIFactory<TView>をラップして非同期インターフェースを提供
    /// 実際のインスタンス化はInstallerで設定されたファクトリーに委譲
    /// </summary>
    /// <typeparam name="TView">作成するViewの型</typeparam>
    public class ViewAsyncFactory<TView> : IViewFactory<TView> where TView : IView
    {
        private readonly IFactory<TView> syncFactory;

        [Inject]
        public ViewAsyncFactory(IFactory<TView> syncFactory)
        {
            this.syncFactory = syncFactory;
        }

        public async UniTask<TView> CreateAsync()
        {
            // ファクトリーでの生成は同期的
            // （Addressablesのロードは事前にキャッシュ済みの想定）
            await UniTask.Yield();
            return syncFactory.Create();
        }
    }
}

