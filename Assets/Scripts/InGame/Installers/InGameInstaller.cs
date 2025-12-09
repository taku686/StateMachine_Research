using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace InGame.Installers
{
    using InGame.Domain.Models;
    using InGame.Domain.Services;
    using InGame.Domain.Repositories;
    using InGame.Infrastructure.Factories;
    using InGame.Infrastructure.Repositories;
    using InGame.Infrastructure.Data;
    using InGame.Application.Ports.Input;
    using InGame.Application.Ports.Output;
    using InGame.Application.UseCases;
    using InGame.Presentation.Views;
    using InGame.Presentation.Controllers;
    using InGame.Presentation.Presenters;
    using InGame.StateMachine;
    using InGame.States;

    /// <summary>
    /// InGameのZenject Installer
    /// </summary>
    public class InGameInstaller : MonoInstaller
    {
        [Header("View Prefabs")]
        [SerializeField] private PlayerView playerViewPrefab;
        [SerializeField] private EnemyView enemyViewPrefab;
        [SerializeField] private HealthBarView healthBarViewPrefab;
        [SerializeField] private BattleHUDView battleHUDViewPrefab;

        public override void InstallBindings()
        {
            Debug.Log("InGameInstaller: Installing bindings...");

            // ===== Domain Services =====
            Container.Bind<IDamageCalculator>()
                .To<DamageCalculator>()
                .AsSingle();

            Container.Bind<IRewardCalculator>()
                .To<RewardCalculator>()
                .AsSingle();

            // ===== Repositories =====
            Container.Bind<IPlayerRepository>()
                .To<PlayerRepositoryImpl>()
                .AsSingle();

            Container.Bind<IEnemyRepository>()
                .To<EnemyRepositoryImpl>()
                .AsSingle();

            Container.Bind<IEquipmentRepository>()
                .To<EquipmentRepositoryImpl>()
                .AsSingle();

            Container.Bind<IBattleConfigRepository>()
                .To<BattleConfigRepositoryImpl>()
                .AsSingle();

            // ===== Zenject Factories =====

            // Player Factory
            Container.BindFactory<string, int, CombatStats, Player, Player.Factory>()
                .FromNew();

            // Enemy Factory
            Container.BindFactory<string, EnemyType, CombatStats, BattleReward, Enemy, Enemy.Factory>()
                .FromNew();

            // Infrastructure Enemy Factory
            Container.Bind<IFactory<EnemyData, Enemy>>()
                .To<EnemyFactory>()
                .AsSingle();

            // EnemyViewController Factory
            Container.BindFactory<Enemy, EnemyView, HealthBarView, EnemyViewController, EnemyViewController.Factory>()
                .FromNew();

            // PlayerViewController Factory
            Container.BindFactory<Player, PlayerView, HealthBarView, PlayerViewController, PlayerViewController.Factory>()
                .FromNew();

            // ===== Use Cases =====
            Container.Bind<IStartBattleInputPort>()
                .To<StartBattleInteractor>()
                .AsSingle();

            Container.Bind<IAttackEnemyInputPort>()
                .To<ProcessCombatInteractor>()
                .AsSingle();

            Container.Bind<ICompleteBattleInputPort>()
                .To<CompleteBattleInteractor>()
                .AsSingle();

            // ===== Presenters =====
            Container.Bind<IBattleOutputPort>()
                .To<BattlePresenter>()
                .AsSingle();

            Container.Bind<IRewardOutputPort>()
                .To<BattlePresenter>()
                .FromResolve();  // 同じインスタンス

            Container.Bind<BattlePresenter>()
                .FromResolve();  // 直接参照用

            // ===== Views =====
            if (playerViewPrefab != null)
            {
                Container.Bind<PlayerView>()
                    .FromComponentInNewPrefab(playerViewPrefab)
                    .AsSingle();
            }

            if (battleHUDViewPrefab != null)
            {
                Container.Bind<BattleHUDView>()
                    .FromComponentInNewPrefab(battleHUDViewPrefab)
                    .AsSingle();
            }

            // HealthBarView (PlayerとEnemy用に複数作成可能)
            if (healthBarViewPrefab != null)
            {
                Container.BindFactory<HealthBarView, HealthBarView.Factory>()
                    .FromComponentInNewPrefab(healthBarViewPrefab);
            }

            // ===== Controllers =====
            Container.Bind<BattleHUDController>()
                .AsSingle();

            Container.Bind<List<EnemyViewController>>()
                .AsSingle();

            // ===== StateMachine =====
            Container.Bind<InGameStateMachine>()
                .AsSingle();

            // States
            Container.Bind<BattlePreparationState>().AsSingle();
            Container.Bind<BattleState>().AsSingle();
            Container.Bind<BattleResultState>().AsSingle();

            Debug.Log("InGameInstaller: Binding completed");
        }
    }
}

