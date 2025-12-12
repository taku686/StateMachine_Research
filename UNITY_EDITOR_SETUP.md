# Unity Editor 作業手順書

> **このファイルについて**  
> C#コードの実装完了後、Unity Editor上で行う作業の手順をまとめたものです。  
> 手順に従って、Prefab作成、シーン作成、オブジェクト配置を行ってください。

---

## 📋 作業前の確認

### 実装済みC#ファイルの確認

以下のファイルが作成されていることを確認してください：

```
Assets/Scripts/Common/
├── Domain/Services/ (5ファイル)
├── Application/ (2ファイル + Bootstrap/)
├── Infrastructure/ (4ファイル)
└── Installers/ (2ファイル)

Assets/Scripts/OutGame/
├── Application/UseCases/StartBattleInteractor.cs
├── Domain/Models/UserProfile.cs (修正済み)
├── Presentation/Controllers/HomeController.cs (修正済み)
└── Installers/OutGameInstaller.cs (修正済み)

Assets/Scripts/InGame/
├── InGameManager.cs
├── Application/UseCases/CompleteBattleInteractor.cs (修正済み)
└── States/BattlePreparationState.cs (修正済み)
```

---

## 🎯 手順1: ProjectContext.prefab 作成

### 目的
ゲーム全体で共有されるサービス（シーンローダーなど）を管理するProjectContextを作成します。

### 手順

1. **Resourcesフォルダの確認**
   - `Assets/Resources/` フォルダが存在するか確認
   - 存在しない場合は作成

2. **ProjectContext.prefab作成**
   
   **方法A: Zenjectメニューから自動作成（推奨）**
   ```
   1. Hierarchyビューで右クリック
   2. Zenject > Scene Context > ProjectContext を選択
   3. 自動的に Resources/ProjectContext.prefab が作成される
   ```

   **方法B: 手動作成**
   ```
   1. Hierarchyビューで空のGameObjectを作成
   2. 名前を "ProjectContext" に変更
   3. Add Component > ProjectContext をアタッチ
   4. Assets/Resources/ にドラッグしてPrefab化
   5. Hierarchyから削除
   ```

3. **ProjectContextInstallerのアタッチ**
   ```
   1. Resources/ProjectContext.prefab をダブルクリックで開く
   2. Inspector > ProjectContext コンポーネント
   3. Installers リストの + ボタンをクリック
   4. ProjectContextInstaller をドラッグ＆ドロップ
      （Assets/Scripts/Common/Installers/ProjectContextInstaller.cs）
   5. Parent New Objects Under Context: チェックを入れる（推奨）
   6. Prefabを保存（Ctrl+S）
   ```

4. **確認**
   - ProjectContext.prefabが `Assets/Resources/` に存在
   - Installerリストに ProjectContextInstaller が設定されている

---

## 🎯 手順2: Bootstrap.unity シーン作成

### 目的
ゲーム起動時の初期化処理を行うBootstrapシーンを作成します。

### 手順

1. **新規シーン作成**
   ```
   1. File > New Scene
   2. 空のシーンを選択
   3. File > Save As...
   4. 保存先: Assets/Scenes/Bootstrap.unity
   ```

2. **SceneContext作成**
   ```
   1. Hierarchyビューで右クリック
   2. Zenject > Scene Context
   3. 名前が "SceneContext" になっていることを確認
   ```

3. **BootstrapInstallerのアタッチ**
   ```
   1. SceneContext を選択
   2. Inspector > Scene Context コンポーネント
   3. Installers リストの + ボタンをクリック
   4. BootstrapInstaller をドラッグ＆ドロップ
      （Assets/Scripts/Common/Installers/BootstrapInstaller.cs）
   ```

4. **GameBootstrapオブジェクト作成**
   ```
   1. Hierarchyビューで右クリック > Create Empty
   2. 名前を "GameBootstrap" に変更
   3. Add Component > GameBootstrap をアタッチ
      （Assets/Scripts/Common/Application/Bootstrap/GameBootstrap.cs）
   ```

