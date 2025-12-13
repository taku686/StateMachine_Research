# クリーンアーキテクチャへのリファクタリング完了報告

## 実施内容

既存のMVPパターンのコードを、クリーンアーキテクチャに基づいて全面的にリファクタリングしました。

---

## 新しいディレクトリ構成

```
Assets/Scripts/OutGame/
├── Domain/                          # Entity層
│   ├── Models/
│   │   ├── AudioSettings.cs        # オーディオ設定エンティティ
│   │   └── UserProfile.cs          # ユーザープロフィールエンティティ
│   └── Repositories/
│       ├── IAudioSettingsRepository.cs
│       └── IUserProfileRepository.cs
│
├── Application/                     # UseCase層
│   ├── UseCases/
│   │   ├── UpdateBgmVolumeInteractor.cs
│   │   ├── UpdateSeVolumeInteractor.cs
│   │   ├── NavigateToHomeInteractor.cs
│   │   ├── OpenSettingsInteractor.cs
│   │   └── NavigateBackInteractor.cs
│   └── Ports/
│       ├── Input/
│       │   ├── IUpdateBgmVolumeInputPort.cs
│       │   ├── IUpdateSeVolumeInputPort.cs
│       │   ├── INavigateToHomeInputPort.cs
│       │   ├── IOpenSettingsInputPort.cs
│       │   └── INavigateBackInputPort.cs
│       └── Output/
│           ├── IVolumeUpdateOutputPort.cs
│           └── INavigationOutputPort.cs
│
├── Presentation/                    # InterfaceAdapter層
│   ├── Controllers/
│   │   ├── TitleController.cs
│   │   ├── HomeController.cs
│   │   └── SettingsController.cs
│   └── Presenters/
│       ├── VolumePresenter.cs
│       └── NavigationPresenter.cs
│
├── Infrastructure/                  # FrameworkAndDriver層
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── BaseView.cs
│   │   │   └── IView.cs
│   │   ├── TitleView.cs
│   │   ├── HomeView.cs
│   │   └── SettingsView.cs
│   └── Repositories/
│       ├── AudioSettingsRepositoryImpl.cs
│       └── UserProfileRepositoryImpl.cs
│
├── MVP/                             # 既存のMVP（段階的に削除予定）
│   └── ...
│
├── StateMachine/                    # 状態管理
│   └── ...
│
├── States/                          # 各画面のState
│   ├── TitleState.cs               # ✅ リファクタリング済み
│   ├── HomeState.cs                # ✅ リファクタリング済み
│   └── SettingsState.cs            # ✅ リファクタリング済み
│
└── Installers/                      # DI設定
    └── OutGameInstaller.cs         # ✅ 全層のバインド設定完了
```

---

## 作成したファイル一覧

### Entity層（Domain）
- ✅ `Domain/Models/AudioSettings.cs` - オーディオ設定エンティティ
- ✅ `Domain/Models/UserProfile.cs` - ユーザープロフィールエンティティ
- ✅ `Domain/Repositories/IAudioSettingsRepository.cs` - リポジトリインターフェース
- ✅ `Domain/Repositories/IUserProfileRepository.cs` - リポジトリインターフェース

### UseCase層（Application）
- ✅ `Application/Ports/Input/IUpdateBgmVolumeInputPort.cs`
- ✅ `Application/Ports/Input/IUpdateSeVolumeInputPort.cs`
- ✅ `Application/Ports/Input/INavigateToHomeInputPort.cs`
- ✅ `Application/Ports/Input/IOpenSettingsInputPort.cs`
- ✅ `Application/Ports/Input/INavigateBackInputPort.cs`
- ✅ `Application/Ports/Output/IVolumeUpdateOutputPort.cs`
- ✅ `Application/Ports/Output/INavigationOutputPort.cs`
- ✅ `Application/UseCases/UpdateBgmVolumeInteractor.cs`
- ✅ `Application/UseCases/UpdateSeVolumeInteractor.cs`
- ✅ `Application/UseCases/NavigateToHomeInteractor.cs`
- ✅ `Application/UseCases/OpenSettingsInteractor.cs`
- ✅ `Application/UseCases/NavigateBackInteractor.cs`

### InterfaceAdapter層（Presentation）
- ✅ `Presentation/Controllers/TitleController.cs`
- ✅ `Presentation/Controllers/HomeController.cs`
- ✅ `Presentation/Controllers/SettingsController.cs`
- ✅ `Presentation/Presenters/VolumePresenter.cs`
- ✅ `Presentation/Presenters/NavigationPresenter.cs`

### FrameworkAndDriver層（Infrastructure）
- ✅ `Infrastructure/Repositories/AudioSettingsRepositoryImpl.cs`
- ✅ `Infrastructure/Repositories/UserProfileRepositoryImpl.cs`

### 修正したファイル
- ✅ `States/TitleState.cs` - クリーンアーキテクチャに対応
- ✅ `States/HomeState.cs` - クリーンアーキテクチャに対応
- ✅ `States/SettingsState.cs` - クリーンアーキテクチャに対応
- ✅ `OutGameManager.cs` - DI対応
- ✅ `Installers/OutGameInstaller.cs` - 全層のバインド設定

---

## 処理フローの変化

### Before（MVPパターン）

```
View → Presenter → Model
  ↑        ↓
  └────────┘
```

