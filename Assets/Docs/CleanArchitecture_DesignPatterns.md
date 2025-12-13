# プロジェクトのクリーンアーキテクチャ&デザインパターンまとめ

---

## **1. アーキテクチャ概要**

このプロジェクトは**クリーンアーキテクチャ**を採用したUnityゲーム開発プロジェクトです。

### **採用している技術スタック**

| カテゴリ | 技術 | 用途 |
| --- | --- | --- |
| **アーキテクチャ** | Clean Architecture | 責務分離、依存関係の整理 |
| **状態管理** | State Machine パターン | 画面遷移、ゲーム状態の管理 |
| **DI** | Zenject | 依存性注入、インスタンス管理 |
| **非同期処理** | UniTask | async/await処理、パフォーマンス最適化 |
| **リアクティブ** | R3 | イベント駆動、データバインディング |
| **アセット管理** | Addressables | 動的リソース読み込み、メモリ管理 |
| **アニメーション** | DOTween | UI遷移アニメーション |

---

## **2. クリーンアーキテクチャの4層構造**

プロジェクトは内側から外側に向かって4つの層で構成されています：

```
┌─────────────────────────────────────────────────────────┐
│                 FrameworkAndDriver層                     │
│         (Infrastructure - Unity固有実装)                 │
│         View, Repository実装, Addressables               │
└─────────────────────────────────────────────────────────┘
                        ↑ 依存
┌─────────────────────────────────────────────────────────┐
│                  InterfaceAdapter層                      │
│              (Presentation - データ変換)                 │
│              Controller, Presenter                       │
└─────────────────────────────────────────────────────────┘
                        ↑ 依存
┌─────────────────────────────────────────────────────────┐
│                    UseCase層                            │
│        (Application - ビジネスロジック実行)              │
│         Interactor, InputPort, OutputPort               │
└─────────────────────────────────────────────────────────┘
                        ↑ 依存
┌─────────────────────────────────────────────────────────┐
│                    Entity層                             │
│         (Domain - ビジネスルール中核)                    │
│         Models, Repository Interface                    │
└─────────────────────────────────────────────────────────┘
```

### **依存関係のルール**

- **外側→内側**: 外側の層は内側の層に依存できる
- **内側→外側**: 内側の層は外側の層を知らない（依存しない）
- **インターフェース**: 依存性逆転の原則を活用

---

## **3. 処理フローの理解**

### **基本的な処理の流れ**

```
ユーザー操作
    ↓
【View】イベント発火（Observable）
    ↓
【Controller】入力受付 → データ検証
    ↓
【Interactor】ビジネスロジック実行
    ├→ Repository経由でデータ取得/保存
    └→ Domain Modelを操作
    ↓
【Presenter】結果受信 → 表示用データに変換
    ↓
【View】画面更新
```

### **具体例：BGM音量変更**

```csharp
// 1. View: ユーザーがスライダーを動かす
bgmSlider.OnValueChangedAsObservable()
    .Subscribe(volume => OnBgmVolumeChanged?.Invoke(volume));

// 2. Controller: 入力を受け取る
public void HandleBgmVolumeChange(float volume)
{
    updateBgmVolumeInputPort.Execute(volume);
}

// 3. Interactor: ビジネスロジック実行
public async UniTask Execute(float volume)
{
    var settings = await repository.Load();
    settings.UpdateBgmVolume(volume);  // Entityのメソッド
    await repository.Save(settings);
    outputPort.OnVolumeUpdated(volume);
}

// 4. Presenter: 表示用に変換
public void OnVolumeUpdated(float volume)
{
    view.UpdateVolumeDisplay($"{(int)(volume * 100)}%");
}

// 5. View: 画面更新
public void UpdateVolumeDisplay(string text)
{
    volumeLabel.text = text;
}
```

---

## **4. 各層の詳細**

### **📦 Entity層（Domain）**

**役割**: ビジネスルールの中核、ドメイン知識

**特徴**:
- 他の層に依存しない
- 純粋なC#クラス
- Unity非依存

**含まれるもの**:
- Models（エンティティ、値オブジェクト）
- Repository Interface
- Domain Service

**例**:

```csharp
// ドメインモデル
public class AudioSettings
{
    public float BgmVolume { get; private set; }
    public float SeVolume { get; private set; }

    public void UpdateBgmVolume(float volume)
    {
        // ビジネスルール：音量は0.0〜1.0の範囲
        if (volume < 0f || volume > 1f)
            throw new ArgumentOutOfRangeException(nameof(volume));
        
        BgmVolume = volume;
    }
}

// リポジトリインターフェース（実装は外側の層）
public interface IAudioSettingsRepository
{
    UniTask<AudioSettings> Load();
    UniTask Save(AudioSettings settings);
}
```

