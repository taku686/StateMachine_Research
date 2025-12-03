# Unityゲーム開発におけるクリーンアーキテクチャまとめ

## 参考記事
1. [Unity ゲーム開発で使うクリーンアーキテクチャの勘所](https://zenn.dev/fig_codefactory/articles/09122552d41c1a)
2. [Unityを利用した大規模なゲーム開発にクリーンアーキテクチャを採用した話](https://developers.wonderpla.net/entry/2021/02/18/121932)

---

## クリーンアーキテクチャの基本概念

### 最も重要なポイント
クリーンアーキテクチャを理解する上で最も重要なのは、**処理の流れ**です。

```
Controller → Interactor → Presenter
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
│   │   │   └── AttackEnemyInteractor.cs
│   │   └── Ports/
│   │       ├── Input/
│   │       │   ├── IPickUpItemInputPort.cs
│   │       │   └── IMovePlayerInputPort.cs
│   │       └── Output/
│   │           ├── IDialogOutputPort.cs
│   │           └── IToastOutputPort.cs
│   │
│   ├── Presentation/        # InterfaceAdapter層
│   │   ├── Controllers/
│   │   │   ├── PlayerController.cs
│   │   │   └── ItemController.cs
│   │   └── Presenters/
│   │       ├── DialogPresenter.cs
│   │       └── ToastPresenter.cs
│   │
│   └── Infrastructure/      # FrameworkAndDriver層
│       ├── Views/
│       │   ├── PlayerView.cs
│       │   ├── DialogView.cs
│       │   └── ToastView.cs
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

---

## まとめ

クリーンアーキテクチャの本質は、**処理の流れ（Controller → Interactor → Presenter）**と**依存関係の方向性（外→内）**です。

この2つを理解し、各層の責務を明確にすることで、変更に強く、テストしやすい、保守性の高いコードベースを実現できます。

Unityゲーム開発においては、DIコンテナと組み合わせることで、より効果的にクリーンアーキテクチャを適用できます。

