using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Application.Bootstrap.Steps
{
    /// <summary>
    /// Bootstrap自身を破棄するステップ
    /// </summary>
    public class UnloadBootstrapSceneStep : IBootstrapStep
    {
        private const string BOOTSTRAP_SCENE_NAME = "Bootstrap";

        public async UniTask ExecuteAsync()
        {
            Debug.Log("[Bootstrap] Unloading Bootstrap scene...");
            
            var bootstrapScene = SceneManager.GetSceneByName(BOOTSTRAP_SCENE_NAME);
            
            if (bootstrapScene.IsValid() && bootstrapScene.isLoaded)
            {
                var operation = SceneManager.UnloadSceneAsync(bootstrapScene);
                await operation;
                
                Debug.Log("[Bootstrap] Bootstrap scene unloaded");
            }
            else
            {
                Debug.LogWarning("[Bootstrap] Bootstrap scene not found or already unloaded");
            }
        }
    }
}
