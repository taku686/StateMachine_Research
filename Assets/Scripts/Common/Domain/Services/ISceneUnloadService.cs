using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Common.Domain.Services
{
    /// <summary>
    /// シーン破棄とリソース管理を担当
    /// </summary>
    public interface ISceneUnloadService
    {
        /// <summary>
        /// シーンを破棄
        /// </summary>
        UniTask UnloadSceneAsync(Scene scene);

        /// <summary>
        /// 未使用のアセットを解放してGCを実行
        /// </summary>
        UniTask UnloadUnusedAssetsAsync();
    }
}
