using UnityEngine;

namespace InGame.Infrastructure.Data
{
    using InGame.Domain.Models;

    /// <summary>
    /// 敵のマスターデータ（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "InGame/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("基本情報")]
        public string enemyId;
        public string enemyName;
        public EnemyType enemyType = EnemyType.Normal;
        public Sprite sprite;

        [Header("ステータス")]
        public int maxHp = 100;
        public int attackPower = 10;
        public int defense = 5;
        public float attackSpeed = 2f;  // 秒
        public float moveSpeed = 0f;    // 0なら動かない

        [Header("報酬")]
        public int goldReward = 10;
        public int expReward = 5;

        [Header("見た目")]
        public float scale = 1f;
        public Color tintColor = Color.white;
    }
}

