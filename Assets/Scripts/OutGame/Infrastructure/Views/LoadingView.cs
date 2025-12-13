using System.Threading;
using Common.Constants;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace OutGame.Infrastructure.Views
{
    /// <summary>
    /// ローディング＆フェード画面のView
    /// フェード時は背景のみ、ローディング時はプログレスバーと回転画像も表示
    /// </summary>
    public class LoadingView : MonoBehaviour, IView
    {
        [Header("フェード用")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _background;

        [Header("ローディング用")]
        [SerializeField] private GameObject _loadingElements;
        [SerializeField] private Slider _progressBar;

        [Header("スピナー（フレームアニメーション）")]
        [SerializeField] private Image _spinnerImage;
        [SerializeField] private Sprite[] _spinnerFrames;
        [SerializeField] private float _frameRate = AppConstants.Animation.DefaultSpinnerFrameRate; // フレーム/秒

        private bool _isSpinning;
        private CancellationTokenSource _spinnerCts;

        private void Awake()
        {
            // 初期状態：非表示
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }

            // ローディング要素を非表示
            HideLoadingElements();
        }

        #region IView Implementation

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            await UniTask.CompletedTask;
        }

        public async UniTask Hide()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }

        public void Dispose()
        {
            StopSpinner();
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Fade Methods

        /// <summary>
        /// フェードイン（画面を暗くする）
        /// </summary>
        public async UniTask FadeIn(float duration)
        {
            if (_canvasGroup == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            _canvasGroup.blocksRaycasts = true;
            await _canvasGroup.DOFade(1f, duration).SetUpdate(true).ToUniTask();
        }

        /// <summary>
        /// フェードアウト（画面を明るくする）
        /// </summary>
        public async UniTask FadeOut(float duration)
        {
            if (_canvasGroup == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            await _canvasGroup.DOFade(0f, duration).SetUpdate(true).ToUniTask();
            _canvasGroup.blocksRaycasts = false;
        }

        #endregion

        #region Loading Methods

        /// <summary>
        /// ローディング要素を表示（プログレスバー、回転画像）
        /// </summary>
        public void ShowLoadingElements()
        {
            if (_loadingElements != null)
            {
                _loadingElements.SetActive(true);
            }

            // プログレスバーを初期化
            if (_progressBar != null)
            {
                _progressBar.value = AppConstants.Progress.Min;
            }

            // スピナー回転開始
            StartSpinner();
        }

        /// <summary>
        /// ローディング要素を非表示
        /// </summary>
        public void HideLoadingElements()
        {
            if (_loadingElements != null)
            {
                _loadingElements.SetActive(false);
            }

            // スピナー回転停止
            StopSpinner();
        }

        /// <summary>
        /// プログレスバーの値を設定 (0.0 - 1.0)
        /// </summary>
        public void SetProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.value = Mathf.Clamp01(progress);
            }
        }

        #endregion

        #region Spinner

        private void StartSpinner()
        {
            if (_spinnerImage == null || _spinnerFrames == null || _spinnerFrames.Length == 0 || _isSpinning)
                return;

            _isSpinning = true;
            _spinnerCts = new CancellationTokenSource();
            SpinnerAnimationLoop(_spinnerCts.Token).Forget();
        }

        private async UniTaskVoid SpinnerAnimationLoop(CancellationToken ct)
        {
            int frameIndex = 0;
            float frameInterval = 1f / _frameRate;

            while (!ct.IsCancellationRequested)
            {
                if (_spinnerImage != null && _spinnerFrames.Length > 0)
                {
                    _spinnerImage.sprite = _spinnerFrames[frameIndex];
                    frameIndex = (frameIndex + 1) % _spinnerFrames.Length;
                }

                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(frameInterval),
                    ignoreTimeScale: true,
                    cancellationToken: ct
                );
            }
        }

        private void StopSpinner()
        {
            if (!_isSpinning) return;

            _spinnerCts?.Cancel();
            _spinnerCts?.Dispose();
            _spinnerCts = null;
            _isSpinning = false;

            // 最初のフレームに戻す
            if (_spinnerImage != null && _spinnerFrames != null && _spinnerFrames.Length > 0)
            {
                _spinnerImage.sprite = _spinnerFrames[0];
            }
        }

        #endregion

        private void OnDestroy()
        {
            StopSpinner();
        }
    }
}

