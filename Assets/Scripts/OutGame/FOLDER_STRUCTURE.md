# OutGameフォルダ構造（クリーンアーキテクチャ）

## 📁 整理後のフォルダ構造

```
OutGame/
├── Domain/                          # Entity層（最も内側）
│   ├── Models/                      # ドメインモデル
│   │   ├── AudioSettings.cs         # オーディオ設定エンティティ
│   │   └── UserProfile.cs           # ユーザープロフィールエンティティ
│   └── Repositories/                # リポジトリインターフェース
│       ├── IAudioSettingsRepository.cs
│       └── IUserProfileRepository.cs
│
├── Application/                     # UseCase層（ビジネスロジック）
│   ├── Ports/
│   │   ├── Input/                   # UseCaseのインターフェース
│   │   │   ├── IUpdateBgmVolumeInputPort.cs
│   │   │   ├── IUpdateSeVolumeInputPort.cs
│   │   │   ├── INavigateToHomeInputPort.cs
│   │   │   ├── IOpenSettingsInputPort.cs
│   │   │   └── INavigateBackInputPort.cs
│   │   └── Output/                  # Presenterのインターフェース
│   │       ├── IVolumeUpdateOutputPort.cs
│   │       └── INavigationOutputPort.cs
│   └── UseCases/                    # Interactor（ビジネスロジックの実装）
│       ├── UpdateBgmVolumeInteractor.cs
│       ├── UpdateSeVolumeInteractor.cs
│       ├── NavigateToHomeInteractor.cs
│       ├── OpenSettingsInteractor.cs
│       └── NavigateBackInteractor.cs
│
├── Presentation/                    # InterfaceAdapter層（データ変換）
│   ├── Controllers/                 # 入力の窓口
│   │   ├── TitleController.cs
│   │   ├── HomeController.cs
│   │   └── SettingsController.cs
│   └── Presenters/                  # 出力の窓口
│       ├── VolumePresenter.cs
│       └── NavigationPresenter.cs
│
├── Infrastructure/                  # FrameworkAndDriver層（最も外側）
│   ├── Views/                       # Unity固有のView実装
│   │   ├── Base/
│   │   │   ├── BaseView.cs
│   │   │   └── IView.cs
│   │   ├── TitleView.cs
│   │   ├── HomeView.cs
│   │   └── SettingsView.cs
│   └── Repositories/                # リポジトリの実装
│       ├── AudioSettingsRepositoryImpl.cs
│       └── UserProfileRepositoryImpl.cs
│
├── States/                          # 状態管理
│   ├── TitleState.cs
│   ├── HomeState.cs
│   └── SettingsState.cs
│
├── StateMachine/                    # ステートマシン
│   ├── IState.cs
│   ├── BaseState.cs
│   └── StateMachine.cs
│
├── Installers/                      # DI設定（Zenject）
│   ├── OutGameInstaller.cs
│   └── OutGameSceneInstaller.cs
│
├── _Deprecated/                     # 非推奨（旧MVP層）
│   └── MVP/
│       ├── Base/
│       ├── Title/
│       ├── Home/
│       └── Settings/
│
├── OutGameManager.cs                # アウトゲーム全体の管理
└── OutGameStateKey.cs               # 状態キーの定義
```

---

## 🔄 変更内容

### 1. Viewの移動
**Before**: `MVP/Title/TitleView.cs`
**After**: `Infrastructure/Views/TitleView.cs`

すべてのViewファイルを`Infrastructure/Views/`に統合しました。

### 2. 名前空間の変更
**Before**: `namespace OutGame.MVP.Title`
**After**: `namespace OutGame.Infrastructure.Views`

すべてのViewが統一された名前空間を使用するようになりました。

### 3. 古いMVP層の移動
**Before**: `MVP/` フォルダに散在
**After**: `_Deprecated/MVP/` に移動

古いPresenter/Modelファイルは`_Deprecated`フォルダに移動しました。
これらのファイルは今後削除予定です。

---

## 📊 各層の役割

