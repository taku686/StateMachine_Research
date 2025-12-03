namespace OutGame.Application.Ports.Output
{
    /// <summary>
    /// 画面遷移結果を通知するインターフェース
    /// </summary>
    public interface INavigationOutputPort
    {
        /// <summary>
        /// 画面遷移開始時に呼ばれる
        /// </summary>
        void OnNavigationStarted(OutGameStateKey targetState);

        /// <summary>
        /// 画面遷移完了時に呼ばれる
        /// </summary>
        void OnNavigationCompleted();
    }
}

