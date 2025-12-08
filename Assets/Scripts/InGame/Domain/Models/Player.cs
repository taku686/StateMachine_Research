using System.Collections.Generic;
using Zenject;

namespace InGame.Domain.Models
{
    /// <summary>
    /// プレイヤーエンティティ
    /// Zenject Factoryで生成
    /// </summary>
    public class Player : Combatant
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public List<Equipment> EquippedItems { get; private set; }

        // Zenject用デフォルトコンストラクタ
        public Player()
        {
            EquippedItems = new List<Equipment>();
        }

        /// <summary>
        /// 初期化メソッド（Factoryから呼ばれる）
        /// </summary>
        public void Initialize(string id, int level, CombatStats baseStats)
        {
            Id = id;
            Level = level;
            Stats = baseStats.Clone();
            Experience = 0;
        }

        /// <summary>
        /// 装備を追加
        /// </summary>
        public void EquipItem(Equipment equipment)
        {
            EquippedItems.Add(equipment);
            RecalculateStats();
        }

        /// <summary>
        /// 装備を解除
        /// </summary>
        public void UnequipItem(Equipment equipment)
        {
            if (EquippedItems.Remove(equipment))
            {
                RecalculateStats();
            }
        }

        /// <summary>
        /// 装備ボーナスを再計算
        /// </summary>
        private void RecalculateStats()
        {
            // 現在のHPの割合を保持
            float hpRatio = (float)Stats.CurrentHp / Stats.MaxHp;

            int bonusAttack = 0;
            int bonusDefense = 0;
            int bonusHp = 0;

            foreach (var equipment in EquippedItems)
            {
                bonusAttack += equipment.AttackBonus;
                bonusDefense += equipment.DefenseBonus;
                bonusHp += equipment.HpBonus;
            }

            // 新しいステータスを作成
            int newMaxHp = CalculateBaseMaxHp(Level) + bonusHp;
            int newCurrentHp = (int)(newMaxHp * hpRatio);

            Stats = new CombatStats(
                newMaxHp,
                newCurrentHp,
                CalculateBaseAttack(Level) + bonusAttack,
                CalculateBaseDefense(Level) + bonusDefense,
                Stats.AttackSpeed,
                Stats.MoveSpeed,
                Stats.CriticalRate
            );
        }

        /// <summary>
        /// 経験値を追加
        /// </summary>
        public void AddExperience(int exp)
        {
            Experience += exp;
            CheckLevelUp();
        }

        /// <summary>
        /// レベルアップチェック
        /// </summary>
        private void CheckLevelUp()
        {
            int requiredExp = CalculateRequiredExp(Level);

            while (Experience >= requiredExp)
            {
                Experience -= requiredExp;
                LevelUp();
                requiredExp = CalculateRequiredExp(Level);
            }
        }

        /// <summary>
        /// レベルアップ処理
        /// </summary>
        private void LevelUp()
        {
            Level++;

            // ステータス上昇
            int newMaxHp = CalculateBaseMaxHp(Level);
            Stats = new CombatStats(
                newMaxHp,
                newMaxHp,  // HPを全回復
                CalculateBaseAttack(Level),
                CalculateBaseDefense(Level),
                Stats.AttackSpeed,
                Stats.MoveSpeed,
                Stats.CriticalRate
            );

            // 装備ボーナスを再適用
            RecalculateStats();
        }

        /// <summary>
        /// レベルに必要な経験値を計算
        /// </summary>
        private int CalculateRequiredExp(int level)
        {
            return level * 100;
        }

        /// <summary>
        /// レベルによる基礎最大HPを計算
        /// </summary>
        private int CalculateBaseMaxHp(int level)
        {
            return 100 + (level * 10);
        }

        /// <summary>
        /// レベルによる基礎攻撃力を計算
        /// </summary>
        private int CalculateBaseAttack(int level)
        {
            return 10 + (level * 2);
        }

        /// <summary>
        /// レベルによる基礎防御力を計算
        /// </summary>
        private int CalculateBaseDefense(int level)
        {
            return 5 + level;
        }

        /// <summary>
        /// Zenject PlaceholderFactory定義
        /// </summary>
        public class Factory : PlaceholderFactory<string, int, CombatStats, Player>
        {
        }
    }

    /// <summary>
    /// 装備アイテム（簡易版）
    /// TODO: OutGameの装備システム実装後に置き換え
    /// </summary>
    public class Equipment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int HpBonus { get; set; }
    }
}

