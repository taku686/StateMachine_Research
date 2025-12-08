using System.Collections.Generic;

namespace InGame.Domain.Models
{
    /// <summary>
    /// Wave情報
    /// 敵の出現グループを表す
    /// </summary>
    public class Wave
    {
        public int WaveNumber { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public bool IsCleared => Enemies.TrueForAll(e => !e.IsAlive);

        public Wave(int waveNumber)
        {
            WaveNumber = waveNumber;
            Enemies = new List<Enemy>();
        }

        /// <summary>
        /// 敵を追加
        /// </summary>
        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        /// <summary>
        /// 生存している敵のリストを取得
        /// </summary>
        public List<Enemy> GetAliveEnemies()
        {
            return Enemies.FindAll(e => e.IsAlive);
        }
    }
}

