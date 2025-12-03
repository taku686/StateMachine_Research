using OutGame.Domain.Models;

namespace OutGame.Application.Ports.Output
{
    /// <summary>
    /// 音量更新結果を通知するインターフェース
    /// </summary>
    public interface IVolumeUpdateOutputPort
    {
        /// <summary>
        /// 音量更新成功時に呼ばれる
        /// </summary>
        void OnVolumeUpdated(AudioSettings settings);

        /// <summary>
        /// 音量更新失敗時に呼ばれる
        /// </summary>
        void OnVolumeUpdateFailed(string errorMessage);
    }
}

