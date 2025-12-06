using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace OutGame.StateMachine
{
    /// <summary>
    /// フェードによる画面遷移アニメーション
    /// </summary>
    public class FadeTransitionAnimator : MonoBehaviour, IStateTransitionAnimator
    {
        [SerializeField] private CanvasGroup _fadeCanvasGroup;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private float _fadeDuration = 0.3f;

        private void Awake()
        {
            if (_fadeCanvasGroup != null)
            {
                _fadeCanvasGroup.alpha = 0f;
                _fadeCanvasGroup.blocksRaycasts = false;
            }

            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(false);
            }
        }

        public async UniTask PlayExitTransition()
        {
            if (_fadeCanvasGroup == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            Debug.Log("[Transition] Playing exit transition (fade out)...");
            
            _fadeCanvasGroup.blocksRaycasts = true;
            await _fadeCanvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true).ToUniTask();
            
            Debug.Log("[Transition] Exit transition complete!");
        }

        public async UniTask PlayEnterTransition()
        {
            if (_fadeCanvasGroup == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            Debug.Log("[Transition] Playing enter transition (fade in)...");
            
            await _fadeCanvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).ToUniTask();
            _fadeCanvasGroup.blocksRaycasts = false;
            
            Debug.Log("[Transition] Enter transition complete!");
        }

        public async UniTask ShowLoading()
        {
            if (_loadingPanel != null)
            {
                Debug.Log("[Transition] Showing loading...");
                _loadingPanel.SetActive(true);
            }
            await UniTask.CompletedTask;
        }

        public async UniTask HideLoading()
        {
            if (_loadingPanel != null)
            {
                Debug.Log("[Transition] Hiding loading...");
                _loadingPanel.SetActive(false);
            }
            await UniTask.CompletedTask;
        }
    }
}

