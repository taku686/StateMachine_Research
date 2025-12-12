using Cysharp.Threading.Tasks;

namespace Common.Application.Bootstrap
{
    /// <summary>
    /// Bootstrap手順の各ステップの共通インターフェース
    /// </summary>
    public interface IBootstrapStep
    {
        /// <summary>
        /// ステップを実行
        /// </summary>
        UniTask ExecuteAsync();
    }
}
