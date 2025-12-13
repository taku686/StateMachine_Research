using Cysharp.Threading.Tasks;
using InGame.Constants;
using Zenject;

namespace InGame.Infrastructure.Repositories
{
    using InGame.Domain.Models;
    using InGame.Domain.Repositories;
    using OutGame.Domain.Repositories;

    /// <summary>
    /// プレイヤーリポジトリ実装
    /// OutGameのUserProfileと連携
    /// </summary>
    public class PlayerRepositoryImpl : IPlayerRepository
    {
        private readonly IUserProfileRepository userProfileRepository;
        private readonly Player.Factory playerFactory;
        private readonly IEquipmentRepository equipmentRepository;

        [Inject]
        public PlayerRepositoryImpl(
            IUserProfileRepository userProfileRepository,
            Player.Factory playerFactory,
            IEquipmentRepository equipmentRepository)
        {
            this.userProfileRepository = userProfileRepository;
            this.playerFactory = playerFactory;
            this.equipmentRepository = equipmentRepository;
        }

        public async UniTask<Player> GetPlayer()
        {
            // OutGameからプレイヤーデータ取得
            var profile = userProfileRepository.Load();

            // ベースステータス計算
            var baseStats = CalculateBaseStats(profile.Level);

            // Zenject Factoryでプレイヤー生成
            var player = playerFactory.Create(
                profile.UserId ?? "Player_1",
                profile.Level,
                baseStats
            );

            // 装備データ取得と反映
            var equippedItems = await equipmentRepository.GetEquippedItems();
            ApplyEquipment(player, equippedItems);

            return player;
        }

        public async UniTask SavePlayer(Player player)
        {
            // TODO: プレイヤーデータをOutGameに保存
            // 現在はレベル情報のみ保存（経験値は別途管理）
            await UniTask.CompletedTask;
        }

        private CombatStats CalculateBaseStats(int level)
        {
            return new CombatStats(
                maxHp: InGameConstants.Player.BaseHp + (level * InGameConstants.Player.HpPerLevel),
                currentHp: InGameConstants.Player.BaseHp + (level * InGameConstants.Player.HpPerLevel),
                attackPower: InGameConstants.Player.BaseAttackPower + (level * InGameConstants.Player.AttackPowerPerLevel),
                defense: InGameConstants.Player.BaseDefense + level * InGameConstants.Player.DefensePerLevel,
                attackSpeed: InGameConstants.Player.DefaultAttackSpeed,
                moveSpeed: InGameConstants.Player.DefaultMoveSpeed,
                criticalRate: InGameConstants.Combat.DefaultCriticalRate
            );
        }

        private void ApplyEquipment(Player player, System.Collections.Generic.List<Equipment> equipment)
        {
            foreach (var item in equipment)
            {
                player.EquipItem(item);
            }
        }
    }
}

