# 新規作成ファイルチェックリスト

クリーンアーキテクチャへのリファクタリングで作成した新しいファイルの一覧です。
Unityエディタでこれらのファイルが正しく認識されているか確認してください。

## Entity層（Domain）

### Models
- [ ] `Assets/Scripts/OutGame/Domain/Models/AudioSettings.cs`
- [ ] `Assets/Scripts/OutGame/Domain/Models/UserProfile.cs`

### Repositories（インターフェース）
- [ ] `Assets/Scripts/OutGame/Domain/Repositories/IAudioSettingsRepository.cs`
- [ ] `Assets/Scripts/OutGame/Domain/Repositories/IUserProfileRepository.cs`

## UseCase層（Application）

### Ports/Input
- [ ] `Assets/Scripts/OutGame/Application/Ports/Input/IUpdateBgmVolumeInputPort.cs`
- [ ] `Assets/Scripts/OutGame/Application/Ports/Input/IUpdateSeVolumeInputPort.cs`
- [ ] `Assets/Scripts/OutGame/Application/Ports/Input/INavigateToHomeInputPort.cs`
- [ ] `Assets/Scripts/OutGame/Application/Ports/Input/IOpenSettingsInputPort.cs`
- [ ] `Assets/Scripts/OutGame/Application/Ports/Input/INavigateBackInputPort.cs`

### Ports/Output
- [ ] `Assets/Scripts/OutGame/Application/Ports/Output/IVolumeUpdateOutputPort.cs`
- [ ] `Assets/Scripts/OutGame/Application/Ports/Output/INavigationOutputPort.cs`

### UseCases
- [ ] `Assets/Scripts/OutGame/Application/UseCases/UpdateBgmVolumeInteractor.cs`
- [ ] `Assets/Scripts/OutGame/Application/UseCases/UpdateSeVolumeInteractor.cs`
- [ ] `Assets/Scripts/OutGame/Application/UseCases/NavigateToHomeInteractor.cs`
- [ ] `Assets/Scripts/OutGame/Application/UseCases/OpenSettingsInteractor.cs`
- [ ] `Assets/Scripts/OutGame/Application/UseCases/NavigateBackInteractor.cs`

## InterfaceAdapter層（Presentation）

### Controllers
- [ ] `Assets/Scripts/OutGame/Presentation/Controllers/TitleController.cs`
- [ ] `Assets/Scripts/OutGame/Presentation/Controllers/HomeController.cs`
- [ ] `Assets/Scripts/OutGame/Presentation/Controllers/SettingsController.cs`

### Presenters
- [ ] `Assets/Scripts/OutGame/Presentation/Presenters/VolumePresenter.cs`
- [ ] `Assets/Scripts/OutGame/Presentation/Presenters/NavigationPresenter.cs`

## FrameworkAndDriver層（Infrastructure）

### Repositories（実装）
- [ ] `Assets/Scripts/OutGame/Infrastructure/Repositories/AudioSettingsRepositoryImpl.cs`
- [ ] `Assets/Scripts/OutGame/Infrastructure/Repositories/UserProfileRepositoryImpl.cs`

## 修正したファイル

- [ ] `Assets/Scripts/OutGame/States/TitleState.cs`
- [ ] `Assets/Scripts/OutGame/States/HomeState.cs`
- [ ] `Assets/Scripts/OutGame/States/SettingsState.cs`
- [ ] `Assets/Scripts/OutGame/OutGameManager.cs`
- [ ] `Assets/Scripts/OutGame/Installers/OutGameInstaller.cs`

---

## Unityエディタでの確認手順

### 1. プロジェクトの再読み込み
1. Unityエディタを開く
2. `Assets > Refresh` を実行（または Ctrl+R）
3. コンソールにエラーが表示されていないか確認

### 2. .metaファイルの自動生成確認
各フォルダとファイルに対して`.meta`ファイルが自動生成されているか確認：
- `Domain.meta`
- `Domain/Models.meta`
- `Domain/Models/AudioSettings.cs.meta`
- など...

### 3. コンパイルエラーの確認
コンソールウィンドウで以下を確認：
- [ ] コンパイルエラーがない
- [ ] 警告が最小限である
- [ ] namespace の解決ができている

### 4. DIコンテナの確認
1. シーン内の`ProjectContext`または`SceneContext`を確認
2. `OutGameInstaller`がアタッチされているか確認
3. インスペクターで設定が正しいか確認

---

## トラブルシューティング

### エラー: "The type or namespace name 'Domain' does not exist"

**原因**: .metaファイルが生成されていない、またはUnityがファイルを認識していない

**解決方法**:
1. Unityエディタで `Assets > Refresh` を実行
2. それでも解決しない場合、Unityエディタを再起動
3. `Library`フォルダを削除して再コンパイル（最終手段）

### エラー: "The type or namespace name 'Zenject' could not be found"

**原因**: Zenjectがインストールされていない

**解決方法**:
1. Package ManagerでZenject（Extenject）をインストール
2. または、Unity Asset StoreからZenjectをインポート

### エラー: "The type or namespace name 'UniTask' could not be found"

**原因**: UniTaskがインストールされていない

**解決方法**:
1. Package Managerで UniTask をインストール
2. `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`

### エラー: "The type or namespace name 'R3' could not be found"

**原因**: R3がインストールされていない

**解決方法**:
1. Package Managerで R3 をインストール
2. `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity`

---

## 次のステップ

### 1. コンパイルの確認
- [ ] Unityエディタでコンパイルエラーがないことを確認

### 2. 実行テスト
- [ ] プレイモードで実行
- [ ] タイトル画面が表示されるか確認
- [ ] スタートボタンでホーム画面に遷移するか確認
- [ ] 設定ボタンで設定画面に遷移するか確認
- [ ] 設定画面で音量変更ができるか確認
- [ ] 戻るボタンで前の画面に戻れるか確認

### 3. デバッグログの確認
以下のログが出力されるか確認：
- "画面遷移開始: Home"
- "画面遷移完了"
- "音量を更新しました - BGM: XX%, SE: XX%"

### 4. PlayerPrefsの確認
設定画面で音量を変更後、以下のキーが保存されているか確認：
- `AudioSettings_BgmVolume`
- `AudioSettings_SeVolume`
- `AudioSettings_HasAdjusted`

---

## 完了条件

以下がすべて✅になったら、リファクタリング完了です：

- [ ] すべてのファイルが作成されている
- [ ] コンパイルエラーがない
- [ ] プレイモードで実行できる
- [ ] 画面遷移が正常に動作する
- [ ] 音量変更が正常に動作する
- [ ] データが永続化される（PlayerPrefs）
- [ ] デバッグログが正しく出力される

---

## 備考

### 既存のMVP層について
以下のファイルは今後削除予定ですが、段階的に移行するため現時点では残しています：
- `Assets/Scripts/OutGame/MVP/Title/TitlePresenter.cs`
- `Assets/Scripts/OutGame/MVP/Title/TitleModel.cs`
- `Assets/Scripts/OutGame/MVP/Home/HomePresenter.cs`
- `Assets/Scripts/OutGame/MVP/Home/HomeModel.cs`
- `Assets/Scripts/OutGame/MVP/Settings/SettingsPresenter.cs`
- `Assets/Scripts/OutGame/MVP/Settings/SettingsModel.cs`

新しいコードが正常に動作することを確認してから削除してください。

