# OutGame アーキテクチャ設計ドキュメント

> 最終更新: 2025年12月6日

このドキュメントでは、OutGameモジュールのアーキテクチャ、設計方針、実装詳細を説明します。

---

## 📚 目次

1. [概要](#1-概要)
2. [フォルダ構成](#2-フォルダ構成)
3. [クリーンアーキテクチャ](#3-クリーンアーキテクチャ)
4. [StateMachine（状態遷移）](#4-statemachine状態遷移)
5. [Addressables管理](#5-addressables管理)
6. [プリロード/クリーンアップ](#6-プリロードクリーンアップ)
7. [遷移アニメーション](#7-遷移アニメーション)
8. [DI（依存性注入）](#8-di依存性注入)
9. [新規画面の追加方法](#9-新規画面の追加方法)

---

<a id="1-概要"></a>
## 1. 概要

<details>
<summary>📖 クリックして展開</summary>

### 技術スタック

| カテゴリ | 使用技術 |
|---------|---------|
| アーキテクチャ | Clean Architecture |
| 状態管理 | StateMachine パターン |
| DI | Zenject |
| 非同期処理 | UniTask |
| アセット管理 | Addressables |
| リアクティブ | R3 (Reactive Extensions) |
| アニメーション | DOTween |

### 設計原則

- **DRY原則**: 重複コードの排除
- **SOLID原則**: 単一責任、インターフェース分離
- **Installer主導**: インスタンス化はInstallerで一元管理
- **型安全性**: ジェネリクスによるコンパイル時チェック

</details>

---

<a id="2-フォルダ構成"></a>
## 2. フォルダ構成

<details>
<summary>📖 クリックして展開</summary>

```
OutGame/
├── Application/                    # アプリケーション層
│   ├── Ports/
│   │   ├── Input/                 # 入力ポート（UseCase呼び出し用）
│   │   │   ├── INavigateToHomeInputPort.cs
│   │   │   ├── IOpenSettingsInputPort.cs
│   │   │   ├── INavigateBackInputPort.cs
│   │   │   ├── IUpdateBgmVolumeInputPort.cs
│   │   │   └── IUpdateSeVolumeInputPort.cs
│   │   └── Output/                # 出力ポート（結果通知用）
│   │       ├── INavigationOutputPort.cs
│   │       └── IVolumeUpdateOutputPort.cs
│   └── UseCases/                  # ユースケース実装（Interactor）
│       ├── NavigateToHomeInteractor.cs
│       ├── OpenSettingsInteractor.cs
│       ├── NavigateBackInteractor.cs
│       ├── UpdateBgmVolumeInteractor.cs
│       └── UpdateSeVolumeInteractor.cs
│
├── Domain/                         # ドメイン層
│   ├── Models/                    # エンティティ/値オブジェクト
│   │   ├── AudioSettings.cs
│   │   └── UserProfile.cs
│   └── Repositories/              # リポジトリインターフェース
│       ├── IAudioSettingsRepository.cs
│       └── IUserProfileRepository.cs
│
├── Infrastructure/                 # インフラストラクチャ層
│   ├── Addressables/              # Addressables管理
│   │   ├── AddressableAssetKey.cs
│   │   ├── IAddressableAssetProvider.cs
│   │   └── AddressableAssetProvider.cs
│   ├── Factories/                 # ファクトリー
│   │   ├── IViewFactory.cs
│   │   ├── ViewAsyncFactory.cs
│   │   ├── AddressableViewFactory.cs
│   │   └── IAssetPreloadable.cs
│   ├── Repositories/              # リポジトリ実装
│   │   ├── AudioSettingsRepositoryImpl.cs
│   │   └── UserProfileRepositoryImpl.cs
│   └── Views/                     # View実装
│       ├── Base/
│       │   ├── IView.cs
│       │   └── BaseView.cs
│       ├── TitleView.cs
│       ├── HomeView.cs
│       ├── SettingsView.cs
│       └── LoadingView.cs         # ローディング＆フェード兼用
│
├── Presentation/                   # プレゼンテーション層
│   ├── Controllers/               # コントローラー
│   │   ├── TitleController.cs
│   │   ├── HomeController.cs
│   │   └── SettingsController.cs
│   └── Presenters/                # プレゼンター
│       ├── VolumePresenter.cs
│       └── NavigationPresenter.cs
│
├── StateMachine/                   # 状態遷移管理
│   ├── IState.cs
│   ├── BaseState.cs
│   ├── StateMachine.cs
│   ├── IStateTransitionAnimator.cs
│   └── FadeTransitionAnimator.cs
│
├── States/                         # 各画面のState
│   ├── TitleState.cs
│   ├── HomeState.cs
│   └── SettingsState.cs
│
├── Installers/                     # Zenject Installer
│   └── OutGameInstaller.cs
│
├── OutGameManager.cs               # 全体管理
└── OutGameStateKey.cs              # State識別用Enum
```

</details>

---

<a id="3-クリーンアーキテクチャ"></a>
## 3. クリーンアーキテクチャ

<details>
<summary>📖 クリックして展開</summary>

### レイヤー構成

```
┌─────────────────────────────────────────────────┐
│                 Presentation                     │
│    (Controllers, Presenters, Views)              │
├─────────────────────────────────────────────────┤
│                  Application                     │
│         (UseCases/Interactors, Ports)           │
├─────────────────────────────────────────────────┤
│                    Domain                        │
│         (Entities, Repositories IF)              │
├─────────────────────────────────────────────────┤
│                Infrastructure                    │
│    (Repository Impl, External Services)          │
└─────────────────────────────────────────────────┘
```

### データフロー

```
View → Controller → InputPort → Interactor → Repository
                                    ↓
View ← Presenter ← OutputPort ← Interactor
```

### 例：音量変更フロー

```csharp
// 1. View: ユーザー入力を検知
_bgmSlider.OnValueChangedAsObservable()
    .Subscribe(value => _onBgmVolumeChanged.OnNext(value));

// 2. Controller: InputPortを呼び出し
_updateBgmVolumeInputPort.Execute(value);

// 3. Interactor: ビジネスロジック実行
public void Execute(float volume)
{
    var settings = _repository.Load();
    settings.SetBgmVolume(volume);
    _repository.Save(settings);
    _outputPort.OnBgmVolumeUpdated(volume);
}

// 4. Presenter: Viewを更新
public void OnBgmVolumeUpdated(float volume)
{
    _view?.SetBgmVolume(volume);
}
```

</details>

---

<a id="4-statemachine状態遷移"></a>
## 4. StateMachine（状態遷移）

<details>
<summary>📖 クリックして展開</summary>

### IState インターフェース

```csharp
public interface IState
{
    // アセットプリロード（State遷移前）
    UniTask OnPreloadAssets();
    
    // データロード（State遷移前）
    UniTask OnLoadData();
    
    // State入場
    UniTask OnEnter();
    
    // State退出
    UniTask OnExit();
    
    // アセットクリーンアップ（State退出後）
    UniTask OnCleanupAssets();
    
    // データクリーンアップ（State退出後）
    UniTask OnCleanupData();
    
    // 更新処理
    void OnUpdate();
}
```

### State遷移フロー（5フェーズ）

```
State A → State B への遷移

┌─ Phase 1: 遷移開始アニメーション ─┐
│  PlayExitTransition()            │ ← フェードアウト
└─────────────────────────────────┘
              ↓
┌─ Phase 2: 準備 ─────────────────┐
│  ShowLoading()                   │
│  OnPreloadAssets()              │ ← アセットロード
│  OnLoadData()                   │ ← データロード（並行）
│  HideLoading()                   │
└─────────────────────────────────┘
              ↓
┌─ Phase 3: 現State終了 ──────────┐
│  OnExit()                        │
│  OnCleanupAssets()              │ ← アセット解放
│  OnCleanupData()                │ ← （並行）
└─────────────────────────────────┘
              ↓
┌─ Phase 4: 新State開始 ──────────┐
│  OnEnter()                       │ ← View表示
└─────────────────────────────────┘
              ↓
┌─ Phase 5: 遷移完了アニメーション ─┐
│  PlayEnterTransition()           │ ← フェードイン
└─────────────────────────────────┘
```

### BaseState（基底クラス）

```csharp
public abstract class BaseState : IState
{
    private readonly List<IAssetPreloadable> preloadableAssets = new();

    // アセットを登録（コンストラクタで呼び出す）
    protected void RegisterPreloadableAsset(IAssetPreloadable asset)
    {
        preloadableAssets.Add(asset);
    }

    // 自動プリロード
    public async UniTask OnPreloadAssets()
    {
        foreach (var asset in preloadableAssets)
        {
            await asset.PreloadAsync();
        }
        await OnPreloadAssetsAsync(); // 派生クラス用フック
    }

    // 自動クリーンアップ
    public async UniTask OnCleanupAssets()
    {
        await OnCleanupAssetsAsync(); // 派生クラス用フック
        foreach (var asset in preloadableAssets)
        {
            asset.Unload();
        }
    }

    // 派生クラスでオーバーライド可能
    protected virtual async UniTask OnPreloadAssetsAsync() { }
    protected virtual async UniTask OnLoadDataAsync() { }
    protected virtual async UniTask OnCleanupAssetsAsync() { }
    protected virtual async UniTask OnCleanupDataAsync() { }
}
```

### State実装例（HomeState）

```csharp
public class HomeState : BaseState
{
    private readonly IViewFactory<HomeView> viewFactory;
    private readonly IUserProfileRepository userProfileRepository;
    private HomeView view;
    private UserProfile userProfile;

    [Inject]
    public HomeState(
        IViewFactory<HomeView> viewFactory,
        AddressableViewFactory<HomeView> addressableFactory,
        IUserProfileRepository userProfileRepository)
    {
        this.viewFactory = viewFactory;
        this.userProfileRepository = userProfileRepository;
        
        // アセット登録（自動プリロード/クリーンアップ対象）
        RegisterPreloadableAsset(addressableFactory);
    }

    // データロード（オーバーライド）
    protected override async UniTask OnLoadDataAsync()
    {
        userProfile = userProfileRepository.Load();
    }

    // State入場
    public override async UniTask OnEnter()
    {
        view = await viewFactory.CreateAsync(); // 即座に作成
        view.SetPlayerLevel(userProfile.Level);
        await view.Show();
    }

    // State退出
    public override async UniTask OnExit()
    {
        await view.Hide();
        view.Dispose();
        view = null;
    }
}
```

</details>

---

<a id="5-addressables管理"></a>
## 5. Addressables管理

<details>
<summary>📖 クリックして展開</summary>

### アーキテクチャ

```
OutGameInstaller
    ↓ バインド
IAddressableAssetProvider
    ↓ 使用
AddressableViewFactory<TView>
    ↓ ラップ
ViewAsyncFactory<TView>
    ↓ 注入
State (HomeState等)
```

### AddressableAssetKey（定数管理）

```csharp
public static class AddressableAssetKey
{
    public static class Views
    {
        public const string Title = "TitleView";
        public const string Home = "HomeView";
        public const string Settings = "SettingsView";
    }

    // 遷移アニメーション用アセット
    public static class Transition
    {
        public const string LoadingView = "LoadingView"; // ローディング＆フェード兼用
    }
}
```

### IAddressableAssetProvider

```csharp
public interface IAddressableAssetProvider
{
    // プレハブをロード（非インスタンス化）
    UniTask<GameObject> LoadPrefabAsync(string key);
    
    // アセットをアンロード
    void ReleaseAsset(GameObject asset);
}
```

### AddressableViewFactory

```csharp
public class AddressableViewFactory<TView> : IFactory<TView>, IAssetPreloadable
{
    private GameObject cachedPrefab;

    // 事前ロード
    public async UniTask PreloadAsync()
    {
        cachedPrefab = await assetProvider.LoadPrefabAsync(assetKey);
    }

    // インスタンス化（DI対応）
    public TView Create()
    {
        return container.InstantiatePrefabForComponent<TView>(
            cachedPrefab, canvas.transform);
    }

    // アンロード
    public void Unload()
    {
        assetProvider.ReleaseAsset(cachedPrefab);
        cachedPrefab = null;
    }
}
```

### Installer設定

```csharp
// Viewファクトリーをバインド
BindViewFactory<TitleView>(AddressableAssetKey.Views.Title);
BindViewFactory<HomeView>(AddressableAssetKey.Views.Home);
BindViewFactory<SettingsView>(AddressableAssetKey.Views.Settings);

private void BindViewFactory<TView>(string assetKey)
{
    Container.Bind<AddressableViewFactory<TView>>()
        .AsSingle()
        .WithArguments(assetKey);

    Container.Bind<IFactory<TView>>()
        .To<AddressableViewFactory<TView>>()
        .FromResolve();

    Container.Bind<IViewFactory<TView>>()
        .To<ViewAsyncFactory<TView>>()
        .AsSingle();
}
```

</details>

---

<a id="6-プリロードクリーンアップ"></a>
## 6. プリロード/クリーンアップ

<details>
<summary>📖 クリックして展開</summary>

### 設計方針

| | **アセット** | **データ** |
|---|---|---|
| 例 | ViewPrefab, AudioClip | UserProfile, Settings |
| ロード元 | Addressables | Repository, API |
| キャッシュ | 一時的（使用後アンロード） | 持続的 |
| クリーンアップ | 必須 | 通常不要 |

### 自動処理の仕組み

```csharp
// コンストラクタでアセット登録
public HomeState(AddressableViewFactory<HomeView> factory)
{
    RegisterPreloadableAsset(factory); // ← これだけ！
}

// BaseStateが自動処理
public async UniTask OnPreloadAssets()
{
    foreach (var asset in preloadableAssets)
    {
        await asset.PreloadAsync(); // 自動プリロード
    }
}

public async UniTask OnCleanupAssets()
{
    foreach (var asset in preloadableAssets)
    {
        asset.Unload(); // 自動アンロード
    }
}
```

### データロードのオーバーライド

```csharp
// 必要な場合のみオーバーライド
protected override async UniTask OnLoadDataAsync()
{
    userProfile = userProfileRepository.Load();
    await UniTask.CompletedTask;
}
```

### 並行処理による最適化

```csharp
// Phase 2: アセットとデータを並行ロード
await UniTask.WhenAll(
    newState.OnPreloadAssets(),
    newState.OnLoadData()
);

// Phase 3: クリーンアップも並行処理
await UniTask.WhenAll(
    currentState.OnCleanupAssets(),
    currentState.OnCleanupData()
);
```

</details>

---

<a id="7-遷移アニメーション"></a>
## 7. 遷移アニメーション

<details>
<summary>📖 クリックして展開</summary>

### IStateTransitionAnimator

```csharp
public interface IStateTransitionAnimator
{
    // フェードアウト（State退出前）
    UniTask PlayExitTransition();
    
    // フェードイン（State入場後）
    UniTask PlayEnterTransition();
    
    // ローディング表示
    UniTask ShowLoading();
    UniTask HideLoading();
}
```

### FadeTransitionAnimator

Installer経由でLoadingViewファクトリーを注入（設計原則に準拠）

```csharp
public class FadeTransitionAnimator : IStateTransitionAnimator, IDisposable
{
    private readonly IViewFactory<LoadingView> _loadingViewFactory;
    private readonly AddressableViewFactory<LoadingView> _addressableFactory;
    private readonly float _fadeDuration;
    private LoadingView _loadingView;

    [Inject]
    public FadeTransitionAnimator(
        IViewFactory<LoadingView> loadingViewFactory,
        AddressableViewFactory<LoadingView> addressableFactory,
        [InjectOptional] float fadeDuration = 0.3f)
    {
        _loadingViewFactory = loadingViewFactory;
        _addressableFactory = addressableFactory;
        _fadeDuration = fadeDuration;
    }

    public async UniTask InitializeAsync()
    {
        await _addressableFactory.PreloadAsync();
        _loadingView = await _loadingViewFactory.CreateAsync();
    }

    public async UniTask PlayExitTransition()
    {
        _loadingView.HideLoadingElements();
        await _loadingView.FadeIn(_fadeDuration);
    }

    public async UniTask PlayEnterTransition()
    {
        _loadingView.HideLoadingElements();
        await _loadingView.FadeOut(_fadeDuration);
    }

    public async UniTask ShowLoading()
    {
        _loadingView?.ShowLoadingElements();
    }

    public async UniTask HideLoading()
    {
        _loadingView?.HideLoadingElements();
    }
}
```

### LoadingView（フェード＆ローディング兼用）

```csharp
public class LoadingView : MonoBehaviour, IView
{
    [Header("フェード用")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _background;

    [Header("ローディング用")]
    [SerializeField] private GameObject _loadingElements;
    [SerializeField] private Slider _progressBar;

    [Header("スピナー（フレームアニメーション）")]
    [SerializeField] private Image _spinnerImage;
    [SerializeField] private Sprite[] _spinnerFrames; // 複数フレームの画像
    [SerializeField] private float _frameRate = 12f;  // フレーム/秒

    // フェードイン（画面を暗くする）
    public async UniTask FadeIn(float duration);

    // フェードアウト（画面を明るくする）
    public async UniTask FadeOut(float duration);

    // ローディング要素を表示（プログレスバー、フレームアニメーション開始）
    public void ShowLoadingElements();

    // ローディング要素を非表示
    public void HideLoadingElements();

    // プログレスバーの値を設定 (0.0 - 1.0)
    public void SetProgress(float progress);
}
```

### DI設定（Installer）

```csharp
// LoadingViewファクトリーをバインド
BindViewFactory<LoadingView>(AddressableAssetKey.Transition.LoadingView);

// FadeTransitionAnimator（LoadingViewファクトリーを注入）
Container.Bind<IStateTransitionAnimator>()
    .To<FadeTransitionAnimator>()
    .AsSingle();
```

### StateMachineへの自動注入

```csharp
[Inject]
public StateMachine([InjectOptional] IStateTransitionAnimator animator = null)
{
    _transitionAnimator = animator; // nullでも動作
}
```

### カスタムアニメーション例

```csharp
// スライドアニメーション
public class SlideTransitionAnimator : MonoBehaviour, IStateTransitionAnimator
{
    public async UniTask PlayExitTransition()
    {
        await transform.DOLocalMoveX(-1920f, 0.3f).ToUniTask();
    }

    public async UniTask PlayEnterTransition()
    {
        transform.localPosition = new Vector3(1920f, 0, 0);
        await transform.DOLocalMoveX(0f, 0.3f).ToUniTask();
    }
}
```

</details>

---

<a id="8-di依存性注入"></a>
## 8. DI（依存性注入）

<details>
<summary>📖 クリックして展開</summary>

### OutGameInstaller構成

```csharp
public override void InstallBindings()
{
    // ===== 基盤層 =====
    Container.Bind<Canvas>().FromComponentInHierarchy().AsSingle();
    Container.Bind<IAddressableAssetProvider>().To<AddressableAssetProvider>().AsSingle();

    // ===== Viewファクトリー =====
    BindViewFactory<TitleView>(AddressableAssetKey.Views.Title);
    BindViewFactory<HomeView>(AddressableAssetKey.Views.Home);
    BindViewFactory<SettingsView>(AddressableAssetKey.Views.Settings);

    // ===== 遷移アニメーション（Addressablesから取得） =====
    // LoadingViewファクトリーをバインド（フェード＆ローディング兼用）
    BindViewFactory<LoadingView>(AddressableAssetKey.Transition.LoadingView);
    
    // FadeTransitionAnimator（LoadingViewファクトリーを注入）
    Container.Bind<IStateTransitionAnimator>()
        .To<FadeTransitionAnimator>()
        .AsSingle();

    // ===== StateMachine =====
    Container.Bind<StateMachine<OutGameStateKey>>().AsSingle();

    // ===== Repository =====
    Container.Bind<IAudioSettingsRepository>().To<AudioSettingsRepositoryImpl>().AsSingle();
    Container.Bind<IUserProfileRepository>().To<UserProfileRepositoryImpl>().AsSingle();

    // ===== Presenter =====
    Container.BindInterfacesAndSelfTo<VolumePresenter>().AsSingle();
    Container.BindInterfacesAndSelfTo<NavigationPresenter>().AsSingle();

    // ===== UseCase =====
    Container.Bind<IUpdateBgmVolumeInputPort>().To<UpdateBgmVolumeInteractor>().AsSingle();
    Container.Bind<INavigateToHomeInputPort>().To<NavigateToHomeInteractor>().AsSingle();
    // ... その他

    // ===== Controller =====
    Container.Bind<TitleController>().AsSingle();
    Container.Bind<HomeController>().AsSingle();
    Container.Bind<SettingsController>().AsSingle();

    // ===== State =====
    Container.Bind<TitleState>().AsSingle();
    Container.Bind<HomeState>().AsSingle();
    Container.Bind<SettingsState>().AsSingle();

    // ===== Manager =====
    Container.Bind<OutGameManager>()
        .FromComponentInNewPrefab(_outGameManagerPrefab)
        .AsSingle()
        .NonLazy();
}
```

### DIフロー図

```
OutGameInstaller
    │
    ├─ IAddressableAssetProvider
    │       └─ AddressableAssetProvider
    │
    ├─ IStateTransitionAnimator (Optional)
    │       └─ FadeTransitionAnimator
    │
    ├─ AddressableViewFactory<TView>
    │       └─ IViewFactory<TView>
    │               └─ ViewAsyncFactory<TView>
    │
    ├─ StateMachine<OutGameStateKey>
    │       └─ [InjectOptional] IStateTransitionAnimator
    │
    ├─ TitleState / HomeState / SettingsState
    │       ├─ IViewFactory<TView>
    │       ├─ AddressableViewFactory<TView>
    │       ├─ Controller
    │       └─ Repository
    │
    └─ OutGameManager
            └─ StateMachine + States
```

</details>

---

<a id="9-新規画面の追加方法"></a>
## 9. 新規画面の追加方法

<details>
<summary>📖 クリックして展開</summary>

### Step 1: StateKeyに追加

```csharp
// OutGameStateKey.cs
public enum OutGameStateKey
{
    Title,
    Home,
    Settings,
    NewScreen  // ← 追加
}
```

### Step 2: アセットキーを追加

```csharp
// AddressableAssetKey.cs
public static class Views
{
    public const string NewScreen = "NewScreenView";  // ← 追加
}
```

### Step 3: Viewを作成

```csharp
// NewScreenView.cs
public class NewScreenView : BaseView
{
    [SerializeField] private Button _someButton;
    
    public Observable<Unit> OnSomeButtonClicked => 
        _someButton.OnClickAsObservable();
}
```

### Step 4: Controllerを作成

```csharp
// NewScreenController.cs
public class NewScreenController : IDisposable
{
    private CompositeDisposable _disposables = new();

    public void Initialize(NewScreenView view)
    {
        view.OnSomeButtonClicked
            .Subscribe(_ => DoSomething())
            .AddTo(_disposables);
    }

    public void Dispose() => _disposables.Dispose();
}
```

### Step 5: Stateを作成

```csharp
// NewScreenState.cs
public class NewScreenState : BaseState
{
    private readonly IViewFactory<NewScreenView> viewFactory;
    private readonly NewScreenController controller;
    private NewScreenView view;

    [Inject]
    public NewScreenState(
        IViewFactory<NewScreenView> viewFactory,
        AddressableViewFactory<NewScreenView> addressableFactory,
        NewScreenController controller)
    {
        this.viewFactory = viewFactory;
        this.controller = controller;
        RegisterPreloadableAsset(addressableFactory);
    }

    public override async UniTask OnEnter()
    {
        view = await viewFactory.CreateAsync();
        controller.Initialize(view);
        await view.Show();
    }

    public override async UniTask OnExit()
    {
        controller?.Dispose();
        await view.Hide();
        view.Dispose();
        view = null;
    }
}
```

### Step 6: Installerに登録

```csharp
// OutGameInstaller.cs
public override void InstallBindings()
{
    // Viewファクトリー
    BindViewFactory<NewScreenView>(AddressableAssetKey.Views.NewScreen);

    // Controller
    Container.Bind<NewScreenController>().AsSingle();

    // State
    Container.Bind<NewScreenState>().AsSingle();
}
```

### Step 7: OutGameManagerに登録

```csharp
// OutGameManager.cs
[Inject]
public void Construct(
    StateMachine<OutGameStateKey> stateMachine,
    NewScreenState newScreenState)  // ← 追加
{
    _stateMachine.RegisterState(OutGameStateKey.NewScreen, newScreenState);
}
```

### Step 8: Addressablesに登録

1. NewScreenViewプレハブを作成
2. Addressablesグループに追加
3. アドレスを`NewScreenView`に設定

### まとめ

| ファイル | 追加内容 |
|---------|---------|
| OutGameStateKey.cs | Enum値 |
| AddressableAssetKey.cs | アセットキー定数 |
| NewScreenView.cs | View実装 |
| NewScreenController.cs | Controller実装 |
| NewScreenState.cs | State実装 |
| OutGameInstaller.cs | DIバインド |
| OutGameManager.cs | State登録 |
| Addressables | プレハブ登録 |

</details>

---

## 📝 更新履歴

| 日付 | 内容 |
|------|------|
| 2025/12/06 | 初版作成（全ドキュメント統合） |
| 2025/12/06 | 遷移アニメーション機能を追加 |
| 2025/12/06 | プリロード/クリーンアップ機能を追加 |
| 2025/12/06 | Addressables管理システムを追加 |
| 2025/12/06 | LoadingView追加（フェード＆ローディング兼用、Installer主導に変更） |

