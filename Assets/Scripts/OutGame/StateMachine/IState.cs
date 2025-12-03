using System;
using Cysharp.Threading.Tasks;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートの基本インターフェース
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// ステートに入る時の処理
        /// </summary>
        UniTask OnEnter();

        /// <summary>
        /// ステートから出る時の処理
        /// </summary>
        UniTask OnExit();

        /// <summary>
        /// ステートの更新処理
        /// </summary>
        void OnUpdate();
    }
}