5. **シーン保存**
   ```
   Ctrl+S で保存
   ```

6. **Build Settingsに追加**
   ```
   1. File > Build Settings...
   2. Scenes In Build リストの一番上にドラッグ＆ドロップ
   3. Bootstrap.unity が Index 0 になっていることを確認
   ```

---

## 🎯 手順3: Transition.unity シーン作成

### 目的
シーン遷移時に常駐し、ローディング画面を表示する中継シーンを作成します。

### 手順

1. **新規シーン作成**
   ```
   1. File > New Scene
   2. 空のシーンを選択
   3. File > Save As...
   4. 保存先: Assets/Scenes/Transition.unity
   ```

2. **Canvas作成**
   ```
   1. Hierarchyビューで右クリック
   2. UI > Canvas
   3. Canvas の Render Mode を確認
      - Screen Space - Overlay に設定（デフォルト）
   ```

3. **LoadingView配置**
   ```
   1. Project ビューで Assets/Prefab/LoadingView.prefab を探す
   2. Hierarchy の Canvas の下にドラッグ＆ドロップ
   3. Canvas
      └── LoadingView
      という構造になっていることを確認
   ```

4. **LoadingViewの設定確認**
   ```
   1. LoadingView を選択
   2. Inspector で以下を確認：
      - Canvas Group コンポーネントが存在
      - Background Image が設定されている
      - Loading Elements が設定されている
      - Progress Bar が設定されている
      - Spinner Image と Frames が設定されている
   ```

5. **シーン保存**
   ```
   Ctrl+S で保存
   ```

6. **Build Settingsに追加**
   ```
   1. File > Build Settings...
   2. Transition.unity をドラッグ＆ドロップ
   3. 順序は Bootstrap の次（Index 1）
   ```

---

## 🎯 手順4: InGame.unity シーン設定

### 目的
InGameシーンにInGameManagerを配置し、シーン遷移システムと統合します。

### 手順

1. **InGame.unityを開く**
   ```
   Assets/Scenes/InGame.unity をダブルクリック
   ```

2. **SceneContextの確認**
   ```
   1. Hierarchyに "SceneContext" が存在するか確認
   2. 存在しない場合:
      - 右クリック > Zenject > Scene Context
   3. Inspector > Scene Context コンポーネント
   4. Installers リストに InGameInstaller が設定されているか確認
   ```

3. **InGameManagerオブジェクト作成**
   ```
   1. Hierarchyビューで右クリック > Create Empty
   2. 名前を "InGameManager" に変更
   3. Add Component > InGameManager をアタッチ
      （Assets/Scripts/InGame/InGameManager.cs）
   ```

4. **シーン保存**
   ```
   Ctrl+S で保存
   ```

5. **Build Settingsに追加（未追加の場合）**
   ```
   1. File > Build Settings...
   2. InGame.unity がリストに存在するか確認
   3. なければドラッグ＆ドロップで追加
   ```

---

## 🎯 手順5: OutGame.unity シーン確認

### 目的
OutGameシーンが正しく設定されているか確認します。

### 手順

1. **OutGame.unityを開く**
   ```
   Assets/Scenes/OutGame.unity をダブルクリック
   ```

2. **SceneContextの確認**
   ```
   1. Hierarchyに "SceneContext" が存在するか確認
   2. Inspector > Scene Context コンポーネント
   3. Installers リストに OutGameInstaller が設定されているか確認
   ```

3. **Build Settingsに追加（未追加の場合）**
   ```
   1. File > Build Settings...
   2. OutGame.unity がリストに存在するか確認
   3. なければドラッグ＆ドロップで追加
   ```

---

## 🎯 手順6: Build Settings 最終確認

### 目的
シーンの読み込み順序を正しく設定します。

### 正しい順序

