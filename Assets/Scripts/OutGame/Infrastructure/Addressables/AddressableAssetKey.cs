namespace OutGame.Infrastructure.Addressables
{
    /// <summary>
    /// Addressablesアセットキーの定数管理
    /// タイポ防止と型安全性を提供
    /// </summary>
    public static class AddressableAssetKey
    {
        /// <summary>
        /// View系のアセットキー
        /// </summary>
        public static class Views
        {
            public const string Title = "TitleView";
            public const string Home = "HomeView";
            public const string Settings = "SettingsView";
        }

        // 将来的な拡張用
        // public static class Prefabs { }
        // public static class Audio { }
        // public static class Effects { }
    }
}

