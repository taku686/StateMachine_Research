using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace OutGame.Infrastructure.Views
{
    /// <summary>
    /// View の基底クラス
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.3f;

        protected virtual void Awake()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);
            if (_canvasGroup != null)
            {
                await _canvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true);
            }
        }

        public virtual async UniTask Hide()
        {
            if (_canvasGroup != null)
            {
                await _canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true);
            }
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}

