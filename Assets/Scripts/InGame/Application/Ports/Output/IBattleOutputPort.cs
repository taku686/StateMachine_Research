using Cysharp.Threading.Tasks;

namespace InGame.Application.Ports.Output
{
    using InGame.Domain.Models;

    /// <summary>
    /// バトルイベントの出力ポート（Presenter実装）
    /// </summary>
    public interface IBattleOutputPort
    {
        /// <summary>
        /// バトル開始時
        /// </summary>
        UniTask OnBattleStarted(BattleContext context);

        /// <summary>
        /// Wave開始時
        /// </summary>
        UniTask OnWaveStarted(Wave wave);

        /// <summary>
        /// ダメージ発生時
        /// </summary>
        UniTask OnDamageDealt(Combatant attacker, Combatant defender, int damage, bool isCritical);

        /// <summary>
        /// 戦闘参加者撃破時
        /// </summary>
        UniTask OnCombatantDefeated(Combatant defeated);

        /// <summary>
        /// Wave完了時
        /// </summary>
        UniTask OnWaveCompleted(Wave wave, BattleReward reward);

        /// <summary>
        /// バトル完了時
        /// </summary>
        UniTask OnBattleCompleted(BattleContext context);
    }
}

