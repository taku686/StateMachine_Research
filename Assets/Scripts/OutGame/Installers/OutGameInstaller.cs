using UnityEngine;
using Zenject;

namespace OutGame.Installers
{
    /// <summary>
    /// アウトゲーム用のZenject Installer
    /// </summary>
    public class OutGameInstaller : MonoInstaller
    {
        [SerializeField] private OutGameManager _outGameManagerPrefab;

        public override void InstallBindings()
        {
            // OutGameManager をバインド
            Container.Bind<OutGameManager>()
                .FromComponentInNewPrefab(_outGameManagerPrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}

