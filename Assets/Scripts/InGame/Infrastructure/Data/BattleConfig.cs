using UnityEngine;
using System.Collections.Generic;

namespace InGame.Infrastructure.Data
{
    /// <summary>
    /// ステージのバトル設定（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "BattleConfig", menuName = "InGame/Battle Config")]
    public class BattleConfig : ScriptableObject
    {
        [Header("ステージ情報")]
        public string stageId;
        public string stageName;

        [Header("Wave設定")]
        public int totalWaves = 3;
        public int enemiesPerWave = 3;

        [Header("難易度")]
        [Range(0.5f, 3f)]
        public float difficultyMultiplier = 1f;

        [Header("敵の出現パターン")]
        public List<WaveEnemyPattern> wavePatterns = new List<WaveEnemyPattern>();
    }

    [System.Serializable]
    public class WaveEnemyPattern
    {
        public int waveNumber;
        public List<EnemyData> enemies = new List<EnemyData>();
    }
}

