using Cysharp.Threading.Tasks;

namespace Common.Domain.Services
{
    /// <summary>
    /// 遷移アニメーション（フェード・ローディング）を担当
    /// </summary>
    public interface ITransitionAnimator
    {
        /// <summary>
        /// フェードイン（画面を暗くする）
        /// </summary>
        UniTask FadeInAsync(float duration);

        /// <summary>
        /// フェードアウト（画面を明るくする）
        /// </summary>
        UniTask FadeOutAsync(float duration);

        /// <summary>
        /// ローディング要素を表示
        /// </summary>
        void ShowLoading();

        /// <summary>
        /// ローディング要素を非表示
        /// </summary>
        void HideLoading();

        /// <summary>
        /// プログレスバーの値を設定 (0.0 - 1.0)
        /// </summary>
        void SetProgress(float progress);
    }
}
