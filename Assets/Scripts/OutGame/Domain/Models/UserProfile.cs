namespace OutGame.Domain.Models
{
    /// <summary>
    /// ユーザープロフィールのエンティティ
    /// </summary>
    public class UserProfile
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public int Level { get; private set; }
        public int Gold { get; private set; }

        public UserProfile(string userId, string userName, int level = 1, int gold = 1000)
        {
            UserId = userId;
            UserName = userName;
            Level = level;
            Gold = gold;
        }

        /// <summary>
        /// レベルを設定
        /// </summary>
        public void SetLevel(int level)
        {
            if (level > 0)
            {
                Level = level;
            }
        }

        /// <summary>
        /// ゴールドを設定
        /// </summary>
        public void SetGold(int gold)
        {
            if (gold >= 0)
            {
                Gold = gold;
            }
        }

        /// <summary>
        /// ゴールドを追加
        /// </summary>
        public void AddGold(int amount)
        {
            if (amount > 0)
            {
                Gold += amount;
            }
        }

        /// <summary>
        /// ゴールドを消費（ビジネスルール：足りない場合はfalse）
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            return true;
        }
    }
}