Presenterがビジネスロジックと表示ロジックを両方持っていた。

### After（クリーンアーキテクチャ）

```
View → Controller → Interactor → Presenter → View
                        ↓
                    Repository
                        ↓
                      Entity
```

各層の責務が明確に分離された。

---

## 具体例：BGM音量変更の処理フロー

### Before
```
1. SettingsView: スライダー変更
2. SettingsPresenter: イベント受信 → Modelを更新
3. SettingsModel: 値を保持
4. SettingsPresenter: Viewを更新
```

### After
```
1. SettingsView: スライダー変更イベント発火
2. SettingsController: イベント受信 → データ検証
3. UpdateBgmVolumeInteractor: ビジネスロジック実行
   - Repositoryから現在の設定を取得
   - 妥当性チェック
   - エンティティを更新
   - Repositoryに保存
4. VolumePresenter: 結果を受信 → 表示用に変換
5. SettingsView: 画面を更新
```

---

## 各層の役割

### 1. Entity層（Domain）
**責務**: ビジネスルール、ドメインモデル

**例**:
- `AudioSettings`: 音量の妥当性チェック、音量更新
- `UserProfile`: ゴールドの加算・消費ロジック

**特徴**:
- 他の層に依存しない
- 純粋なC#クラス
- Unity非依存

### 2. UseCase層（Application）
**責務**: アプリケーション固有のビジネスロジック

**例**:
- `UpdateBgmVolumeInteractor`: BGM音量更新のシナリオ
- `NavigateToHomeInteractor`: ホーム画面遷移のシナリオ

**特徴**:
- Entityを操作
- Repositoryを通じてデータを永続化
- Presenterに結果を通知
- Unity非依存

### 3. InterfaceAdapter層（Presentation）
**責務**: データ変換、入出力の制御

**例**:
- `SettingsController`: Viewからの入力を受け取りUseCaseに渡す
- `VolumePresenter`: ビジネスデータを表示用データに変換

**特徴**:
- Controllerは「入力の窓口」
- Presenterは「出力の窓口」
- Unity非依存

### 4. FrameworkAndDriver層（Infrastructure）
**責務**: Unity固有の実装、外部ライブラリとの連携

**例**:
- `SettingsView`: MonoBehaviourを継承、UI操作
- `AudioSettingsRepositoryImpl`: PlayerPrefsでデータ永続化

**特徴**:
- Unity依存
- MonoBehaviourを使用
- 具体的な実装

---

## 依存関係の方向

```
外側（Infrastructure）
    ↓ 依存
中間（Presentation）
    ↓ 依存
内側（Application）
    ↓ 依存
中核（Domain）
```

**重要**: 内側は外側を知らない（依存しない）

---

## DIコンテナの設定（Zenject）

`OutGameInstaller.cs`で以下をバインド：

1. **Repository実装** → インターフェース
2. **Interactor** → InputPort
3. **Presenter** → OutputPort
4. **Controller** → 直接バインド
5. **State** → 直接バインド

すべての依存関係はコンストラクタインジェクションで解決。

---

## メリット

### 1. テスタビリティの向上
- Interactorは純粋なC#クラスなのでUnityエディタなしでテスト可能
- モックを使った単体テストが容易

### 2. 保守性の向上
- 各層の責務が明確
- ビジネスロジックの変更がUIに影響しない
- どこに何を書けばいいかが明確

### 3. 再利用性の向上
- Interactorは他のプラットフォームでも再利用可能
- 同じUseCaseを複数のControllerから呼び出せる

### 4. 拡張性の向上
- 新機能の追加が容易
- 既存コードへの影響を最小限に抑えられる

---

## 今後の作業

### 優先度：高
1. ✅ 既存のMVP層のPresenter/Modelを削除または非推奨化
2. ✅ クエスト画面、ショップ画面などの他の画面も同様にリファクタリング
3. ✅ Interactorの単体テストを追加

### 優先度：中
1. ✅ エラーハンドリングの強化
2. ✅ ローディング画面の実装
3. ✅ トースト通知の実装

### 優先度：低
1. ✅ ドキュメントの充実
2. ✅ コード規約の整備

---

## 注意点

### 1. 既存のMVP層について
- `MVP/`フォルダ内のPresenter/Modelは段階的に削除予定
- Viewは`Infrastructure/Views/`に移動済み
- 古いPresenter/Modelを使用しないように注意

### 2. Stateの作成方法
新しい画面を追加する場合：
1. View（Infrastructure層）
2. Controller（Presentation層）
3. Interactor（Application層）
4. Presenter（Presentation層）
5. State（Stateクラス）
6. Installer（DIバインド）

の順で作成してください。

### 3. ビジネスロジックの配置
- ビジネスロジックは必ずInteractorに記述
- ControllerやPresenterにはビジネスロジックを書かない
- Viewには一切のロジックを書かない

---

## まとめ

クリーンアーキテクチャへのリファクタリングにより、以下を実現しました：

✅ **責務の明確化**: 各層が明確な役割を持つ
✅ **依存関係の整理**: 外側→内側の一方向依存
✅ **テスタビリティ**: Interactorの単体テストが容易
✅ **保守性**: 変更に強い設計
✅ **拡張性**: 新機能の追加が容易

この設計により、長期的な保守性と拡張性を大幅に向上させることができました。

