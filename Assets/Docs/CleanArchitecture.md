# Unityゲーム開発におけるクリーンアーキテクチャ完全ガイド

> このプロジェクトで採用しているクリーンアーキテクチャとデザインパターンの包括的なドキュメントです。

---

## 📚 目次

1. [クリーンアーキテクチャの基本概念](#クリーンアーキテクチャの基本概念)
2. [処理フローの理解](#処理フローの理解)
3. [各コンポーネントの詳細](#各コンポーネントの詳細)
4. [4層構造の詳細](#4層構造の詳細)
5. [デザインパターン](#デザインパターン)
6. [クラス設計とアーキテクチャ構成](#クラス設計とアーキテクチャ構成)
7. [実装時のベストプラクティス](#実装時のベストプラクティス)
8. [新規画面の追加手順](#新規画面の追加手順)

---

## クリーンアーキテクチャの基本概念

### 最も重要なポイント

クリーンアーキテクチャを理解する上で最も重要なのは、**処理の流れ**です。

**基本の流れ**:
```
Controller → Interactor → Presenter
```

**StateMachineパターンとの統合**:
```
State (画面遷移管理)
  ↓
Controller (入力の窓口)
  ↓
Interactor (UseCase実装)
  ↓
Presenter (出力の窓口)
  ↓
View (UI表示)
```

この単純な流れが、クリーンアーキテクチャの本質を表しています。

### 4つの層構造

内側から外側に向かって以下の4層で構成されます:

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

**依存関係のルール**: 
- 外側の層は内側の層に依存できる
- 内側の層は外側の層を知らない（依存しない）

---

## 処理フローの理解

### 基本的な処理の流れ

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

### 具体例：BGM音量変更

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

## 各コンポーネントの詳細

### 1. View（ビュー）

**役割**:
- ユーザーの入力を受け取る
- 画面に表示する
- Unity固有の実装（MonoBehaviour）

**やること**:
- ボタンクリック、スライダー変更などのイベントを検知
- イベントを`Observable`として公開
- Presenterから指示された内容を画面に表示

**やらないこと**:
- ビジネスロジック
- データの加工
- 他の画面への遷移判断

**コード例**:

```csharp
using UnityEngine;
using UnityEngine.UI;
using R3;
using Cysharp.Threading.Tasks;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Text bgmVolumeText;
    [SerializeField] private Button backButton;
    [SerializeField] private CanvasGroup canvasGroup;

    // イベントをObservableとして公開
    public Observable<float> OnBgmVolumeChanged => 
        bgmSlider.OnValueChangedAsObservable();
    
    public Observable<Unit> OnBackButtonClicked => 
        backButton.OnClickAsObservable();

    // Presenterから呼ばれる：BGM音量を画面に表示
    public void SetBgmVolume(float volume)
    {
        bgmSlider.SetValueWithoutNotify(volume);
        bgmVolumeText.text = $"BGM: {Mathf.RoundToInt(volume * 100)}%";
    }

    public async UniTask Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        await canvasGroup.DOFade(1f, 0.3f);
    }

    public async UniTask Hide()
    {
        await canvasGroup.DOFade(0f, 0.3f);
        gameObject.SetActive(false);
    }
}
```

---

### 2. Controller（コントローラー）

**役割**:
- Viewからの入力を受け取る
- 入力データを加工する
- 適切なUseCaseを呼び出す

**やること**:
- Viewのイベントを購読（Subscribe）
- 入力データの検証・変換
- 複数のViewからの入力を統合
- UseCaseの呼び出し

**やらないこと**:
- ビジネスロジック（それはInteractorの仕事）
- 画面表示（それはPresenterとViewの仕事）

**コード例**:

```csharp
using R3;
using Zenject;

public class SettingsController
{
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeUseCase;
    private readonly INavigateBackInputPort navigateBackUseCase;
    private readonly CompositeDisposable disposables = new();

    [Inject]
    public SettingsController(
        IUpdateBgmVolumeInputPort updateBgmVolumeUseCase,
        INavigateBackInputPort navigateBackUseCase)
    {
        this.updateBgmVolumeUseCase = updateBgmVolumeUseCase;
        this.navigateBackUseCase = navigateBackUseCase;
    }

    public void Initialize(SettingsView view)
    {
        // BGM音量変更イベントを購読
        view.OnBgmVolumeChanged
            .Subscribe(volume => OnBgmVolumeChanged(volume))
            .AddTo(disposables);

        // 戻るボタンイベントを購読
        view.OnBackButtonClicked
            .Subscribe(_ => OnBackButtonClicked())
            .AddTo(disposables);
    }

    private void OnBgmVolumeChanged(float volume)
    {
        // 入力データの検証・加工
        float clampedVolume = Mathf.Clamp01(volume);
        
        // UseCaseを呼び出す
        updateBgmVolumeUseCase.Execute(clampedVolume);
    }

    private void OnBackButtonClicked()
    {
        navigateBackUseCase.Execute();
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}
```

---

### 3. Interactor（インタラクター）= UseCase

**役割**:
- ビジネスロジックを実行する
- アプリケーションの中核
- 「〇〇する」という処理の実装

**やること**:
- ビジネスルールの実行
- Entityの操作
- Repositoryからのデータ取得・保存
- Presenterへの結果通知

**やらないこと**:
- UI表示（それはPresenterとViewの仕事）
- Unity固有の処理（MonoBehaviourは使わない）
- 入力の検証（それはControllerの仕事）

**コード例**:

```csharp
using Zenject;

public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    private readonly IAudioSettingsRepository audioSettingsRepository;
    private readonly IVolumeUpdateOutputPort outputPort;
    private readonly IAudioManager audioManager;

    [Inject]
    public UpdateBgmVolumeInteractor(
        IAudioSettingsRepository audioSettingsRepository,
        IVolumeUpdateOutputPort outputPort,
        IAudioManager audioManager)
    {
        this.audioSettingsRepository = audioSettingsRepository;
        this.outputPort = outputPort;
        this.audioManager = audioManager;
    }

    public void Execute(float volume)
    {
        // 1. 現在の設定を取得
        AudioSettings currentSettings = audioSettingsRepository.Load();
        
        // 2. ビジネスルール：音量の妥当性チェック
        if (!currentSettings.IsValidVolume(volume))
        {
            outputPort.OnVolumeUpdateFailed("音量は0～1の範囲で指定してください");
            return;
        }
        
        // 3. エンティティを更新
        currentSettings.UpdateBgmVolume(volume);
        
        // 4. 実際のオーディオも変更
        audioManager.SetBgmVolume(volume);
        
        // 5. 変更を永続化
        audioSettingsRepository.Save(currentSettings);
        
        // 6. 初回チェック
        if (!currentSettings.HasAdjustedVolumeOnce)
        {
            currentSettings.MarkVolumeAdjusted();
            audioSettingsRepository.Save(currentSettings);
            outputPort.OnTutorialCompleted("音量調整");
        }
        
        // 7. 結果をPresenterに通知
        outputPort.OnVolumeUpdated(currentSettings);
    }
}
```

---

### 4. Presenter（プレゼンター）

**役割**:
- Interactorからの結果を受け取る
- 表示用のデータに変換する
- Viewに表示を指示する

**やること**:
- ビジネスデータを表示用データに変換
- Viewのメソッドを呼び出して画面更新
- 複数のViewを協調させる

**やらないこと**:
- ビジネスロジック（それはInteractorの仕事）
- 入力の受付（それはControllerの仕事）
- Unity固有の実装（それはViewの仕事）

**コード例**:

```csharp
using Zenject;
using Cysharp.Threading.Tasks;

public class VolumePresenter : IVolumeUpdateOutputPort
{
    private readonly SettingsView settingsView;
    private readonly ToastView toastView;

    [Inject]
    public VolumePresenter(
        SettingsView settingsView,
        ToastView toastView)
    {
        this.settingsView = settingsView;
        this.toastView = toastView;
    }

    public void OnVolumeUpdated(AudioSettings settings)
    {
        // ビジネスデータを表示用データに変換
        float bgmVolumePercent = settings.BgmVolume;
        
        // Viewに表示を指示
        settingsView.SetBgmVolume(bgmVolumePercent);
        
        // トースト通知を表示
        toastView.Show("音量を変更しました");
    }

    public void OnVolumeUpdateFailed(string errorMessage)
    {
        toastView.ShowError($"エラー: {errorMessage}");
    }

    public void OnTutorialCompleted(string tutorialName)
    {
        toastView.Show($"「{tutorialName}」をマスターしました");
    }
}
```

---

### 5. InputPort と OutputPort（インターフェース）

**目的**:
- テストやモックへの切り替えを容易にする
- IoC（制御の反転）を実現する
- 依存関係の方向性を守る

**実装例**:

```csharp
// InputPort: Interactorが実装する
public interface IUpdateBgmVolumeInputPort
{
    void Execute(float volume);
}

// OutputPort: Presenterが実装する
public interface IVolumeUpdateOutputPort
{
    void OnVolumeUpdated(AudioSettings settings);
    void OnVolumeUpdateFailed(string errorMessage);
    void OnTutorialCompleted(string tutorialName);
}

// 使用例
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    // インターフェース経由で依存
    [Inject] private readonly IVolumeUpdateOutputPort outputPort;
    
    public void Execute(float volume)
    {
        // 実装...
        outputPort.OnVolumeUpdated(settings);
    }
}
```

---

## 4層構造の詳細

### 📦 Entity層（Domain）

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

### 📦 UseCase層（Application）

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

---

### 📦 InterfaceAdapter層（Presentation）

**役割**: データ変換、入出力の制御

**特徴**:
- Controllerは「入力の窓口」
- Presenterは「出力の窓口」
- Unity非依存（基本的には）

**含まれるもの**:
- Controller（Viewからの入力を受け取る）
- Presenter（結果を表示用に変換）

---

### 📦 FrameworkAndDriver層（Infrastructure）

**役割**: Unity固有の実装、外部ライブラリとの連携

**特徴**:
- Unity依存
- MonoBehaviourを使用
- 具体的な実装

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
}

// Repository実装
public class AudioSettingsRepositoryImpl : IAudioSettingsRepository
{
    private const string BGM_VOLUME_KEY = "BgmVolume";

    public UniTask<AudioSettings> Load()
    {
        var bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        var settings = new AudioSettings(bgmVolume);
        return UniTask.FromResult(settings);
    }

    public UniTask Save(AudioSettings settings)
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, settings.BgmVolume);
        PlayerPrefs.Save();
        return UniTask.CompletedTask;
    }
}
```

---

## デザインパターン

### 🎯 State Machineパターン

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
        await viewFactory.PreloadAsync();
    }

    public override async UniTask OnEnter()
    {
        view = viewFactory.Create();
        controller.Initialize(view);
    }

    public override async UniTask OnExit()
    {
        Object.Destroy(view.gameObject);
    }

    public override async UniTask OnCleanup()
    {
        await viewFactory.ReleaseAsync();
    }
}
```

---

### 🎯 Dependency Injection (DI) パターン

**目的**: 依存関係の注入、疎結合化、テスタビリティ向上

**使用ライブラリ**: Zenject

**Installerでの設定例**:

```csharp
public class OutGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Repository実装
        Container.Bind<IAudioSettingsRepository>()
            .To<AudioSettingsRepositoryImpl>()
            .AsSingle();

        // Interactor → InputPort
        Container.Bind<IUpdateBgmVolumeInputPort>()
            .To<UpdateBgmVolumeInteractor>()
            .AsSingle();

        // Presenter → OutputPort
        Container.Bind<IVolumeUpdateOutputPort>()
            .To<VolumePresenter>()
            .AsSingle();

        // Controller
        Container.Bind<SettingsController>().AsSingle();

        // State
        Container.Bind<SettingsState>().AsSingle();

        // StateMachine
        Container.Bind<StateMachine<OutGameStateKey>>()
            .AsSingle();
    }
}
```

---

### 🎯 Repositoryパターン

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

---

### 🎯 Factoryパターン

**目的**: オブジェクト生成の抽象化

**種類**:

#### 1. Zenject Factory（シンプル）

```csharp
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

#### 2. Addressable View Factory（プリロード対応）

```csharp
public class AddressableViewFactory<TView> : IFactory<TView>, IAssetPreloadable
    where TView : Component, IView
{
    private readonly DiContainer container;
    private readonly IAddressableAssetProvider assetProvider;
    private readonly string assetKey;
    private GameObject cachedPrefab;

    public async UniTask PreloadAsync()
    {
        cachedPrefab = await assetProvider.LoadAssetAsync<GameObject>(assetKey);
    }

    public TView Create()
    {
        return container.InstantiatePrefabForComponent<TView>(
            cachedPrefab,
            canvas.transform
        );
    }

    public async UniTask ReleaseAsync()
    {
        await assetProvider.ReleaseAsset(assetKey);
        cachedPrefab = null;
    }
}
```

---

### 🎯 Observerパターン（Reactive Extensions - R3）

**目的**: イベント駆動プログラミング、データバインディング

**例**:

```csharp
// Viewでイベントを公開
public class SettingsView : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;

    public Observable<float> OnBgmVolumeChanged =>
        bgmSlider.OnValueChangedAsObservable();
}

// Controllerで購読
public class SettingsController
{
    private readonly CompositeDisposable disposables = new();

    public void Initialize(SettingsView view)
    {
        view.OnBgmVolumeChanged
            .Subscribe(volume => HandleBgmVolumeChange(volume))
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
```

---

## クラス設計とアーキテクチャ構成

### ディレクトリ構成

```
Assets/Scripts/OutGame/
├── Domain/                          # Entity層
│   ├── Models/
│   │   ├── AudioSettings.cs
│   │   └── UserProfile.cs
│   └── Repositories/
│       ├── IAudioSettingsRepository.cs
│       └── IUserProfileRepository.cs
│
├── Application/                     # UseCase層
│   ├── UseCases/
│   │   ├── UpdateBgmVolumeInteractor.cs
│   │   └── NavigateToHomeInteractor.cs
│   └── Ports/
│       ├── Input/
│       │   ├── IUpdateBgmVolumeInputPort.cs
│       │   └── INavigateToHomeInputPort.cs
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
├── StateMachine/                    # 状態管理
│   ├── IState.cs
│   └── StateMachine.cs
│
├── States/                          # 各画面のState
│   ├── TitleState.cs
│   ├── HomeState.cs
│   └── SettingsState.cs
│
└── Installers/                      # DI設定
    └── OutGameInstaller.cs
```

---

## 実装時のベストプラクティス

### ✅ DO（推奨）

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

### ❌ DON'T（非推奨）

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

## 新規画面の追加手順

### Step 1: StateKeyを追加

```csharp
public enum OutGameStateKey
{
    Title,
    Home,
    Settings,
    Shop  // ← 追加
}
```

### Step 2: Domain層を作成

```csharp
// Models/ShopItem.cs
public class ShopItem
{
    public string Id { get; }
    public string Name { get; }
    public int Price { get; }
}

// Repositories/IShopRepository.cs
public interface IShopRepository
{
    UniTask<List<ShopItem>> GetItems();
    UniTask<bool> Purchase(string itemId);
}
```

### Step 3: Application層を作成

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
    private readonly IShopOutputPort outputPort;

    public async UniTask Execute(string itemId)
    {
        var item = await shopRepository.GetItemById(itemId);
        // ビジネスロジック...
        outputPort.OnPurchaseSuccess(item);
    }
}
```

### Step 4: Presentation層を作成

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

    public void OnPurchaseSuccess(ShopItem item)
    {
        view.ShowSuccessMessage($"{item.Name}を購入しました");
    }
}
```

### Step 5: Infrastructure層を作成

```csharp
// Views/ShopView.cs
public class ShopView : MonoBehaviour, IView
{
    [SerializeField] private Button[] itemButtons;

    public Observable<string> OnItemButtonClicked { get; private set; }

    public void ShowSuccessMessage(string message)
    {
        Debug.Log(message);
    }
}

// Repositories/ShopRepositoryImpl.cs
public class ShopRepositoryImpl : IShopRepository
{
    public async UniTask<List<ShopItem>> GetItems()
    {
        // 実装...
        return new List<ShopItem>();
    }
}
```

### Step 6: Stateを作成

```csharp
// States/ShopState.cs
public class ShopState : BaseState
{
    private readonly ShopController controller;
    private readonly IFactory<ShopView> viewFactory;
    private ShopView view;

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
        controller.Initialize(view);
    }

    public override async UniTask OnExit()
    {
        Object.Destroy(view.gameObject);
    }
}
```

### Step 7: Installerでバインド

```csharp
public class OutGameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
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

        // Shop - Controller
        Container.Bind<ShopController>()
            .AsSingle();

        // Shop - State
        Container.Bind<ShopState>()
            .AsSingle();
    }
}
```

---

## まとめ

### クリーンアーキテクチャのメリット

✅ **テスタビリティ**: Interactorは純粋なC#クラスなので単体テストが容易  
✅ **保守性**: 各層の責務が明確で、変更の影響範囲が限定的  
✅ **拡張性**: 新機能追加時に既存コードへの影響が最小限  
✅ **再利用性**: ビジネスロジックは他プラットフォームでも利用可能  
✅ **依存関係の整理**: 内側→外側の一方向依存で循環参照を防止

### 重要な原則

1. **Controller → Interactor → Presenter** の処理フロー
2. **外側→内側** の依存関係の方向
3. **インターフェース** に対するプログラミング
4. **Interactor** にビジネスロジックを集約
5. **State** でライフサイクルを管理

このアーキテクチャにより、**長期的な保守性**と**チーム開発での拡張性**を実現しています。

