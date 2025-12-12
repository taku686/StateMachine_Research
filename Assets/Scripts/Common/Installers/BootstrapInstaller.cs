using Common.Application.Bootstrap;
using Common.Application.Bootstrap.Steps;
using UnityEngine;
using Zenject;

namespace Common.Installers
{
    /// <summary>
    /// BootstrapシーンのInstaller
    /// Bootstrap処理に必要なステップとシーケンスをバインド
    /// </summary>
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("[BootstrapInstaller] Installing bindings...");

            // ===== Bootstrap Steps =====
            Container.Bind<LoadTransitionSceneStep>()
                .AsSingle();

            Container.Bind<LoadFirstGameSceneStep>()
                .AsSingle();

            Container.Bind<UnloadBootstrapSceneStep>()
                .AsSingle();

            // ===== Bootstrap Sequence =====
            // ステップの配列を注入してBootstrapSequenceを作成
            Container.Bind<IBootstrapStep[]>()
                .FromMethod(ctx => new IBootstrapStep[]
                {
                    ctx.Container.Resolve<LoadTransitionSceneStep>(),
                    ctx.Container.Resolve<LoadFirstGameSceneStep>(),
                    ctx.Container.Resolve<UnloadBootstrapSceneStep>()
                })
                .AsSingle();

            Container.Bind<IBootstrapSequence>()
                .To<BootstrapSequence>()
                .AsSingle();

            Debug.Log("[BootstrapInstaller] Bindings completed");
        }
    }
}
