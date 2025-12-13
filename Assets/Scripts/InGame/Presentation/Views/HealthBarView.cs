using InGame.Constants;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

namespace InGame.Presentation.Views
{
    /// <summary>
    /// HPバーのView
    /// </summary>
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Gradient colorGradient;

        [Header("設定")]
        [SerializeField] private float animationDuration = InGameConstants.UI.HealthBarAnimationDuration;
        [SerializeField] private bool useGradient = true;

        private void Awake()
        {
            // Sliderがない場合は自動で作成
            if (hpSlider == null)
            {
                hpSlider = GetComponentInChildren<Slider>();
            }

            if (fillImage == null && hpSlider != null)
            {
                fillImage = hpSlider.fillRect?.GetComponent<Image>();
            }

            // デフォルトグラデーション作成
            if (colorGradient == null || colorGradient.colorKeys.Length == 0)
            {
                colorGradient = new Gradient();
                var colorKeys = new GradientColorKey[3];
                colorKeys[0] = new GradientColorKey(Color.red, InGameConstants.UI.HealthBarGradientLowThreshold);
                colorKeys[1] = new GradientColorKey(Color.yellow, InGameConstants.UI.HealthBarGradientMidThreshold);
                colorKeys[2] = new GradientColorKey(Color.green, InGameConstants.UI.HealthBarGradientHighThreshold);

                var alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(1f, 1f);

                colorGradient.SetKeys(colorKeys, alphaKeys);
            }
        }

        public void SetValue(int currentHp, int maxHp)
        {
            if (hpSlider == null || maxHp <= 0)
            {
                return;
            }

            float targetValue = (float)currentHp / maxHp;

            // アニメーション付きで値を変更
            hpSlider.DOValue(targetValue, animationDuration)
                .SetEase(Ease.OutQuad);

            // グラデーション色を適用
            if (useGradient && fillImage != null)
            {
                fillImage.color = colorGradient.Evaluate(targetValue);
            }
        }

        public void SetValueImmediate(int currentHp, int maxHp)
        {
            if (hpSlider == null || maxHp <= 0)
            {
                return;
            }

            float value = (float)currentHp / maxHp;
            hpSlider.value = value;

            if (useGradient && fillImage != null)
            {
                fillImage.color = colorGradient.Evaluate(value);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Zenject PlaceholderFactory定義
        /// </summary>
        public class Factory : PlaceholderFactory<HealthBarView>
        {
        }
    }
}

