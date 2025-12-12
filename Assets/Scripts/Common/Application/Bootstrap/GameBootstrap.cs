using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Common.Application.Bootstrap
{
    /// <summary>
    /// ゲーム起動時の初期化処理
    /// Bootstrapシーンに配置
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        private IBootstrapSequence bootstrapSequence;

        [Inject]
        public void Construct(IBootstrapSequence bootstrapSequence)
        {
            this.bootstrapSequence = bootstrapSequence;
        }

        private async void Start()
        {
            Debug.Log("[GameBootstrap] Game starting...");

            try
            {
                await bootstrapSequence.ExecuteAsync();
                Debug.Log("[GameBootstrap] Game initialization complete");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[GameBootstrap] Bootstrap failed: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
