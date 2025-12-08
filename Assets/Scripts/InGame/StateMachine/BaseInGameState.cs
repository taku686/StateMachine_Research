using Cysharp.Threading.Tasks;

namespace InGame.StateMachine
{
    /// <summary>
    /// InGameステートの基底クラス
    /// </summary>
    public abstract class BaseInGameState : IInGameState
    {
        public virtual async UniTask OnEnter()
        {
            await UniTask.CompletedTask;
        }

        public virtual async UniTask OnExit()
        {
            await UniTask.CompletedTask;
        }

        public virtual void OnUpdate()
        {
            // Override in derived classes
        }
    }
}

