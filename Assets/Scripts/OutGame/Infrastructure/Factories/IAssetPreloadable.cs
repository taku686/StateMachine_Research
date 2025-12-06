using Cysharp.Threading.Tasks;

namespace OutGame.Infrastructure.Factories
{
    /// <summary>
    /// プリロード/アンロード可能なアセットのインターフェース
    /// AddressableViewFactoryなどが実装
    /// BaseStateで自動的にプリロード/アンロード処理を行うために使用
    /// </summary>
    public interface IAssetPreloadable
    {
        /// <summary>
        /// アセットを事前にロード
        /// </summary>
        UniTask PreloadAsync();

        /// <summary>
        /// アセットをアンロード
        /// </summary>
        void Unload();
    }
}

