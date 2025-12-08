using Cysharp.Threading.Tasks;

namespace InGame.StateMachine
{
    /// <summary>
    /// InGameステートのインターフェース
    /// </summary>
    public interface IInGameState
    {
        UniTask OnEnter();
        UniTask OnExit();
        void OnUpdate();
    }
}

