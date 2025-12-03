using Cysharp.Threading.Tasks;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートの基底クラス
    /// </summary>
    public abstract class BaseState : IState
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
        }
    }
}

