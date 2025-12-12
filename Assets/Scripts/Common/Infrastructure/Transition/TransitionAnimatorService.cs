using Common.Domain.Services;
using OutGame.Infrastructure.Views;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Common.Infrastructure.Transition
{
    /// <summary>
    /// フェード・ローディング表示のみを担当
    /// LoadingViewへの委譲
    /// </summary>
    public class TransitionAnimatorService : ITransitionAnimator
    {
        private readonly LoadingView loadingView;
        private readonly float defaultFadeDuration;

        public class Settings
        {
            public float DefaultFadeDuration = 0.3f;
        }

        [Inject]
        public TransitionAnimatorService(
            LoadingView loadingView,
            [InjectOptional] Settings settings = null)
        {
            this.loadingView = loadingView;
            this.defaultFadeDuration = settings?.DefaultFadeDuration ?? 0.3f;
        }

        public async UniTask FadeInAsync(float duration)
        {
            await loadingView.Show();
            await loadingView.FadeIn(duration);
        }

        public async UniTask FadeOutAsync(float duration)
        {
            await loadingView.FadeOut(duration);
            await loadingView.Hide();
        }

        public void ShowLoading()
        {
            loadingView.ShowLoadingElements();
        }

        public void HideLoading()
        {
            loadingView.HideLoadingElements();
        }

        public void SetProgress(float progress)
        {
            loadingView.SetProgress(progress);
        }
    }
}
