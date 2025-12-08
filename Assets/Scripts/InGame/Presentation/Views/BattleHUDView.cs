using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGame.Presentation.Views
{
    /// <summary>
    /// バトルHUDのView
    /// </summary>
    public class BattleHUDView : MonoBehaviour
    {
        [Header("Wave情報")]
        [SerializeField] private TextMeshProUGUI waveText;

        [Header("プレイヤー情報")]
        [SerializeField] private HealthBarView playerHealthBar;
        [SerializeField] private TextMeshProUGUI playerHpText;

        [Header("報酬情報")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI expText;

        [Header("ボタン")]
        [SerializeField] private Button pauseButton;

        private void Awake()
        {
            // 必要に応じてコンポーネントを検索
            if (waveText == null)
            {
                waveText = transform.Find("WaveText")?.GetComponent<TextMeshProUGUI>();
            }

            if (goldText == null)
            {
                goldText = transform.Find("GoldText")?.GetComponent<TextMeshProUGUI>();
            }

            if (expText == null)
            {
                expText = transform.Find("ExpText")?.GetComponent<TextMeshProUGUI>();
            }
        }

        public void UpdateWaveInfo(int currentWave, int totalWaves)
        {
            if (waveText != null)
            {
                waveText.text = $"Wave {currentWave}/{totalWaves}";
            }
        }

        public void UpdatePlayerHp(int currentHp, int maxHp)
        {
            if (playerHealthBar != null)
            {
                playerHealthBar.SetValue(currentHp, maxHp);
            }

            if (playerHpText != null)
            {
                playerHpText.text = $"{currentHp}/{maxHp}";
            }
        }

        public void UpdateGold(int gold)
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {gold}";
            }
        }

        public void UpdateExp(int exp)
        {
            if (expText != null)
            {
                expText.text = $"Exp: {exp}";
            }
        }

        public Button GetPauseButton()
        {
            return pauseButton;
        }
    }
}

