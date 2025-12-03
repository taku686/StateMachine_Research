namespace OutGame.Application.Ports.Input
{
    /// <summary>
    /// SE音量を更新するUseCaseのインターフェース
    /// </summary>
    public interface IUpdateSeVolumeInputPort
    {
        void Execute(float volume);
    }
}

