using OutGame.Application.Ports.Input;
using OutGame.Application.Ports.Output;
using OutGame.Domain.Repositories;
using Zenject;

namespace OutGame.Application.UseCases
{
    /// <summary>
    /// BGM音量を更新するInteractor
    /// ビジネスロジックの実装
    /// </summary>
    public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
    {
        private readonly IAudioSettingsRepository audioSettingsRepository;
        private readonly IVolumeUpdateOutputPort outputPort;

        [Inject]
        public UpdateBgmVolumeInteractor(
            IAudioSettingsRepository audioSettingsRepository,
            IVolumeUpdateOutputPort outputPort)
        {
            this.audioSettingsRepository = audioSettingsRepository;
            this.outputPort = outputPort;
        }

        public void Execute(float volume)
        {
            // 1. 現在の設定を取得
            var currentSettings = audioSettingsRepository.Load();

            // 2. ビジネスルール：音量の妥当性チェック
            if (!currentSettings.IsValidVolume(volume))
            {
                outputPort.OnVolumeUpdateFailed("音量は0～1の範囲で指定してください");
                return;
            }

            // 3. エンティティを更新
            currentSettings.UpdateBgmVolume(volume);

            // 4. 初回調整フラグを立てる
            if (!currentSettings.HasAdjustedVolumeOnce)
            {
                currentSettings.MarkVolumeAdjusted();
            }

            // 5. 変更を永続化
            audioSettingsRepository.Save(currentSettings);

            // 6. 結果をPresenterに通知
            outputPort.OnVolumeUpdated(currentSettings);
        }
    }
}

