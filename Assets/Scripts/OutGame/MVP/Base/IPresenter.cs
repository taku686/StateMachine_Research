using System;
using Cysharp.Threading.Tasks;

namespace OutGame.MVP
{
    /// <summary>
    /// Presenter の基本インターフェース
    /// </summary>
    public interface IPresenter : IDisposable
    {
        /// <summary>
        /// 初期化処理
        /// </summary>
        UniTask Initialize();

        /// <summary>
        /// View を表示
        /// </summary>
        UniTask Show();

        /// <summary>
        /// View を非表示
        /// </summary>
        UniTask Hide();
    }
}

