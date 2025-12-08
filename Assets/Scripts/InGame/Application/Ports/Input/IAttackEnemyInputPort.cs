using Cysharp.Threading.Tasks;

namespace InGame.Application.Ports.Input
{
    using InGame.Domain.Models;

    /// <summary>
    /// 敵攻撃ユースケースの入力ポート
    /// </summary>
    public interface IAttackEnemyInputPort
    {
        UniTask Execute(Combatant attacker, Combatant defender);
    }
}

