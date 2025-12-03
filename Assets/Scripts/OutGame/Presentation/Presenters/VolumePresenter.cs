using OutGame.Application.Ports.Output;
using OutGame.Domain.Models;
using OutGame.Infrastructure.Views;
using UnityEngine;
using Zenject;
using AudioSettings = OutGame.Domain.Models.AudioSettings;

namespace OutGame.Presentation.Presenters
{
    /// <summary>
    /// 音量更新結果を表示するPresenter
    /// Interactorからの結果を受け取り、Viewに表示を指示する
    /// </summary>
    public class VolumePresenter : IVolumeUpdateOutputPort
    {
        private SettingsView settingsView;

        [Inject]
        public VolumePresenter()
        {
        }

        /// <summary>
        /// Viewを設定する
        /// </summary>
        public void SetView(SettingsView view)
        {
            settingsView = view;
        }

        /// <summary>
        /// Interactorから呼ばれる：音量更新成功
        /// </summary>
        public void OnVolumeUpdated(AudioSettings settings)
        {
            if (settingsView == null) return;

            // ビジネスデータを表示用データに変換
            float bgmVolume = settings.BgmVolume;
            float seVolume = settings.SeVolume;

            // Viewに表示を指示
            settingsView.SetBgmVolume(bgmVolume);
            settingsView.SetSeVolume(seVolume);

            // デバッグログ
            Debug.Log($"音量を更新しました - BGM: {Mathf.RoundToInt(bgmVolume * 100)}%, SE: {Mathf.RoundToInt(seVolume * 100)}%");
        }

        /// <summary>
        /// Interactorから呼ばれる：音量更新失敗
        /// </summary>
        public void OnVolumeUpdateFailed(string errorMessage)
        {
            // エラーメッセージをログ出力
            Debug.LogError($"音量更新エラー: {errorMessage}");

            // TODO: エラートーストやダイアログを表示する場合はここで実装
        }
    }
}