```
Scenes In Build:
✅ 0. Bootstrap.unity      (起動シーン)
✅ 1. Transition.unity     (中継シーン)
✅ 2. OutGame.unity        (アウトゲーム)
✅ 3. InGame.unity         (インゲーム)
```

### 確認方法

```
1. File > Build Settings...
2. 上記の順序になっているか確認
3. 順序が異なる場合はドラッグ＆ドロップで並び替え
```

---

## 🎯 手順7: 動作確認

### 7-1. 起動フロー確認

1. **Bootstrap.unityから起動**
   ```
   1. Bootstrap.unity を開く
   2. Playボタンを押す
   ```

2. **期待される動作**
   ```
   Console出力:
   [GameBootstrap] Game starting...
   [BootstrapSequence] === Starting Bootstrap Sequence ===
   [Bootstrap] Loading Transition scene...
   [Bootstrap] Transition scene loaded
   [Bootstrap] Loading OutGame scene...
   [SceneTransitionOrchestrator] Initial scene set: OutGame
   [Bootstrap] OutGame scene loaded and set as active
   [Bootstrap] Unloading Bootstrap scene...
   [Bootstrap] Bootstrap scene unloaded
   [BootstrapSequence] === Bootstrap Sequence Complete ===
   [OutGameManager] Initializing StateMachine...
   ```

3. **確認ポイント**
   - ✅ Bootstrapシーンが破棄される
   - ✅ Transitionシーンが常駐する
   - ✅ OutGameシーンが表示される
   - ✅ タイトル画面またはホーム画面が表示される

### 7-2. OutGame → InGame 遷移確認

1. **ホーム画面のクエストボタンをクリック**
   ```
   1. ホーム画面が表示されたら
   2. "Quest" ボタンをクリック
   ```

2. **期待される動作**
   ```
   Console出力:
   [StartBattleInteractor] Starting battle for stage 1
   [StartBattleInteractor] CurrentStageId saved: 1
   [SceneTransitionOrchestrator] === Starting transition to InGame ===
   [Phase 1] Fade In
   [Phase 2] Show Loading
   [Phase 3] Loading scene: InGame
   [SceneLoadService] Loading scene: InGame
   [SceneLoadService] Scene loaded: InGame
   [Phase 4] Unloading previous scene: OutGame
   [SceneUnloadService] Unloading scene: OutGame
   [SceneUnloadService] Unused assets unloaded and GC executed
   [Phase 5] Setting active scene: InGame
   [Phase 6] Hide Loading
   [Phase 7] Fade Out
   [SceneTransitionOrchestrator] === Transition complete: InGame ===
   [InGameManager] Starting InGame...
   [BattlePreparationState] Battle Preparation Started
   ```

3. **確認ポイント**
   - ✅ 画面がフェードイン（暗くなる）
   - ✅ ローディング画面が表示される
   - ✅ プログレスバーが進行する
   - ✅ OutGameシーンが破棄される
   - ✅ InGameシーンが表示される
   - ✅ フェードアウト（明るくなる）
   - ✅ カクツキが少ない

### 7-3. InGame → OutGame 復帰確認

1. **バトル完了処理を実行**
   ```
   注: 現在バトルシステムは未実装のため、
   CompleteBattleInteractor.Execute(true) を
   テストコードまたはデバッグボタンから呼び出す
   ```

2. **期待される動作**
   ```
   Console出力:
   [CompleteBattleInteractor] Returning to OutGame...
   [SceneTransitionOrchestrator] === Starting transition to OutGame ===
   （以下、遷移フローのログ）
   [OutGameManager] Initializing StateMachine...
   ```

3. **確認ポイント**
   - ✅ InGameシーンが破棄される
   - ✅ OutGameシーンに復帰する
   - ✅ ホーム画面が表示される

---

## 🐛 トラブルシューティング

### 問題1: ProjectContextが見つからない

**症状**
```
MissingReferenceException: The object of type 'ProjectContext' has been destroyed
```