---

### **📦 UseCase層（Application）**

**役割**: アプリケーション固有のビジネスロジック

**特徴**:
- Entityを操作
- Repositoryを通じてデータ永続化
- Presenterに結果を通知
- Unity非依存

**含まれるもの**:
- Interactor（UseCaseの実装）
- InputPort（Interactorのインターフェース）
- OutputPort（Presenterのインターフェース）

**例**:

```csharp
// InputPort（Controllerが呼び出す）
public interface IUpdateBgmVolumeInputPort
{
    UniTask Execute(float volume);
}

// OutputPort（Presenterが実装する）
public interface IVolumeUpdateOutputPort
{
    void OnVolumeUpdated(float volume);
    void OnError(string errorMessage);
}

// Interactor（実装）
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    private readonly IAudioSettingsRepository repository;
    private readonly IVolumeUpdateOutputPort outputPort;

    public UpdateBgmVolumeInteractor(
        IAudioSettingsRepository repository,
        IVolumeUpdateOutputPort outputPort)
    {
        this.repository = repository;
        this.outputPort = outputPort;
    }

    public async UniTask Execute(float volume)
    {
        try
        {
            var settings = await repository.Load();
            settings.UpdateBgmVolume(volume);
            await repository.Save(settings);
            
            outputPort.OnVolumeUpdated(volume);
        }
        catch (Exception ex)
        {
            outputPort.OnError(ex.Message);
        }
    }
}
```

---

### **📦 InterfaceAdapter層（Presentation）**

**役割**: データ変換、入出力の制御

**特徴**:
- Controllerは「入力の窓口」
- Presenterは「出力の窓口」
- Unity非依存（基本的には）

**含まれるもの**:
- Controller（Viewからの入力を受け取る）
- Presenter（結果を表示用に変換）

**例**:

```csharp
// Controller
public class SettingsController
{
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeInputPort;

    public SettingsController(IUpdateBgmVolumeInputPort updateBgmVolumeInputPort)
    {
        this.updateBgmVolumeInputPort = updateBgmVolumeInputPort;
    }

    public void HandleBgmVolumeChange(float volume)
    {
        // データ検証
        volume = Mathf.Clamp01(volume);
        
        // UseCaseに委譲
        updateBgmVolumeInputPort.Execute(volume).Forget();
    }
}

// Presenter
public class VolumePresenter : IVolumeUpdateOutputPort
{
    private SettingsView view;

    public void SetView(SettingsView view)
    {
        this.view = view;
    }

    public void OnVolumeUpdated(float volume)
    {
        // ビジネスデータを表示用に変換
        var displayText = $"BGM: {(int)(volume * 100)}%";
        view.UpdateVolumeDisplay(displayText);
    }

    public void OnError(string errorMessage)
    {
        view.ShowError(errorMessage);
    }
}
```

---

### **📦 FrameworkAndDriver層（Infrastructure）**

**役割**: Unity固有の実装、外部ライブラリとの連携

**特徴**:
- Unity依存
- MonoBehaviourを使用
- 具体的な実装（PlayerPrefs、Addressablesなど）

**含まれるもの**:
- View（UI表示、ユーザー入力）
- Repository実装（データ永続化）
- Addressables管理
- Factory実装

**例**:

```csharp
// View
public class SettingsView : MonoBehaviour, IView
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI volumeLabel;

    public Observable<float> OnBgmVolumeChanged => 
        bgmSlider.OnValueChangedAsObservable();

    public void UpdateVolumeDisplay(string text)
    {
        volumeLabel.text = text;
    }

    public void ShowError(string message)
    {
        Debug.LogError(message);
    }
}

// Repository実装
public class AudioSettingsRepositoryImpl : IAudioSettingsRepository
{
    private const string BGM_VOLUME_KEY = "BgmVolume";
    private const string SE_VOLUME_KEY = "SeVolume";

    public UniTask<AudioSettings> Load()
    {
        var bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        var seVolume = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 1f);
        
        var settings = new AudioSettings(bgmVolume, seVolume);
        return UniTask.FromResult(settings);
    }

    public UniTask Save(AudioSettings settings)
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, settings.BgmVolume);
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, settings.SeVolume);
        PlayerPrefs.Save();
        
        return UniTask.CompletedTask;
    }
}
```

---

