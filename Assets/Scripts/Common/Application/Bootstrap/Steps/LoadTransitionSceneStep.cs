using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common.Application.Bootstrap.Steps
{
    /// <summary>
    /// Transitionシーンを Additive ロードするステップ
    /// </summary>
    public class LoadTransitionSceneStep : IBootstrapStep
    {
        private const string TRANSITION_SCENE_NAME = "Transition";

        public async UniTask ExecuteAsync()
        {
            Debug.Log($"[Bootstrap] Loading {TRANSITION_SCENE_NAME} scene...");
            
            var operation = SceneManager.LoadSceneAsync(TRANSITION_SCENE_NAME, LoadSceneMode.Additive);
            await operation;
            
            Debug.Log($"[Bootstrap] {TRANSITION_SCENE_NAME} scene loaded");
        }
    }
}
