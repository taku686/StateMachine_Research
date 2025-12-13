using System;
using Common.Constants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using OutGame.Infrastructure.Factories;
using OutGame.Infrastructure.Views;
using Zenject;

namespace OutGame.StateMachine
{
    /// <summary>
    /// フェードによる画面遷移アニメーション
    /// LoadingViewを使用してフェードとローディングを制御
    /// Installer経由でファクトリーを注入（設計原則に準拠）
    /// </summary>
    public class FadeTransitionAnimator : IStateTransitionAnimator, IDisposable
    {
        private readonly IViewFactory<LoadingView> _loadingViewFactory;
        private readonly AddressableViewFactory<LoadingView> _addressableFactory;
        private readonly float _fadeDuration;

        private LoadingView _loadingView;
        private bool _isInitialized;

        [Inject]
        public FadeTransitionAnimator(
            IViewFactory<LoadingView> loadingViewFactory,
            AddressableViewFactory<LoadingView> addressableFactory,
            [InjectOptional] float fadeDuration = AppConstants.Animation.DefaultFadeDuration)
        {
            _loadingViewFactory = loadingViewFactory;
            _addressableFactory = addressableFactory;
            _fadeDuration = fadeDuration;
        }

        /// <summary>
        /// 初期化（ファクトリー経由でLoadingViewをインスタンス化）
        /// </summary>
        public async UniTask InitializeAsync()
        {
            if (_isInitialized) return;

            Debug.Log("[FadeTransitionAnimator] Initializing...");

            // プリロード（Installerで設定されたファクトリーを使用）
            await _addressableFactory.PreloadAsync();

            // インスタンス化（ファクトリー経由 - Canvasの子として作成される）
            _loadingView = await _loadingViewFactory.CreateAsync();

            if (_loadingView == null)
            {
                Debug.LogError("[FadeTransitionAnimator] Failed to create LoadingView!");
                return;
            }

            _isInitialized = true;
            Debug.Log("[FadeTransitionAnimator] Initialized!");
        }

        /// <summary>
        /// 遷移開始アニメーション（フェードアウト：画面を暗くする）
        /// 背景のみ表示
        /// </summary>
        public async UniTask PlayExitTransition()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            if (_loadingView == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            Debug.Log("[Transition] Playing exit transition (fade out)...");

            // ローディング要素は非表示（背景のみ）
            _loadingView.HideLoadingElements();

            // フェードイン（画面を暗く）
            await _loadingView.FadeIn(_fadeDuration);

            Debug.Log("[Transition] Exit transition complete!");
        }

        /// <summary>
        /// 遷移完了アニメーション（フェードイン：画面を明るくする）
        /// </summary>
        public async UniTask PlayEnterTransition()
        {
            if (_loadingView == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            Debug.Log("[Transition] Playing enter transition (fade in)...");

            // ローディング要素を非表示
            _loadingView.HideLoadingElements();

            // フェードアウト（画面を明るく）
            await _loadingView.FadeOut(_fadeDuration);

            Debug.Log("[Transition] Enter transition complete!");
        }

        /// <summary>
        /// ローディング表示（プログレスバーと回転画像を表示）
        /// </summary>
        public async UniTask ShowLoading()
        {
            if (!_isInitialized)
            {
                await InitializeAsync();
            }

            if (_loadingView != null)
            {
                Debug.Log("[Transition] Showing loading elements...");
                _loadingView.ShowLoadingElements();
            }
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// ローディング非表示
        /// </summary>
        public async UniTask HideLoading()
        {
            if (_loadingView != null)
            {
                Debug.Log("[Transition] Hiding loading elements...");
                _loadingView.HideLoadingElements();
            }
            await UniTask.CompletedTask;
        }

        /// <summary>
        /// プログレスバーの値を設定
        /// </summary>
        public void SetProgress(float progress)
        {
            _loadingView?.SetProgress(progress);
        }

        public void Dispose()
        {
            if (_loadingView != null)
            {
                _loadingView.Dispose();
                _loadingView = null;
            }

            _addressableFactory.Unload();
            _isInitialized = false;

            Debug.Log("[FadeTransitionAnimator] Disposed!");
        }
    }
}
