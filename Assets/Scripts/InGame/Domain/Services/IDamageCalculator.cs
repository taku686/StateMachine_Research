namespace InGame.Domain.Services
{
    using InGame.Domain.Models;

    /// <summary>
    /// ダメージ計算サービスのインターフェース
    /// </summary>
    public interface IDamageCalculator
    {
        /// <summary>
        /// ダメージを計算
        /// </summary>
        int Calculate(Combatant attacker, Combatant defender);

        /// <summary>
        /// クリティカルヒット判定
        /// </summary>
        bool IsCriticalHit(Combatant attacker);
    }
}