## **5. デザインパターン**

このプロジェクトで採用されている主要なデザインパターンを説明します。

---

### **🎯 State Machineパターン**

**目的**: 画面遷移やゲーム状態を管理

**構造**:

```
StateMachine<TStateKey>
    ├── RegisterState()     ステート登録
    ├── ChangeState()       ステート遷移
    ├── GoBack()           前のステートに戻る
    └── Update()           現在のステートを更新

IState
    ├── OnPreloadAssets()   アセット読み込み
    ├── OnLoadData()        データ読み込み
    ├── OnEnter()          ステート入場
    ├── OnUpdate()         毎フレーム更新
    ├── OnExit()           ステート退出
    └── OnCleanup()        アセット解放
```

**実装例**:

```csharp
// ステート定義
public enum OutGameStateKey
{
    Title,
    Home,
    Settings
}

// ステートマシーン
public class StateMachine<TStateKey> where TStateKey : Enum
{
    private readonly Dictionary<TStateKey, IState> _states = new();
    private readonly Stack<TStateKey> _stateHistory = new();
    private IState _currentState;
    private TStateKey _currentStateKey;

    public async UniTask ChangeState(TStateKey newStateKey, bool addToHistory = true)
    {
        var newState = _states[newStateKey];

        // Phase 1: 遷移アニメーション
        await PlayExitAnimation();

        // Phase 2: アセット/データ読み込み
        await UniTask.WhenAll(
            newState.OnPreloadAssets(),
            newState.OnLoadData()
        );

        // Phase 3: 現ステート終了
        if (_currentState != null)
        {
            await _currentState.OnExit();
            await _currentState.OnCleanup();
        }

        // Phase 4: 新ステート開始
        _currentState = newState;
        _currentStateKey = newStateKey;
        await _currentState.OnEnter();

        // Phase 5: 入場アニメーション
        await PlayEnterAnimation();

        // 履歴管理
        if (addToHistory)
        {
            _stateHistory.Push(newStateKey);
        }
    }

    public async UniTask GoBack()
    {
        if (_stateHistory.Count > 0)
        {
            var previousState = _stateHistory.Pop();
            await ChangeState(previousState, addToHistory: false);
        }
    }
}

// 個別のステート実装
public class HomeState : BaseState
{
    private readonly HomeController controller;
    private readonly IFactory<HomeView> viewFactory;
    private HomeView view;

    public override async UniTask OnPreloadAssets()
    {
        // Addressablesでアセット読み込み
        await viewFactory.PreloadAsync();
    }

    public override async UniTask OnEnter()
    {
        // View生成、Controller初期化
        view = viewFactory.Create();
        controller.Initialize(view);
    }

    public override async UniTask OnExit()
    {
        // Viewを破棄
        Object.Destroy(view.gameObject);
    }

    public override async UniTask OnCleanup()
    {
        // アセット解放
        await viewFactory.ReleaseAsync();
    }
}
```

**ポイント**:
- ステートごとにライフサイクルを管理
- 履歴管理で「戻る」ボタンに対応
- アセット読み込み/解放を自動化

---

### **🎯 Dependency Injection (DI) パターン**

**目的**: 依存関係の注入、疎結合化、テスタビリティ向上

**使用ライブラリ**: Zenject

**Installerでの設定例**:

```csharp
public class OutGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // ===== Repository実装 =====
        Container.Bind<IAudioSettingsRepository>()
            .To<AudioSettingsRepositoryImpl>()
            .AsSingle();

        Container.Bind<IUserProfileRepository>()
            .To<UserProfileRepositoryImpl>()
            .AsSingle();

        // ===== Interactor → InputPort =====
        Container.Bind<IUpdateBgmVolumeInputPort>()
            .To<UpdateBgmVolumeInteractor>()
            .AsSingle();

        Container.Bind<INavigateToHomeInputPort>()
            .To<NavigateToHomeInteractor>()
            .AsSingle();

        // ===== Presenter → OutputPort =====
        Container.Bind<IVolumeUpdateOutputPort>()
            .To<VolumePresenter>()
            .AsSingle();

        Container.Bind<INavigationOutputPort>()
            .To<NavigationPresenter>()
            .AsSingle();

        // ===== Controller =====
        Container.Bind<HomeController>().AsSingle();
        Container.Bind<SettingsController>().AsSingle();

        // ===== View Factory =====
        Container.Bind<IFactory<HomeView>>()
            .To<AddressableViewFactory<HomeView>>()
            .AsSingle()
            .WithArguments("HomeView");

        // ===== State =====
        Container.Bind<HomeState>().AsSingle();
        Container.Bind<SettingsState>().AsSingle();

        // ===== StateMachine =====
        Container.Bind<StateMachine<OutGameStateKey>>()
            .AsSingle();
    }
}
```

