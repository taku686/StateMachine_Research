using Cysharp.Threading.Tasks;

namespace OutGame.StateMachine
{
    /// <summary>
    /// State遷移時のアニメーションを担当するインターフェース
    /// </summary>
    public interface IStateTransitionAnimator
    {
        /// <summary>
        /// 遷移開始時のアニメーション（フェードアウト等）
        /// 現StateのOnExitの前に実行
        /// </summary>
        UniTask PlayExitTransition();

        /// <summary>
        /// 遷移完了時のアニメーション（フェードイン等）
        /// 新StateのOnEnterの後に実行
        /// </summary>
        UniTask PlayEnterTransition();

        /// <summary>
        /// ローディング表示を開始
        /// </summary>
        UniTask ShowLoading();

        /// <summary>
        /// ローディング表示を終了
        /// </summary>
        UniTask HideLoading();
    }
}

