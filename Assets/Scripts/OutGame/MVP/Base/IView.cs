using System;
using Cysharp.Threading.Tasks;

namespace OutGame.MVP
{
    /// <summary>
    /// View の基本インターフェース
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// View を表示
        /// </summary>
        UniTask Show();

        /// <summary>
        /// View を非表示
        /// </summary>
        UniTask Hide();

        /// <summary>
        /// View を破棄
        /// </summary>
        void Dispose();
    }
}

