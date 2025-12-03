using OutGame.Application.Ports.Output;
using UnityEngine;
using Zenject;

namespace OutGame.Presentation.Presenters
{
    /// <summary>
    /// 画面遷移結果を表示するPresenter
    /// Interactorからの結果を受け取り、必要に応じてローディング表示などを行う
    /// </summary>
    public class NavigationPresenter : INavigationOutputPort
    {
        [Inject]
        public NavigationPresenter()
        {
        }

        /// <summary>
        /// Interactorから呼ばれる：画面遷移開始
        /// </summary>
        public void OnNavigationStarted(OutGameStateKey targetState)
        {
            // TODO: ローディング画面を表示する場合はここで実装
            Debug.Log($"画面遷移開始: {targetState}");
        }

        /// <summary>
        /// Interactorから呼ばれる：画面遷移完了
        /// </summary>
        public void OnNavigationCompleted()
        {
            // TODO: ローディング画面を非表示にする場合はここで実装
            Debug.Log("画面遷移完了");
        }
    }
}

