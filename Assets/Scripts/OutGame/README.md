# OutGame アーキテクチャ設計

## 概要
このプロジェクトは、ステートマシーンパターンとMVPパターンを組み合わせたアウトゲーム部分の実装です。

## アーキテクチャ

### ステートマシーン
- **StateMachine<TStateKey>**: ジェネリックなステートマシーン実装
- **IState**: ステートの基本インターフェース
- **BaseState**: ステートの基底クラス

### MVPパターン
- **Model**: データとビジネスロジックを管理（R3のReactivePropertyを使用）
- **View**: UI表示とユーザー入力を管理（R3のObservableでイベント通知）
- **Presenter**: ModelとViewを仲介し、ロジックを制御

### 依存性注入
- **Zenject**: DIコンテナとして使用
- **OutGameInstaller**: アウトゲーム用のインストーラー

### 非同期処理
- **UniTask**: すべての非同期処理に使用（コルーチンは使用しない）

### アセット管理
- **Addressables**: UI Prefabのロードに使用

## フォルダ構成

```
OutGame/
├── StateMachine/          # ステートマシーン基盤
│   ├── IState.cs
│   ├── BaseState.cs
│   └── StateMachine.cs
├── MVP/                   # MVP基盤と各画面実装
│   ├── Base/             # MVP基底クラス
│   │   ├── IView.cs
│   │   ├── IPresenter.cs
│   │   ├── IModel.cs
│   │   ├── BaseView.cs
│   │   ├── BasePresenter.cs
│   │   └── BaseModel.cs
│   ├── Title/            # タイトル画面
│   │   ├── TitleView.cs
│   │   ├── TitleModel.cs
│   │   └── TitlePresenter.cs
│   ├── Home/             # ホーム画面
│   │   ├── HomeView.cs
│   │   ├── HomeModel.cs
│   │   └── HomePresenter.cs
│   └── Settings/         # 設定画面
│       ├── SettingsView.cs
│       ├── SettingsModel.cs
│       └── SettingsPresenter.cs
├── States/               # 各画面のステート実装
│   ├── TitleState.cs
│   ├── HomeState.cs
│   └── SettingsState.cs
├── Installers/           # Zenjectインストーラー
│   ├── OutGameInstaller.cs
│   └── OutGameSceneInstaller.cs
├── OutGameStateKey.cs    # ステート種類の定義
└── OutGameManager.cs     # アウトゲーム全体の管理

```

## 使用方法

### 1. シーンセットアップ
1. シーンに `SceneContext` を配置
2. `OutGameSceneInstaller` をアタッチ
3. `OutGameManager` をシーンに配置

### 2. View Prefabの作成
各画面のView Prefabを作成し、Addressablesに登録：
- `TitleView`: タイトル画面のUI
- `HomeView`: ホーム画面のUI
- `SettingsView`: 設定画面のUI

### 3. 新しい画面の追加手順

#### 3.1 ステートキーの追加
```csharp
// OutGameStateKey.cs
public enum OutGameStateKey
{
    None,
    Title,
    Home,
    Settings,
    NewScreen, // 追加
}
```

#### 3.2 MVP実装の作成
```csharp
// NewScreenView.cs
public class NewScreenView : BaseView
{
    [SerializeField] private Button _someButton;
    public Observable<Unit> OnSomeButtonClicked => 
        _someButton.OnClickAsObservable();
}

// NewScreenModel.cs
public class NewScreenModel : BaseModel
{
    private readonly ReactiveProperty<string> _someData = new("");
    public ReadOnlyReactiveProperty<string> SomeData => _someData;
    
    public NewScreenModel()
    {
        _someData.AddTo(Disposables);
    }
}

// NewScreenPresenter.cs
public class NewScreenPresenter : BasePresenter<NewScreenView, NewScreenModel>
{
    public event Action OnSomeAction;
    
    public NewScreenPresenter(NewScreenView view, NewScreenModel model) 
        : base(view, model) { }
    
    public override async UniTask Initialize()
    {
        await base.Initialize();
        
        View.OnSomeButtonClicked
            .Subscribe(_ => OnSomeAction?.Invoke())
            .AddTo(Disposables);
    }
}
```

#### 3.3 ステートの作成
```csharp
// NewScreenState.cs
public class NewScreenState : BaseState
{
    private readonly StateMachine<OutGameStateKey> _stateMachine;
    private NewScreenPresenter _presenter;
    
    public NewScreenState(StateMachine<OutGameStateKey> stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public override async UniTask OnEnter()
    {
        var viewObject = await Addressables.InstantiateAsync("NewScreenView");
        var view = viewObject.GetComponent<NewScreenView>();
        var model = new NewScreenModel();
        
        _presenter = new NewScreenPresenter(view, model);
        await _presenter.Initialize();
        
        _presenter.OnSomeAction += OnSomeAction;
        await _presenter.Show();
    }
    
    public override async UniTask OnExit()
    {
        if (_presenter != null)
        {
            _presenter.OnSomeAction -= OnSomeAction;
            await _presenter.Hide();
            _presenter.Dispose();
            _presenter = null;
        }
    }
    
    private void OnSomeAction()
    {
        _stateMachine.ChangeState(OutGameStateKey.Home).Forget();
    }
}
```

#### 3.4 OutGameManagerに登録
```csharp
// OutGameManager.cs の InitializeStateMachine() に追加
_stateMachine.RegisterState(
    OutGameStateKey.NewScreen, 
    new NewScreenState(_stateMachine)
);
```

## 設計の特徴

### 1. 責任の分離
- **State**: 画面遷移とライフサイクル管理
- **Presenter**: UIロジックとイベント処理
- **Model**: データ管理
- **View**: UI表示とユーザー入力

### 2. 疎結合
- Zenjectによる依存性注入
- イベントベースの通信
- インターフェースによる抽象化

### 3. 拡張性
- 新しい画面の追加が容易
- ステートとMVPの独立した拡張が可能

### 4. テスタビリティ
- 各コンポーネントが独立してテスト可能
- モックの作成が容易

## 注意事項

1. **UniTaskの使用**: すべての非同期処理はUniTaskを使用（コルーチンは使用しない）
2. **R3の使用**: ボタンイベントはR3のObservableを使用
3. **Addressables**: UI PrefabはすべてAddressablesで管理
4. **Zenject**: GameObjectの生成はZenjectを通して行う
5. **メモリ管理**: Dispose処理を確実に実行（R3のAddToを活用）

## 今後の拡張案

- ステート履歴の管理（戻るボタン対応）
- ステート間のデータ受け渡し機構
- ローディング画面の統合
- トランジションアニメーションの共通化
- エラーハンドリングの統一