### Domain層（Entity層）
- **依存**: なし（最も独立）
- **役割**: ビジネスルール、ドメインモデル
- **Unity依存**: なし
- **例**: `AudioSettings`, `UserProfile`

### Application層（UseCase層）
- **依存**: Domain層のみ
- **役割**: アプリケーション固有のビジネスロジック
- **Unity依存**: なし
- **例**: `UpdateBgmVolumeInteractor`, `NavigateToHomeInteractor`

### Presentation層（InterfaceAdapter層）
- **依存**: Application層、Domain層
- **役割**: データ変換、入出力の制御
- **Unity依存**: なし
- **例**: `TitleController`, `VolumePresenter`

### Infrastructure層（FrameworkAndDriver層）
- **依存**: すべての層
- **役割**: Unity固有の実装、外部ライブラリとの連携
- **Unity依存**: あり（MonoBehaviour、PlayerPrefs等）
- **例**: `TitleView`, `AudioSettingsRepositoryImpl`

---

## 🎯 依存関係の方向

```
Infrastructure（外側）
    ↓ 依存
Presentation（中間）
    ↓ 依存
Application（内側）
    ↓ 依存
Domain（中核）
```

**重要**: 内側の層は外側の層を知らない（依存しない）

---

## 📝 名前空間の対応表

| 層 | 名前空間 |
|---|---|
| Domain層 | `OutGame.Domain.Models`<br>`OutGame.Domain.Repositories` |
| Application層 | `OutGame.Application.Ports.Input`<br>`OutGame.Application.Ports.Output`<br>`OutGame.Application.UseCases` |
| Presentation層 | `OutGame.Presentation.Controllers`<br>`OutGame.Presentation.Presenters` |
| Infrastructure層 | `OutGame.Infrastructure.Views`<br>`OutGame.Infrastructure.Repositories` |

---

## 🗑️ 削除予定のファイル

以下のファイルは`_Deprecated/MVP/`に移動されており、今後削除予定です：

### Base
- `BaseModel.cs`
- `BasePresenter.cs`
- `IModel.cs`
- `IPresenter.cs`

### Title
- `TitleModel.cs`
- `TitlePresenter.cs`

### Home
- `HomeModel.cs`
- `HomePresenter.cs`

### Settings
- `SettingsModel.cs`
- `SettingsPresenter.cs`

**注意**: これらのファイルは新しいアーキテクチャで置き換えられています。
削除する前に、すべての機能が正常に動作することを確認してください。

---

## ✅ 整理完了後の確認事項

### 1. Unityエディタでの確認
- [ ] `Assets > Refresh` を実行（Ctrl+R）
- [ ] コンパイルエラーがないことを確認
- [ ] .metaファイルが正しく生成されていることを確認

### 2. 名前空間の確認
- [ ] すべてのViewが`OutGame.Infrastructure.Views`を使用
- [ ] すべてのControllerが`OutGame.Presentation.Controllers`を使用
- [ ] すべてのPresenterが`OutGame.Presentation.Presenters`を使用

### 3. 動作確認
- [ ] プレイモードで実行
- [ ] タイトル画面が表示される
- [ ] 画面遷移が正常に動作する
- [ ] 音量変更が正常に動作する

---

## 🚀 次のステップ

### 短期（1週間以内）
1. すべての機能が正常に動作することを確認
2. 単体テストの追加
3. `_Deprecated`フォルダの削除

### 中期（1ヶ月以内）
1. 他の画面（Quest、Shop等）も同様にリファクタリング
2. エラーハンドリングの強化
3. ローディング画面の実装

### 長期（3ヶ月以内）
1. ドキュメントの充実
2. コード規約の整備
3. CI/CDの導入

---

## 📚 参考資料

- [CleanArchitecture_Summary.md](../../CleanArchitecture_Summary.md) - クリーンアーキテクチャの概要
- [CleanArchitecture_DetailedExplanation.md](../../CleanArchitecture_DetailedExplanation.md) - 各コンポーネントの詳細解説
- [REFACTORING_SUMMARY.md](../../REFACTORING_SUMMARY.md) - リファクタリング完了報告

