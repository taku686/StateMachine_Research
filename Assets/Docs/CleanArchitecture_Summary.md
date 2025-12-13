# Unityゲーム開発におけるクリーンアーキテクチャまとめ

## 参考記事
1. [Unity ゲーム開発で使うクリーンアーキテクチャの勘所](https://zenn.dev/fig_codefactory/articles/09122552d41c1a)
2. [Unityを利用した大規模なゲーム開発にクリーンアーキテクチャを採用した話](https://developers.wonderpla.net/entry/2021/02/18/121932)

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
StateMachine (画面遷移実行)
  ↓
State (新しい画面のライフサイクル)
  ↓
Presenter (出力の窓口)
  ↓
View (UI表示)
```

この単純な流れが、クリーンアーキテクチャの本質を表しています。

### 4つの層構造

内側から外側に向かって以下の4層で構成されます:

1. **Entity層** - ビジネスロジック、ドメインモデル
2. **UseCase層** - アプリケーション固有のビジネスルール
3. **InterfaceAdapter層** - データ変換、Controller/Presenter
4. **FrameworkAndDriver層** - UI、DB、外部ライブラリ

**依存関係のルール**: 外側の層は内側の層に依存できるが、内側の層は外側の層を知らない（依存しない）

---

## 各コンポーネントの役割

### 1. Interactor（ユースケースの実装）

**役割**: 
- UseCaseの具体的な実装
- アクター（操作者）とシステムの間で特定の目的を達成するためのシナリオを実装
- ビジネスロジックの中核

**例**: プレイヤーがアイテムを拾う

```csharp
public class PickUpItemInteractor : IPickUpItemInputPort
{
    [Inject] private readonly IDialogOutputPort dialogOutputPort;
    [Inject] private readonly IToastOutputPort toastOutputPort;
    [Inject] private readonly PlayerRepository playerRepository;
    [Inject] private readonly ItemRepository itemRepository;

    public void Execute(int itemId)
    {
        // アイテムを取得
        var item = itemRepository.GetById(itemId);
        
        // プレイヤーのインベントリに追加
        var player = playerRepository.GetCurrentPlayer();
        player.AddItem(item);
        
        // 初めて入手したアイテムかチェック
        bool isFirstTime = !player.HasObtainedBefore(item);
        
        if (isFirstTime)
        {
            // ダイアログを表示（時間停止）
            dialogOutputPort.Handle(item);
        }
        else
        {
            // トーストを表示
            toastPresenter.Handle(item);
        }
    }
}
```

**ポイント**:
- Interactorを見るだけで処理の全体像が把握できる
- 分岐処理もInteractor内で完結させる
- 責務を明確にすることで変更に強くなる

### 2. Controller（入力の窓口）

**役割**:
- Viewからのイベントを受け取る
- 入力データを加工してInteractorに渡す
- 複数の呼び出し箇所がある場合の共通処理

**例**: プレイヤーの移動

```csharp
public class PlayerController
{
    [Inject] private readonly MovePlayerInteractor movePlayerInteractor;

    public void Move(Vector2 moveInput, float deltaTime)
    {
        // ゲームパッドの入力をプレイヤーの移動方向に変換
        Vector3 direction = CalculateDirectionRelativeToCamera(
            moveInput,
            Camera.main.transform);

        // プレイヤー移動のInteractorを実行
        movePlayerInteractor.Execute(direction, deltaTime);
    }

    private Vector3 CalculateDirectionRelativeToCamera(Vector2 input, Transform cameraTransform)
    {
        // カメラの向きを考慮した方向計算
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        return forward * input.y + right * input.x;
    }
}
```

**メリット**:
- 入力データの加工処理を集約
- モックへの差し替えが容易
- 将来の拡張性が高まる

### 3. Presenter（出力の窓口）

**役割**:
- Interactorから呼び出される
- ビジネスロジックの結果をViewに表示するためのデータに変換
- Viewへの表示指示

**例**: アイテム取得時のダイアログ表示

```csharp
public class DialogPresenter : IDialogOutputPort
{
    [Inject] private readonly DialogView dialogView;

    public void Handle(Item item)
    {
        // Viewに表示するためのデータに変換
        var dialogData = new DialogData
        {
            Title = "アイテムを入手しました！",
            Message = $"{item.Name}を手に入れた！",
            IconSprite = item.Icon
        };

        // Viewに表示を指示
        dialogView.Show(dialogData);
    }
}
```

**ポイント**:
- Interactorから直接Viewを操作しない
- データの変換と表示の指示を担当
- テスタビリティの向上

### 4. View（MonoBehaviour）

**役割**:
- Unity固有の実装（MonoBehaviour）
- ユーザー入力の受付
- 画面表示の実装

**例**:

```csharp
public class PlayerView : MonoBehaviour
{
    [Inject] private PlayerController playerController;

    private void Update()
    {
        // 入力を受け取る
        Vector2 moveInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // Controllerに処理を委譲
        if (moveInput.sqrMagnitude > 0.01f)
        {
            playerController.Move(moveInput, Time.deltaTime);
        }
    }
}
```

---

## StateMachine（ステートマシーンパターン）

### 役割
- 画面遷移の管理
- ステートのライフサイクル管理（OnEnter, OnExit, OnUpdate）
- 画面遷移履歴の管理（戻るボタン対応）
- ルートステートの概念（履歴をクリアする基準画面）

### 構造

#### 1. StateMachine<TStateKey>（ステートマシーン本体）

**役割**:
- ジェネリックなステートマシーン実装
- 履歴管理機能付き
- ルートステートの概念で履歴をクリア

**主要メソッド**:
- `RegisterState()`: ステートを登録
- `ChangeState()`: ステートを変更（履歴に追加）
- `GoBack()`: 前のステートに戻る
- `Update()`: 現在のステートの更新処理を実行

**例**:

```csharp
public class StateMachine<TStateKey> where TStateKey : Enum
{
    private readonly Dictionary<TStateKey, IState> _states = new();
    private readonly Stack<TStateKey> _stateHistory = new();
    private readonly HashSet<TStateKey> _rootStates = new();
    private IState _currentState;
    private TStateKey _currentStateKey;

    public TStateKey CurrentStateKey => _currentStateKey;
    public bool CanGoBack => _stateHistory.Count > 0;

    // ステートを登録（isRootState: 履歴をクリアする基準画面）
    public void RegisterState(TStateKey key, IState state, bool isRootState = false)
    {
        _states[key] = state;
        if (isRootState)
        {
            _rootStates.Add(key);
        }
    }

    // ステートを変更（履歴に追加）
    public async UniTask ChangeState(TStateKey newStateKey, bool addToHistory = true)
    {
        // 現在のステートから退出
        if (_currentState != null)
        {
            if (addToHistory && !_rootStates.Contains(_currentStateKey))
            {
                _stateHistory.Push(_currentStateKey);
            }
            await _currentState.OnExit();
        }

        // ルートステートに遷移する場合は履歴をクリア
        if (_rootStates.Contains(newStateKey))
        {
            _stateHistory.Clear();
        }

        // 新しいステートに入る
        _currentState = _states[newStateKey];
        _currentStateKey = newStateKey;
        await _currentState.OnEnter();
    }

    // 前のステートに戻る
    public async UniTask GoBack()
    {
        if (!CanGoBack) return;
        var previousStateKey = _stateHistory.Pop();
        await ChangeState(previousStateKey, addToHistory: false);
    }

    // 現在のステートの更新処理を実行
    public void Update()
    {
        _currentState?.OnUpdate();
    }
}
```

#### 2. IState（ステートのインターフェース）

**役割**:
- ステートの基本インターフェース
- ライフサイクルメソッドを定義

```csharp
public interface IState
{
    UniTask OnEnter();   // ステートに入る時の処理
    UniTask OnExit();    // ステートから出る時の処理
    void OnUpdate();     // ステートの更新処理
}
```

#### 3. BaseState（ステートの基底クラス）

**役割**:
- IStateを実装した抽象クラス
- デフォルト実装を提供

```csharp
public abstract class BaseState : IState
{
    public virtual async UniTask OnEnter() => await UniTask.CompletedTask;
    public virtual async UniTask OnExit() => await UniTask.CompletedTask;
    public virtual void OnUpdate() { }
}
```

#### 4. 具体的なState実装（例: TitleState）

**役割**:
- 各画面のステート実装
- Viewのロード、Controllerの初期化、画面表示を管理

**例**:

```csharp
public class TitleState : BaseState
{
    private readonly TitleController controller;
    private TitleView view;

    [Inject]
    public TitleState(TitleController controller)
    {
        this.controller = controller;
    }

    public override async UniTask OnEnter()
    {
        // Addressablesから View をロード
        var viewObject = await Addressables.InstantiateAsync(nameof(TitleView));
        view = viewObject.GetComponent<TitleView>();

        // Controllerを初期化（ViewとUseCaseを接続）
        controller.Initialize(view);

        // View を表示
        await view.Show();
    }

    public override async UniTask OnExit()
    {
        controller?.Dispose();

        if (view != null)
        {
            await view.Hide();
            view.Dispose();
            view = null;
        }
    }
}
```

#### 5. OutGameManager（ステートマシーンの管理）

**役割**:
- ステートマシーンの初期化と管理
- 各ステートの登録
- ステートマシーンの更新処理

**例**:

```csharp
public class OutGameManager : MonoBehaviour
{
    private StateMachine<OutGameStateKey> _stateMachine;
    private TitleState _titleState;
    private HomeState _homeState;
    private SettingsState _settingsState;

    [Inject]
    public void Construct(
        StateMachine<OutGameStateKey> stateMachine,
        TitleState titleState,
        HomeState homeState,
        SettingsState settingsState)
    {
        _stateMachine = stateMachine;
        _titleState = titleState;
        _homeState = homeState;
        _settingsState = settingsState;

        InitializeStateMachine();
    }

    private void Start()
    {
        // 初期ステートをタイトルに設定
        _stateMachine.ChangeState(OutGameStateKey.Title).Forget();
    }

    private void Update()
    {
        _stateMachine?.Update();
    }

    private void InitializeStateMachine()
    {
        // 各ステートを登録
        // isRootState = true: 履歴をクリアする基準画面（Title, Home）
        _stateMachine.RegisterState(OutGameStateKey.Title, _titleState, isRootState: true);
        _stateMachine.RegisterState(OutGameStateKey.Home, _homeState, isRootState: true);
        _stateMachine.RegisterState(OutGameStateKey.Settings, _settingsState);
    }
}
```

### StateMachineとクリーンアーキテクチャの統合

**処理の流れ**:
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

**ポイント**:
- Stateは画面遷移のライフサイクルを管理
- State内でViewをロードし、Controllerを初期化
- ControllerはUseCase（Interactor）を呼び出して画面遷移を実行
- UseCaseはStateMachineを使って画面遷移を実行
- 画面遷移履歴を管理することで、戻るボタンに対応

---

## InputPort と OutputPort（インターフェース）

### 目的
- テストやモックへの切り替えを容易にする
- IoC（制御の反転）を実現する
- 依存関係の方向性を守る

### 実装例

```csharp
// InputPort: Interactorが実装する
public interface IPickUpItemInputPort
{
    void Execute(int itemId);
}

// OutputPort: Presenterが実装する
public interface IDialogOutputPort
{
    void Handle(Item item);
}

public interface IToastOutputPort
{
    void Handle(Item item);
}
```

**使用例**:

```csharp
public class PickUpItemInteractor : IPickUpItemInputPort
{
    // インターフェース経由で依存
    [Inject] private readonly IDialogOutputPort dialogOutputPort;
    [Inject] private readonly IToastOutputPort toastOutputPort;
    
    public void Execute(int itemId)
    {
        // 実装...
    }
}
```

---

## Unityプロジェクトへの適用

### ディレクトリ構成例

```
Assets/
├── Scripts/
│   ├── Domain/              # Entity層
│   │   ├── Models/
│   │   │   ├── Player.cs
│   │   │   ├── Item.cs
│   │   │   └── Enemy.cs
│   │   └── Repositories/
│   │       └── IPlayerRepository.cs
│   │
│   ├── Application/         # UseCase層
│   │   ├── UseCases/
│   │   │   ├── PickUpItemInteractor.cs
│   │   │   ├── MovePlayerInteractor.cs
│   │   │   ├── AttackEnemyInteractor.cs
│   │   │   ├── NavigateToHomeInteractor.cs
│   │   │   └── OpenSettingsInteractor.cs
│   │   └── Ports/
│   │       ├── Input/
│   │       │   ├── IPickUpItemInputPort.cs
│   │       │   ├── IMovePlayerInputPort.cs
│   │       │   ├── INavigateToHomeInputPort.cs
│   │       │   └── IOpenSettingsInputPort.cs
│   │       └── Output/
│   │           ├── IDialogOutputPort.cs
│   │           ├── IToastOutputPort.cs
│   │           └── INavigationOutputPort.cs
│   │
│   ├── Presentation/        # InterfaceAdapter層
│   │   ├── Controllers/
│   │   │   ├── PlayerController.cs
│   │   │   ├── ItemController.cs
│   │   │   ├── TitleController.cs
│   │   │   ├── HomeController.cs
│   │   │   └── SettingsController.cs
│   │   └── Presenters/
│   │       ├── DialogPresenter.cs
│   │       ├── ToastPresenter.cs
│   │       └── NavigationPresenter.cs
│   │
│   ├── StateMachine/        # ステートマシーン基盤
│   │   ├── IState.cs
│   │   ├── BaseState.cs
│   │   └── StateMachine.cs
│   │
│   ├── States/              # 各画面のステート実装
│   │   ├── TitleState.cs
│   │   ├── HomeState.cs
│   │   └── SettingsState.cs
│   │
│   ├── OutGameManager.cs    # アウトゲーム全体の管理
│   ├── OutGameStateKey.cs   # ステート種類の定義（Enum）
│   │
│   └── Infrastructure/      # FrameworkAndDriver層
│       ├── Views/
│       │   ├── PlayerView.cs
│       │   ├── DialogView.cs
│       │   ├── ToastView.cs
│       │   ├── TitleView.cs
│       │   ├── HomeView.cs
│       │   └── SettingsView.cs
│       └── Repositories/
│           └── PlayerRepositoryImpl.cs
```

### 依存性注入（DI）の必要性

クリーンアーキテクチャを実現するには、DIコンテナが必須です。

**推奨DIコンテナ**:
- Zenject (Extenject)
- VContainer
- Simpleton（有料）

**理由**:
- インターフェースを通じた依存関係の注入
- テスト時のモック差し替え
- シングルトンパターンの回避

---

## メリット

### 1. テスタビリティの向上
- 各層が独立しているため、単体テストが容易
- モックを使った検証が可能

### 2. 変更に強い
- ビジネスロジックがフレームワークに依存しない
- UIの変更がビジネスロジックに影響しない

### 3. 責務の明確化
- 各クラスの役割が明確
- どこに何を書けばいいかが分かりやすい

### 4. 再利用性の向上
- ビジネスロジックを他のプラットフォームでも利用可能
- Interactorは複数のControllerから呼び出せる

---

## 実装時の注意点

### 1. UseCaseの粒度
- 「ひとつのシナリオ」として完結する単位で設計
- 大きすぎず、小さすぎず
- 例: 「アイテムを拾う」には「初回ダイアログ表示」も含める

### 2. 責務の分離
- InterfaceAdapter層（Controller/Presenter）は薄く保つ
- ビジネスロジックはInteractorに集約
- 分岐処理もInteractor内で完結させる

### 3. 依存関係の方向
- 必ず内側に向かって依存する
- 外側の層を内側の層が知らないようにする
- インターフェースを使って依存を逆転させる

### 4. プロジェクト規模に応じた調整
- 小規模プロジェクトでは一部を省略してもOK
- 例: ControllerとViewを統合する
- ただし、将来の拡張性は考慮する

### 5. StateMachineの設計
- Stateは画面遷移のライフサイクル管理に専念する
- Viewのロードと破棄はStateで管理
- Controllerの初期化と破棄もStateで管理
- 画面遷移のロジックはUseCase（Interactor）に集約
- ルートステートの概念を活用して履歴管理を適切に行う
- StateMachineはZenjectでシングルトンとして管理

---

## まとめ

クリーンアーキテクチャの本質は、**処理の流れ（Controller → Interactor → Presenter）**と**依存関係の方向性（外→内）**です。

この2つを理解し、各層の責務を明確にすることで、変更に強く、テストしやすい、保守性の高いコードベースを実現できます。

Unityゲーム開発においては、DIコンテナと組み合わせることで、より効果的にクリーンアーキテクチャを適用できます。

### StateMachineパターンとの統合

StateMachineパターンを組み合わせることで、画面遷移の管理がより明確になります：

- **State**: 画面遷移のライフサイクルを管理（Viewのロード、Controllerの初期化）
- **StateMachine**: 画面遷移の実行と履歴管理
- **Controller → Interactor**: 画面遷移のUseCaseを実行
- **Interactor → StateMachine**: 実際の画面遷移を実行

この統合により、画面遷移のロジックがUseCase層に集約され、クリーンアーキテクチャの原則を保ちながら、画面遷移を管理できます。

