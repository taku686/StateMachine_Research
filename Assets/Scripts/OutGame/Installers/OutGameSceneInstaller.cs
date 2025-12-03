using UnityEngine;
using Zenject;

namespace OutGame.Installers
{
    /// <summary>
    /// アウトゲームシーン用のZenject Scene Installer
    /// より柔軟な設定が必要な場合はこちらを使用
    /// </summary>
    public class OutGameSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // OutGameManager をシーンに配置済みの場合
            Container.Bind<OutGameManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            // 必要に応じて他のバインディングを追加
        }
    }
}