**コンストラクタインジェクション**:

```csharp
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    private readonly IAudioSettingsRepository repository;
    private readonly IVolumeUpdateOutputPort outputPort;

    // Zenjectが自動的に依存を注入
    [Inject]
    public UpdateBgmVolumeInteractor(
        IAudioSettingsRepository repository,
        IVolumeUpdateOutputPort outputPort)
    {
        this.repository = repository;
        this.outputPort = outputPort;
    }
}
```

**ポイント**:
- インターフェースに対してプログラミング
- テスト時にモックと差し替え可能
- 依存関係が明示的

---

### **🎯 Repositoryパターン**

**目的**: データアクセスの抽象化

**構造**:

```
IRepository (Interface - Domain層)
    ↑ 実装
RepositoryImpl (Implementation - Infrastructure層)
    ├→ PlayerPrefs
    ├→ JSON File
    └→ Server API
```

**例**:

```csharp
// インターフェース（Domain層）
public interface IUserProfileRepository
{
    UniTask<UserProfile> Load();
    UniTask Save(UserProfile profile);
}

// 実装（Infrastructure層）
public class UserProfileRepositoryImpl : IUserProfileRepository
{
    private const string SAVE_KEY = "UserProfile";

    public async UniTask<UserProfile> Load()
    {
        var json = PlayerPrefs.GetString(SAVE_KEY, "{}");
        var profile = JsonUtility.FromJson<UserProfile>(json);
        return profile ?? UserProfile.CreateDefault();
    }

    public async UniTask Save(UserProfile profile)
    {
        var json = JsonUtility.ToJson(profile);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }
}
```

**メリット**:
- データソースを変更してもドメイン層に影響なし
- テスト時にインメモリ実装と差し替え可能

---

### **🎯 Factoryパターン**

**目的**: オブジェクト生成の抽象化

**種類**:

#### **1. Zenject Factory（シンプル）**

```csharp
// Entityファクトリー（ドメインモデル生成）
public class Player
{
    public class Factory : PlaceholderFactory<string, int, CombatStats, Player>
    {
    }
}

// Installerでバインド
Container.BindFactory<string, int, CombatStats, Player, Player.Factory>()
    .FromNew();

// 使用例
public class StartBattleInteractor
{
    private readonly Player.Factory playerFactory;

    public void CreatePlayer()
    {
        var player = playerFactory.Create("Player1", 10, stats);
    }
}
```

#### **2. カスタムFactory（複雑な生成ロジック）**

```csharp
// Infrastructure層のFactory
public class EnemyFactory : IFactory<EnemyData, Enemy>
{
    private readonly Enemy.Factory enemyDomainFactory;

    [Inject]
    public EnemyFactory(Enemy.Factory enemyDomainFactory)
    {
        this.enemyDomainFactory = enemyDomainFactory;
    }

    public Enemy Create(EnemyData data)
    {
        // データから変換
        var stats = new CombatStats(
            data.maxHp,
            data.maxHp,
            data.attackPower,
            data.defense,
            data.attackSpeed,
            data.moveSpeed,
            0f
        );

        var reward = new BattleReward(
            data.goldReward,
            data.expReward
        );

        return enemyDomainFactory.Create(
            data.enemyId,
            data.enemyType,
            stats,
            reward
        );
    }
}
```

#### **3. Addressable View Factory（プリロード対応）**

```csharp
// IAssetPreloadableを実装し、事前読み込みをサポート
public class AddressableViewFactory<TView> : IFactory<TView>, IAssetPreloadable
    where TView : Component, IView
{
    private readonly DiContainer container;
    private readonly IAddressableAssetProvider assetProvider;
    private readonly Canvas canvas;
    private readonly string assetKey;
    private GameObject cachedPrefab;

    public async UniTask PreloadAsync()
    {
        // アセット事前読み込み
        cachedPrefab = await assetProvider.LoadAssetAsync<GameObject>(assetKey);
    }

    public TView Create()
    {
        if (cachedPrefab == null)
            throw new InvalidOperationException("Prefab not preloaded");

        // ZenjectのDI対応インスタンス化
        return container.InstantiatePrefabForComponent<TView>(
            cachedPrefab,
            canvas.transform
        );
    }

    public async UniTask ReleaseAsync()
    {
        // アセット解放
        await assetProvider.ReleaseAsset(assetKey);
        cachedPrefab = null;
    }
}
```

