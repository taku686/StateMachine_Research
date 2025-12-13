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
            // #region agent log
            System.IO.File.AppendAllText(@"c:\Users\takun\UnityProjects\StateMachine\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { location = "BootstrapInstaller.cs:30", message = "BEFORE FromMethod resolve", data = new { }, timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), sessionId = "debug-session", hypothesisId = "H3" }) + "\n");
            // #endregion
            _ = Container.Bind<IBootstrapStep[]>()
                .FromMethod(ctx => new IBootstrapStep[]
                {
                    // #region agent log
                    ctx.Container.Resolve<LoadTransitionSceneStep>(),
                    // #endregion
                    ctx.Container.Resolve<LoadFirstGameSceneStep>(),
                    ctx.Container.Resolve<UnloadBootstrapSceneStep>()
                })
                .AsSingle();
            // #region agent log
            System.IO.File.AppendAllText(@"c:\Users\takun\UnityProjects\StateMachine\.cursor\debug.log", Newtonsoft.Json.JsonConvert.SerializeObject(new { location = "BootstrapInstaller.cs:40", message = "AFTER FromMethod binding", data = new { }, timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), sessionId = "debug-session", hypothesisId = "H3" }) + "\n");
            // #endregion

            Container.Bind<IBootstrapSequence>()
                .To<BootstrapSequence>()
                .AsSingle();

            Debug.Log("[BootstrapInstaller] Bindings completed");
        }
    }
}
