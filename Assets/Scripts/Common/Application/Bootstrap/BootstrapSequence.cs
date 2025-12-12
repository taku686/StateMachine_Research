using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Common.Application.Bootstrap
{
    /// <summary>
    /// Bootstrap手順を順番に実行
    /// </summary>
    public class BootstrapSequence : IBootstrapSequence
    {
        private readonly IBootstrapStep[] steps;

        [Inject]
        public BootstrapSequence(IBootstrapStep[] steps)
        {
            this.steps = steps;
            Debug.Log($"[BootstrapSequence] Initialized with {steps.Length} steps");
        }

        public async UniTask ExecuteAsync()
        {
            Debug.Log("[BootstrapSequence] === Starting Bootstrap Sequence ===");

            foreach (var step in steps)
            {
                Debug.Log($"[BootstrapSequence] Executing step: {step.GetType().Name}");
                await step.ExecuteAsync();
            }

            Debug.Log("[BootstrapSequence] === Bootstrap Sequence Complete ===");
        }
    }
}