**ポイント**:
- 生成ロジックの一元管理
- Zenjectとの統合
- プリロード/リリース管理

---

### **🎯 Observerパターン（Reactive Extensions - R3）**

**目的**: イベント駆動プログラミング、データバインディング

**例**:

```csharp
// Viewでイベントを公開
public class SettingsView : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Button homeButton;

    // Observable<float>として公開
    public Observable<float> OnBgmVolumeChanged =>
        bgmSlider.OnValueChangedAsObservable();

    // Observable<Unit>として公開
    public Observable<Unit> OnHomeButtonClicked =>
        homeButton.OnClickAsObservable();
}

// Controllerで購読
public class SettingsController
{
    private SettingsView view;
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeInputPort;
    private readonly CompositeDisposable disposables = new();

    public void Initialize(SettingsView view)
    {
        this.view = view;

        // イベント購読
        view.OnBgmVolumeChanged
            .Subscribe(volume => HandleBgmVolumeChange(volume))
            .AddTo(disposables);

        view.OnHomeButtonClicked
            .Subscribe(_ => HandleHomeButtonClick())
            .AddTo(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}
```

**R3の主要機能**:

```csharp
// Throttle（連続入力を間引く）
view.OnSearchTextChanged
    .Throttle(TimeSpan.FromMilliseconds(300))
    .Subscribe(text => Search(text));

// DistinctUntilChanged（値が変わったときだけ）
view.OnToggleChanged
    .DistinctUntilChanged()
    .Subscribe(isOn => UpdateSetting(isOn));

// WhereAwait（非同期フィルタ）
Observable.Interval(TimeSpan.FromSeconds(1))
    .WhereAwait(async (x, ct) =>
    {
        return await ValidateAsync(x, ct);
    }, AwaitOperation.Sequential)
    .Subscribe(x => Process(x));

// SelectAwait（非同期変換）
view.OnButtonClicked
    .SelectAwait(async (_, ct) =>
    {
        return await FetchDataAsync(ct);
    }, AwaitOperation.Switch)  // 連打時は前の処理をキャンセル
    .Subscribe(data => DisplayData(data));
```

**AwaitOperationの種類**:

| AwaitOperation | 挙動 | 用途 |
| --- | --- | --- |
| **Sequential** | 順番に処理、キューに積む | 順序保証が必要な場合 |
| **Drop** | 実行中は新イベントを無視 | 連打防止 |
| **Switch** | 実行中の処理をキャンセル | 検索入力など最新値のみ処理 |
| **Parallel** | 並列実行、終了順で出力 | 並列可能な処理 |
| **ThrottleFirstLast** | 最新値1つだけ保持 | 負荷分散 |

---

### **🎯 Command/Interactorパターン**

**目的**: ビジネスロジックをカプセル化

**構造**:

```
IInputPort (Command Interface)
    ↓ 実装
Interactor (Command Implementation)
    ├→ Repository経由でデータ操作
    ├→ Domain Modelでビジネスロジック実行
    └→ OutputPort経由で結果通知
```

**例**:

```csharp
// InputPort
public interface INavigateToHomeInputPort
{
    UniTask Execute();
}

// Interactor
public class NavigateToHomeInteractor : INavigateToHomeInputPort
{
    private readonly IUserProfileRepository repository;
    private readonly INavigationOutputPort outputPort;
    private readonly StateMachine<OutGameStateKey> stateMachine;

    public async UniTask Execute()
    {
        // データ保存
        var profile = await repository.Load();
        profile.UpdateLastAccessTime(DateTime.Now);
        await repository.Save(profile);

        // 画面遷移
        await stateMachine.ChangeState(OutGameStateKey.Home);

        // 結果通知
        outputPort.OnNavigationCompleted(OutGameStateKey.Home);
    }
}
```

**ポイント**:
- 1つのInteractorは1つのユースケースを実装
- ビジネスロジックがInteractorに集約
- テストしやすい

---

## **6. フォルダ構成**

