using Cysharp.Threading.Tasks;

namespace Common.Application.Bootstrap
{
    /// <summary>
    /// Bootstrap処理の手順定義
    /// </summary>
    public interface IBootstrapSequence
    {
        UniTask ExecuteAsync();
    }
}
