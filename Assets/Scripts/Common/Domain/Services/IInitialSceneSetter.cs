namespace Common.Domain.Services
{
    /// <summary>
    /// Bootstrap時の初期シーン設定専用インターフェース
    /// LoadingViewへの依存を持たない軽量なインターフェース
    /// </summary>
    public interface IInitialSceneSetter
    {
        /// <summary>
        /// 初期シーンを設定（Bootstrap時に使用）
        /// </summary>
        /// <param name="sceneName">初期シーン名</param>
        void SetInitialScene(string sceneName);
    }
}