```
OutGame/
├── Application/                    # UseCase層
│   ├── Ports/
│   │   ├── Input/                 # InputPort
│   │   │   └── IUpdateBgmVolumeInputPort.cs
│   │   └── Output/                # OutputPort
│   │       └── IVolumeUpdateOutputPort.cs
│   └── UseCases/                  # Interactor
│       └── UpdateBgmVolumeInteractor.cs
│
├── Domain/                         # Entity層
│   ├── Models/                    # エンティティ
│   │   └── AudioSettings.cs
│   └── Repositories/              # Repositoryインターフェース
│       └── IAudioSettingsRepository.cs
│
├── Presentation/                   # InterfaceAdapter層
│   ├── Controllers/               # Controller
│   │   └── SettingsController.cs
│   └── Presenters/                # Presenter
│       └── VolumePresenter.cs
│
├── Infrastructure/                 # FrameworkAndDriver層
│   ├── Addressables/              # Addressables管理
│   │   ├── IAddressableAssetProvider.cs
│   │   └── AddressableAssetProvider.cs
│   ├── Factories/                 # Factory
│   │   ├── AddressableViewFactory.cs
│   │   └── IAssetPreloadable.cs
│   ├── Repositories/              # Repository実装
│   │   └── AudioSettingsRepositoryImpl.cs
│   └── Views/                     # View
│       ├── Base/
│       │   └── IView.cs
│       └── SettingsView.cs
│
├── StateMachine/                   # ステートマシーン
│   ├── IState.cs
│   └── StateMachine.cs
│
├── States/                         # 各画面のState
│   └── SettingsState.cs
│
└── Installers/                     # DI設定
    └── OutGameInstaller.cs
```

---

## **7. 実装時のベストプラクティス**

### **✅ DO（推奨）**

1. **Interactorにビジネスロジックを集約する**
   - 条件分岐、計算、データ操作はすべてInteractorで
   - ControllerやPresenterは薄く保つ

2. **インターフェースに対してプログラミング**
   - 具象クラスではなくインターフェースに依存
   - テスタビリティ向上

3. **Stateでライフサイクルを管理**
   - OnPreloadAssets → OnEnter → OnExit → OnCleanup
   - アセット読み込み/解放を自動化

4. **Observableで非同期イベントを扱う**
   - R3のリアクティブプログラミング
   - メモリリークに注意（AddTo で自動破棄）

5. **Zenject Factoryでオブジェクト生成を一元管理**
   - newを直接使わない
   - DIコンテナ経由で生成

### **❌ DON'T（非推奨）**

1. **Viewにビジネスロジックを書かない**
   - Viewは表示とイベント発火のみ
   - ロジックはInteractorへ

2. **Presenterで状態を持たない**
   - Presenterはステートレス
   - データはドメインモデルで管理

3. **内側の層から外側の層に依存しない**
   - Entity層はUnityを知らない
   - UseCase層もUnity非依存

4. **Installerで複雑なロジックを書かない**
   - Installerはバインド設定のみ
   - 初期化ロジックは別クラスへ

---

## **8. 新規画面の追加手順**

新しい画面を追加する際の標準的な手順です：

### **Step 1: StateKeyを追加**

```csharp
public enum OutGameStateKey
{
    Title,
    Home,
    Settings,
    Shop  // ← 追加
}
```

### **Step 2: Domain層を作成**

```csharp
// Models/ShopItem.cs
public class ShopItem
{
    public string Id { get; }
    public string Name { get; }
    public int Price { get; }

    public ShopItem(string id, string name, int price)
    {
        Id = id;
        Name = name;
        Price = price;
    }
}

// Repositories/IShopRepository.cs
public interface IShopRepository
{
    UniTask<List<ShopItem>> GetItems();
    UniTask<bool> Purchase(string itemId);
}
```

### **Step 3: Application層を作成**

```csharp
// Ports/Input/IPurchaseItemInputPort.cs
public interface IPurchaseItemInputPort
{
    UniTask Execute(string itemId);
}

// Ports/Output/IShopOutputPort.cs
public interface IShopOutputPort
{
    void OnPurchaseSuccess(ShopItem item);
    void OnPurchaseFailed(string reason);
}

// UseCases/PurchaseItemInteractor.cs
public class PurchaseItemInteractor : IPurchaseItemInputPort
{
    private readonly IShopRepository shopRepository;
    private readonly IUserProfileRepository userProfileRepository;
    private readonly IShopOutputPort outputPort;

    public async UniTask Execute(string itemId)
    {
        var item = await shopRepository.GetItemById(itemId);
        var profile = await userProfileRepository.Load();

        if (profile.Gold < item.Price)
        {
            outputPort.OnPurchaseFailed("ゴールドが足りません");
            return;
        }

        profile.ConsumeGold(item.Price);
        await userProfileRepository.Save(profile);
        await shopRepository.Purchase(itemId);

        outputPort.OnPurchaseSuccess(item);
    }
}
```

