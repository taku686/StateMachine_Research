using Common.Domain.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Infrastructure.Scene
{
    /// <summary>
    /// シーン破棄とリソース管理のみを担当
    /// </summary>
    public class SceneUnloadService : ISceneUnloadService
    {
        public async UniTask UnloadSceneAsync(UnityEngine.SceneManagement.Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogWarning($"[SceneUnloadService] Scene is not valid or not loaded: {scene.name}");
                return;
            }

            Debug.Log($"[SceneUnloadService] Unloading scene: {scene.name}");
            
            var operation = SceneManager.UnloadSceneAsync(scene);
            
            if (operation == null)
            {
                Debug.LogError($"[SceneUnloadService] Failed to start unloading scene: {scene.name}");
                return;
            }

            await operation;
            
            Debug.Log($"[SceneUnloadService] Scene unloaded: {scene.name}");
        }

        public async UniTask UnloadUnusedAssetsAsync()
        {
            Debug.Log("[SceneUnloadService] Unloading unused assets...");
            
            await Resources.UnloadUnusedAssets();
            
            // GC実行
            System.GC.Collect();
            
            Debug.Log("[SceneUnloadService] Unused assets unloaded and GC executed");
        }
    }
}
