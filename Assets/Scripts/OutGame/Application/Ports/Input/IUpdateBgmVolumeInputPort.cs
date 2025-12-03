namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// BGM音量を更新するUseCaseのインターフェース
    /// </summary>
    public interface IUpdateBgmVolumeInputPort
    {
        void Execute(float volume);
    }
}