### **Step 4: Presentation層を作成**

```csharp
// Controllers/ShopController.cs
public class ShopController
{
    private readonly IPurchaseItemInputPort purchaseItemInputPort;

    public void HandlePurchaseButton(string itemId)
    {
        purchaseItemInputPort.Execute(itemId).Forget();
    }
}

// Presenters/ShopPresenter.cs
public class ShopPresenter : IShopOutputPort
{
    private ShopView view;

    public void SetView(ShopView view) => this.view = view;

    public void OnPurchaseSuccess(ShopItem item)
    {
        view.ShowSuccessMessage($"{item.Name}を購入しました");
    }

    public void OnPurchaseFailed(string reason)
    {
        view.ShowErrorMessage(reason);
    }
}
```

### **Step 5: Infrastructure層を作成**

```csharp
// Views/ShopView.cs
public class ShopView : MonoBehaviour, IView
{
    [SerializeField] private Button[] itemButtons;

    public Observable<string> OnItemButtonClicked { get; private set; }

    private void Awake()
    {
        // ボタンクリックをObservableに変換
        OnItemButtonClicked = itemButtons
            .Select((btn, index) => btn.OnClickAsObservable()
                .Select(_ => $"item_{index}"))
            .Merge();
    }

    public void ShowSuccessMessage(string message)
    {
        Debug.Log(message);
    }

    public void ShowErrorMessage(string message)
    {
        Debug.LogError(message);
    }
}

// Repositories/ShopRepositoryImpl.cs
public class ShopRepositoryImpl : IShopRepository
{
    public async UniTask<List<ShopItem>> GetItems()
    {
        // JSONから読み込む、またはサーバーから取得
        return new List<ShopItem>();
    }

    public async UniTask<bool> Purchase(string itemId)
    {
        // 購入処理
        return true;
    }
}
```

### **Step 6: Stateを作成**

```csharp
// States/ShopState.cs
public class ShopState : BaseState
{
    private readonly ShopController controller;
    private readonly ShopPresenter presenter;
    private readonly IFactory<ShopView> viewFactory;
    private ShopView view;

    public ShopState(
        ShopController controller,
        ShopPresenter presenter,
        IFactory<ShopView> viewFactory)
    {
        this.controller = controller;
        this.presenter = presenter;
        this.viewFactory = viewFactory;
    }

    public override async UniTask OnPreloadAssets()
    {
        if (viewFactory is IAssetPreloadable preloadable)
        {
            await preloadable.PreloadAsync();
        }
    }

    public override async UniTask OnEnter()
    {
        view = viewFactory.Create();
        presenter.SetView(view);
        controller.Initialize(view);
    }

    public override async UniTask OnExit()
    {
        controller.Dispose();
        Object.Destroy(view.gameObject);
    }

    public override async UniTask OnCleanup()
    {
        if (viewFactory is IAssetPreloadable preloadable)
        {
            await preloadable.ReleaseAsync();
        }
    }
}
```

### **Step 7: Installerでバインド**

```csharp
public class OutGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // ... 既存のバインド ...

        // Shop - Repository
        Container.Bind<IShopRepository>()
            .To<ShopRepositoryImpl>()
            .AsSingle();

        // Shop - UseCase
        Container.Bind<IPurchaseItemInputPort>()
            .To<PurchaseItemInteractor>()
            .AsSingle();

        // Shop - Presenter
        Container.Bind<IShopOutputPort>()
            .To<ShopPresenter>()
            .AsSingle();
        Container.Bind<ShopPresenter>()
            .FromResolve();

        // Shop - Controller
        Container.Bind<ShopController>()
            .AsSingle();

        // Shop - View Factory
        Container.Bind<IFactory<ShopView>>()
            .To<AddressableViewFactory<ShopView>>()
            .AsSingle()
            .WithArguments("ShopView");

        // Shop - State
        Container.Bind<ShopState>()
            .AsSingle();
    }
}
```

### **Step 8: StateMachineに登録**

```csharp
public class OutGameManager : MonoBehaviour
{
    private ShopState _shopState;

    [Inject]
    public void Construct(
        StateMachine<OutGameStateKey> stateMachine,
        // ... 他のState ...
        ShopState shopState)
    {
        _shopState = shopState;
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        // ... 他のState登録 ...
        _stateMachine.RegisterState(OutGameStateKey.Shop, _shopState);
    }
}
```

---

## **9. テスト戦略**

### **単体テスト（Interactor）**