**解決方法**
1. Resources/ProjectContext.prefab が正しく作成されているか確認
2. Prefab内にProjectContextInstallerが設定されているか確認
3. Unity Editorを再起動

---

### 問題2: LoadingViewが見つからない

**症状**
```
ZenjectException: Unable to resolve 'LoadingView'
```

**解決方法**
1. Transition.unityシーンにLoadingViewが配置されているか確認
2. LoadingViewがCanvas配下にあるか確認
3. ProjectContextInstallerで `.FromComponentInHierarchy()` が設定されているか確認
4. Transitionシーンが正しくロードされているか確認

---

### 問題3: シーンが読み込まれない

**症状**
```
Scene 'OutGame' couldn't be loaded because it has not been added to the build settings
```

**解決方法**
1. File > Build Settings を開く
2. 必要なシーンがすべて追加されているか確認
3. 順序が正しいか確認（Bootstrap → Transition → OutGame → InGame）

---

### 問題4: IUserProfileRepository が解決できない

**症状**
```
ZenjectException: Unable to resolve 'IUserProfileRepository' 
while building object with type 'StartBattleInteractor'
```

**解決方法**
1. ProjectContextInstaller で IUserProfileRepository がバインドされているか確認
2. OutGameInstaller から IUserProfileRepository のバインドを削除（重複防止）
3. ProjectContext.prefab に ProjectContextInstaller が設定されているか確認

---

## 📊 最終チェックリスト

### ProjectContext
- [ ] Resources/ProjectContext.prefab が存在
- [ ] ProjectContextInstaller が設定されている
- [ ] Parent New Objects Under Context がチェックされている

### Bootstrap.unity
- [ ] Assets/Scenes/Bootstrap.unity が存在
- [ ] SceneContext が配置されている
- [ ] BootstrapInstaller が SceneContext に設定されている
- [ ] GameBootstrap オブジェクトが配置されている
- [ ] Build Settings の Index 0 に設定

### Transition.unity
- [ ] Assets/Scenes/Transition.unity が存在
- [ ] Canvas が配置されている
- [ ] LoadingView が Canvas 配下に配置されている
- [ ] Build Settings の Index 1 に設定

### InGame.unity
- [ ] SceneContext が配置されている
- [ ] InGameInstaller が SceneContext に設定されている
- [ ] InGameManager オブジェクトが配置されている
- [ ] Build Settings に追加されている

### OutGame.unity
- [ ] SceneContext が配置されている
- [ ] OutGameInstaller が SceneContext に設定されている
- [ ] Build Settings に追加されている

### 動作確認
- [ ] Bootstrap.unity から起動できる
- [ ] OutGame シーンが正しく表示される
- [ ] クエストボタンで InGame に遷移できる
- [ ] フェード・ローディング画面が表示される
- [ ] InGame から OutGame に復帰できる（実装後）

---

## 📚 参考情報

### Zenject SceneContext とは

- 各シーン固有のDIコンテナ
- シーンごとに異なるインスタンスを管理
- ProjectContextの子コンテナとして動作

### シーンの依存関係

```
Bootstrap (起動)
    ↓
Transition (常駐・Additive)
    ↓
OutGame (Additive・置き換え可能)
    ↔
InGame (Additive・置き換え可能)
```

### ログの見方

重要なログの意味：
- `[BootstrapSequence]`: Bootstrap処理の進行状況
- `[SceneTransitionOrchestrator]`: シーン遷移の進行状況
- `[Phase N]`: 遷移フローのどのフェーズか
- `[SceneLoadService]`: シーン読み込みの詳細
- `[SceneUnloadService]`: シーン破棄の詳細

---

## 🎉 完了

すべての手順が完了したら、Bootstrap.unity から起動してゲーム全体の動作を確認してください。

問題が発生した場合は、トラブルシューティングセクションを参照するか、Consoleログを確認して原因を特定してください。
