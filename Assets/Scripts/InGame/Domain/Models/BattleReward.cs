namespace InGame.Domain.Models
{
    /// <summary>
    /// バトル報酬（値オブジェクト）
    /// </summary>
    public class BattleReward
    {
        public int Gold { get; }
        public int Experience { get; }

        public BattleReward(int gold, int experience)
        {
            Gold = gold;
            Experience = experience;
        }

        /// <summary>
        /// 報酬を加算した新しいインスタンスを返す
        /// </summary>
        public BattleReward Add(BattleReward other)
        {
            return new BattleReward(
                Gold + other.Gold,
                Experience + other.Experience
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is BattleReward other)
            {
                return Gold == other.Gold && Experience == other.Experience;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Gold, Experience);
        }

        public override string ToString()
        {
            return $"Gold: {Gold}, Exp: {Experience}";
        }
    }
}