```csharp
[Test]
public async Task UpdateBgmVolume_ValidVolume_Success()
{
    // Arrange
    var mockRepository = new MockAudioSettingsRepository();
    var mockOutputPort = new MockVolumeUpdateOutputPort();
    var interactor = new UpdateBgmVolumeInteractor(
        mockRepository,
        mockOutputPort
    );

    // Act
    await interactor.Execute(0.5f);

    // Assert
    Assert.AreEqual(0.5f, mockRepository.SavedSettings.BgmVolume);
    Assert.IsTrue(mockOutputPort.WasSuccessCalled);
}

[Test]
public async Task UpdateBgmVolume_InvalidVolume_Error()
{
    // Arrange
    var mockRepository = new MockAudioSettingsRepository();
    var mockOutputPort = new MockVolumeUpdateOutputPort();
    var interactor = new UpdateBgmVolumeInteractor(
        mockRepository,
        mockOutputPort
    );

    // Act & Assert
    Assert.ThrowsAsync<ArgumentOutOfRangeException>(
        async () => await interactor.Execute(1.5f)
    );
}
```

### **統合テスト（State）**

```csharp
[UnityTest]
public IEnumerator HomeState_EnterAndExit_Success()
{
    // Arrange
    var container = CreateTestContainer();
    var stateMachine = container.Resolve<StateMachine<OutGameStateKey>>();

    // Act
    yield return stateMachine.ChangeState(OutGameStateKey.Home).ToCoroutine();

    // Assert
    Assert.AreEqual(OutGameStateKey.Home, stateMachine.CurrentStateKey);
    var homeView = GameObject.FindObjectOfType<HomeView>();
    Assert.IsNotNull(homeView);

    // Cleanup
    yield return stateMachine.ChangeState(OutGameStateKey.Title).ToCoroutine();
}
```

---

## **10. パフォーマンス最適化**

### **Addressablesの活用**

```csharp
// プリロード/リリースを明示的に管理
public class AddressableViewFactory<TView> : IAssetPreloadable
{
    private GameObject cachedPrefab;

    public async UniTask PreloadAsync()
    {
        // State開始前に読み込み
        cachedPrefab = await assetProvider.LoadAssetAsync<GameObject>(assetKey);
    }

    public TView Create()
    {
        // 既にロード済みなので即座に生成
        return container.InstantiatePrefabForComponent<TView>(cachedPrefab);
    }

    public async UniTask ReleaseAsync()
    {
        // State終了後に解放
        await assetProvider.ReleaseAsset(assetKey);
        cachedPrefab = null;
    }
}
```

### **Object Pooling（将来の拡張）**

```csharp
// Zenject Memory Poolを活用
Container.BindMemoryPool<EnemyView, EnemyView.Pool>()
    .WithInitialSize(10)
    .FromComponentInNewPrefab(enemyViewPrefab)
    .UnderTransform(enemyContainer);

// 使用例
public class EnemySpawner
{
    private readonly EnemyView.Pool enemyViewPool;

    public EnemyView SpawnEnemy()
    {
        return enemyViewPool.Spawn();
    }

    public void DespawnEnemy(EnemyView enemyView)
    {
        enemyViewPool.Despawn(enemyView);
    }
}
```

---

## **11. まとめ**

### **クリーンアーキテクチャのメリット**

✅ **テスタビリティ**: Interactorは純粋なC#クラスなので単体テストが容易  
✅ **保守性**: 各層の責務が明確で、変更の影響範囲が限定的  
✅ **拡張性**: 新機能追加時に既存コードへの影響が最小限  
✅ **再利用性**: ビジネスロジックは他プラットフォームでも利用可能  
✅ **依存関係の整理**: 内側→外側の一方向依存で循環参照を防止

### **デザインパターンの効果**

🎯 **State Machine**: 画面遷移のライフサイクルを自動管理、履歴機能  
🎯 **Dependency Injection**: 疎結合化、テストしやすい設計  
🎯 **Repository**: データソースの抽象化、変更に強い  
🎯 **Factory**: オブジェクト生成の一元管理、DI統合  
🎯 **Observer (R3)**: イベント駆動、リアクティブプログラミング  
🎯 **Command (Interactor)**: ビジネスロジックのカプセル化

### **重要な原則**

1. **Controller → Interactor → Presenter** の処理フロー
2. **外側→内側** の依存関係の方向
3. **インターフェース** に対するプログラミング
4. **Interactor** にビジネスロジックを集約
5. **State** でライフサイクルを管理

---

このアーキテクチャにより、**長期的な保守性**と**チーム開発での拡張性**を実現しています。

