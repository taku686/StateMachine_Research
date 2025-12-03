# Controller、UseCase、Interactor、Presenterの役割を詳しく解説

## 全体像の理解

まず、**用語の整理**から始めましょう。

```
UseCase = Interactor
```

**UseCase**は「概念」で、**Interactor**はその「実装クラス」です。
つまり、「ユーザーがアイテムを拾う」という**UseCase（ユースケース）**を実装したクラスが**Interactor**です。

---

## 処理の流れを追う

### 具体例：設定画面でBGM音量を変更する

ユーザーがスライダーを動かして、BGMの音量を50%に変更する場合を例に、各コンポーネントの役割を見ていきましょう。

```
ユーザー操作
    ↓
【View】スライダーを動かす（50%）
    ↓
【Controller】入力を受け取り、データを加工してUseCaseに渡す
    ↓
【Interactor（UseCase）】ビジネスロジックを実行
    ↓
【Presenter】結果を受け取り、表示用に変換してViewに渡す
    ↓
【View】画面を更新する
```

---

## 1. View（ビュー）

### 役割
- **ユーザーの入力を受け取る**
- **画面に表示する**
- **Unity固有の実装（MonoBehaviour）**

### やること
- ボタンクリック、スライダー変更などのイベントを検知
- イベントを`Observable`として公開
- Presenterから指示された内容を画面に表示

### やらないこと
- ビジネスロジック
- データの加工
- 他の画面への遷移判断

### コード例

```csharp
using UnityEngine;
using UnityEngine.UI;
using R3;
using Cysharp.Threading.Tasks;

/// <summary>
/// 設定画面のView
/// MonoBehaviourを継承し、Unity固有の実装を担当
/// </summary>
public class SettingsView : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Text bgmVolumeText;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Text seVolumeText;
    [SerializeField] private Button backButton;
    [SerializeField] private CanvasGroup canvasGroup;

    // イベントをObservableとして公開（Controllerが購読する）
    public Observable<float> OnBgmVolumeChanged => 
        bgmSlider.OnValueChangedAsObservable();
    
    public Observable<float> OnSeVolumeChanged => 
        seSlider.OnValueChangedAsObservable();
    
    public Observable<Unit> OnBackButtonClicked => 
        backButton.OnClickAsObservable();

    /// <summary>
    /// Presenterから呼ばれる：BGM音量を画面に表示
    /// </summary>
    public void SetBgmVolume(float volume)
    {
        // スライダーの値を更新（イベントは発火させない）
        bgmSlider.SetValueWithoutNotify(volume);
        
        // テキストも更新（例：「BGM: 50%」）
        bgmVolumeText.text = $"BGM: {Mathf.RoundToInt(volume * 100)}%";
    }

    /// <summary>
    /// Presenterから呼ばれる：SE音量を画面に表示
    /// </summary>
    public void SetSeVolume(float volume)
    {
        seSlider.SetValueWithoutNotify(volume);
        seVolumeText.text = $"SE: {Mathf.RoundToInt(volume * 100)}%";
    }

    /// <summary>
    /// Presenterから呼ばれる：画面を表示
    /// </summary>
    public async UniTask Show()
    {
        gameObject.SetActive(true);
        
        // フェードイン演出
        canvasGroup.alpha = 0f;
        await canvasGroup.DOFade(1f, 0.3f);
    }

    /// <summary>
    /// Presenterから呼ばれる：画面を非表示
    /// </summary>
    public async UniTask Hide()
    {
        // フェードアウト演出
        await canvasGroup.DOFade(0f, 0.3f);
        gameObject.SetActive(false);
    }
}
```

**ポイント**:
- Viewは「入力の検知」と「表示」だけを担当
- ビジネスロジックは一切書かない
- MonoBehaviourを継承し、Unity固有の機能を使う

---

## 2. Controller（コントローラー）

### 役割
- **Viewからの入力を受け取る**
- **入力データを加工する**
- **適切なUseCaseを呼び出す**

### やること
- Viewのイベントを購読（Subscribe）
- 入力データの検証・変換
- 複数のViewからの入力を統合
- UseCaseの呼び出し

### やらないこと
- ビジネスロジック（それはInteractorの仕事）
- 画面表示（それはPresenterとViewの仕事）

### コード例

```csharp
using R3;
using Zenject;

/// <summary>
/// 設定画面のController
/// Viewからの入力を受け取り、UseCaseに渡す
/// </summary>
public class SettingsController
{
    // UseCaseへの参照（インターフェース経由）
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeUseCase;
    private readonly IUpdateSeVolumeInputPort updateSeVolumeUseCase;
    private readonly INavigateBackInputPort navigateBackUseCase;
    
    private readonly CompositeDisposable disposables = new();

    [Inject]
    public SettingsController(
        IUpdateBgmVolumeInputPort updateBgmVolumeUseCase,
        IUpdateSeVolumeInputPort updateSeVolumeUseCase,
        INavigateBackInputPort navigateBackUseCase)
    {
        this.updateBgmVolumeUseCase = updateBgmVolumeUseCase;
        this.updateSeVolumeUseCase = updateSeVolumeUseCase;
        this.navigateBackUseCase = navigateBackUseCase;
    }

    /// <summary>
    /// Viewと接続してイベントを購読する
    /// </summary>
    public void Initialize(SettingsView view)
    {
        // BGM音量変更イベントを購読
        view.OnBgmVolumeChanged
            .Subscribe(volume => OnBgmVolumeChanged(volume))
            .AddTo(disposables);

        // SE音量変更イベントを購読
        view.OnSeVolumeChanged
            .Subscribe(volume => OnSeVolumeChanged(volume))
            .AddTo(disposables);

        // 戻るボタンイベントを購読
        view.OnBackButtonClicked
            .Subscribe(_ => OnBackButtonClicked())
            .AddTo(disposables);
    }

    /// <summary>
    /// BGM音量が変更されたときの処理
    /// </summary>
    private void OnBgmVolumeChanged(float volume)
    {
        // 入力データの検証・加工
        float clampedVolume = Mathf.Clamp01(volume);
        
        // UseCaseを呼び出す
        updateBgmVolumeUseCase.Execute(clampedVolume);
    }

    /// <summary>
    /// SE音量が変更されたときの処理
    /// </summary>
    private void OnSeVolumeChanged(float volume)
    {
        // 入力データの検証・加工
        float clampedVolume = Mathf.Clamp01(volume);
        
        // UseCaseを呼び出す
        updateSeVolumeUseCase.Execute(clampedVolume);
    }

    /// <summary>
    /// 戻るボタンが押されたときの処理
    /// </summary>
    private void OnBackButtonClicked()
    {
        // UseCaseを呼び出す
        navigateBackUseCase.Execute();
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}
```

**ポイント**:
- Controllerは「入力の窓口」
- データの検証・加工はするが、ビジネスロジックは書かない
- UseCaseを呼び出すだけの薄いレイヤー

### Controllerが必要な理由

「Viewが直接Interactorを呼べばいいのでは？」と思うかもしれません。
しかし、Controllerを挟むことで以下のメリットがあります：

1. **入力データの加工を共通化できる**
   ```csharp
   // 複数の場所から呼ばれても、加工処理は1箇所
   private float NormalizeVolume(float rawValue)
   {
       return Mathf.Clamp01(rawValue);
   }
   ```

2. **複数の入力を統合できる**
   ```csharp
   // ゲームパッドとキーボードの入力を統合
   private void OnMoveInput()
   {
       Vector2 gamepadInput = Input.GetAxis("Horizontal"), Input.GetAxis("Vertical");
       Vector2 keyboardInput = GetKeyboardInput();
       Vector2 finalInput = gamepadInput.sqrMagnitude > 0 ? gamepadInput : keyboardInput;
       
       movePlayerUseCase.Execute(finalInput);
   }
   ```

3. **テスト時にモックに差し替えやすい**

---

## 3. Interactor（インタラクター）= UseCase

### 役割
- **ビジネスロジックを実行する**
- **アプリケーションの中核**
- **「〇〇する」という処理の実装**

### やること
- ビジネスルールの実行
- Entityの操作
- Repositoryからのデータ取得・保存
- Presenterへの結果通知

### やらないこと
- UI表示（それはPresenterとViewの仕事）
- Unity固有の処理（MonoBehaviourは使わない）
- 入力の検証（それはControllerの仕事）

### コード例

```csharp
using Zenject;

/// <summary>
/// BGM音量を更新するUseCase
/// ビジネスロジックの中核
/// </summary>
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    // Repositoryへの参照（データの永続化）
    private readonly IAudioSettingsRepository audioSettingsRepository;
    
    // Presenterへの参照（結果の通知）
    private readonly IVolumeUpdateOutputPort outputPort;
    
    // AudioManagerへの参照（実際の音量変更）
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

    /// <summary>
    /// BGM音量を更新する（ビジネスロジック）
    /// </summary>
    public void Execute(float volume)
    {
        // 1. 現在の設定を取得
        AudioSettings currentSettings = audioSettingsRepository.Load();
        
        // 2. ビジネスルール：音量の妥当性チェック
        if (!currentSettings.IsValidVolume(volume))
        {
            // 無効な値の場合はエラーを通知
            outputPort.OnVolumeUpdateFailed("音量は0～1の範囲で指定してください");
            return;
        }
        
        // 3. エンティティを更新
        currentSettings.UpdateBgmVolume(volume);
        
        // 4. ビジネスルール：音量が変更されたら実際のオーディオも変更
        audioManager.SetBgmVolume(volume);
        
        // 5. 変更を永続化
        audioSettingsRepository.Save(currentSettings);
        
        // 6. ビジネスルール：初めて音量を変更した場合はチュートリアル完了フラグを立てる
        if (!currentSettings.HasAdjustedVolumeOnce)
        {
            currentSettings.MarkVolumeAdjusted();
            audioSettingsRepository.Save(currentSettings);
            
            // チュートリアル完了を通知
            outputPort.OnTutorialCompleted("音量調整");
        }
        
        // 7. 結果をPresenterに通知
        outputPort.OnVolumeUpdated(currentSettings);
    }
}
```

**ポイント**:
- Interactorは「ビジネスロジックの塊」
- この例では以下のビジネスルールを実装：
  1. 音量の妥当性チェック
  2. 実際のオーディオ音量の変更
  3. データの永続化
  4. 初回調整時のチュートリアル完了処理
- MonoBehaviourを継承しない純粋なC#クラス
- Presenterを呼び出して結果を通知

### UseCaseの粒度

「どこまでを1つのUseCaseにするか？」という疑問があると思います。

**良い例**：「BGM音量を更新する」
```csharp
public class UpdateBgmVolumeInteractor
{
    public void Execute(float volume)
    {
        // 音量更新
        // 実際のオーディオ変更
        // 保存
        // 初回チェック
        // 結果通知
    }
}
```

**悪い例**：粒度が細かすぎる
```csharp
// これは細かすぎる
public class ValidateVolumeInteractor { }
public class SaveVolumeInteractor { }
public class ApplyVolumeInteractor { }
public class NotifyVolumeChangedInteractor { }
```

**目安**：「ユーザーの1つの意図」を1つのUseCaseにする
- 「BGM音量を変更したい」→ 1つのUseCase
- 「アイテムを拾いたい」→ 1つのUseCase（初回ダイアログ表示も含む）
- 「敵を攻撃したい」→ 1つのUseCase（ダメージ計算、経験値付与も含む）

---

## 4. Presenter（プレゼンター）

### 役割
- **Interactorからの結果を受け取る**
- **表示用のデータに変換する**
- **Viewに表示を指示する**

### やること
- ビジネスデータを表示用データに変換
- Viewのメソッドを呼び出して画面更新
- 複数のViewを協調させる

### やらないこと
- ビジネスロジック（それはInteractorの仕事）
- 入力の受付（それはControllerの仕事）
- Unity固有の実装（それはViewの仕事）

### コード例

```csharp
using Zenject;
using Cysharp.Threading.Tasks;

/// <summary>
/// 音量更新結果を表示するPresenter
/// Interactorからの結果を受け取り、Viewに表示を指示する
/// </summary>
public class VolumePresenter : IVolumeUpdateOutputPort
{
    private readonly SettingsView settingsView;
    private readonly ToastView toastView;
    private readonly TutorialView tutorialView;

    [Inject]
    public VolumePresenter(
        SettingsView settingsView,
        ToastView toastView,
        TutorialView tutorialView)
    {
        this.settingsView = settingsView;
        this.toastView = toastView;
        this.tutorialView = tutorialView;
    }

    /// <summary>
    /// Interactorから呼ばれる：音量更新成功
    /// </summary>
    public void OnVolumeUpdated(AudioSettings settings)
    {
        // ビジネスデータを表示用データに変換
        float bgmVolumePercent = settings.BgmVolume;
        float seVolumePercent = settings.SeVolume;
        
        // Viewに表示を指示
        settingsView.SetBgmVolume(bgmVolumePercent);
        settingsView.SetSeVolume(seVolumePercent);
        
        // トースト通知を表示
        string message = $"音量を変更しました";
        toastView.Show(message);
    }

    /// <summary>
    /// Interactorから呼ばれる：音量更新失敗
    /// </summary>
    public void OnVolumeUpdateFailed(string errorMessage)
    {
        // エラーメッセージを表示用に変換
        string displayMessage = $"エラー: {errorMessage}";
        
        // エラートーストを表示
        toastView.ShowError(displayMessage);
    }

    /// <summary>
    /// Interactorから呼ばれる：チュートリアル完了
    /// </summary>
    public void OnTutorialCompleted(string tutorialName)
    {
        // チュートリアル完了ダイアログを表示
        string title = "チュートリアル完了！";
        string message = $"「{tutorialName}」をマスターしました";
        
        tutorialView.ShowCompletionDialog(title, message);
    }
}
```

**ポイント**:
- Presenterは「表示の窓口」
- ビジネスデータ（AudioSettings）を表示用データ（パーセント表示）に変換
- 複数のViewを協調させる（SettingsView、ToastView、TutorialView）
- ビジネスロジックは一切書かない

### Presenterが必要な理由

「Interactorが直接Viewを呼べばいいのでは？」と思うかもしれません。
しかし、Presenterを挟むことで以下のメリットがあります：

1. **ビジネスロジックと表示ロジックの分離**
   ```csharp
   // Interactor: ビジネスデータを扱う
   AudioSettings settings = new AudioSettings(0.5f, 0.8f);
   
   // Presenter: 表示用に変換
   string bgmText = $"BGM: {Mathf.RoundToInt(settings.BgmVolume * 100)}%";
   ```

2. **複数のViewを協調させる**
   ```csharp
   public void OnVolumeUpdated(AudioSettings settings)
   {
       settingsView.SetBgmVolume(settings.BgmVolume);  // スライダー更新
       toastView.Show("音量を変更しました");            // トースト表示
       audioVisualizerView.Update(settings.BgmVolume);  // ビジュアライザー更新
   }
   ```

3. **Interactorが表示方法を知らなくて済む**
   - Interactorは「音量を更新した」という事実だけを通知
   - どう表示するかはPresenterが決める

---

## 5. InputPort と OutputPort（インターフェース）

### 役割
- **依存関係を逆転させる**
- **テスト時にモックに差し替える**

### InputPort

**定義**：Interactorが実装するインターフェース

```csharp
/// <summary>
/// BGM音量を更新するUseCaseのインターフェース
/// </summary>
public interface IUpdateBgmVolumeInputPort
{
    void Execute(float volume);
}

// Interactorが実装
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    public void Execute(float volume)
    {
        // 実装...
    }
}

// Controllerはインターフェース経由で依存
public class SettingsController
{
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeUseCase;
    
    [Inject]
    public SettingsController(IUpdateBgmVolumeInputPort updateBgmVolumeUseCase)
    {
        this.updateBgmVolumeUseCase = updateBgmVolumeUseCase;
    }
}
```

### OutputPort

**定義**：Presenterが実装するインターフェース

```csharp
/// <summary>
/// 音量更新結果を通知するインターフェース
/// </summary>
public interface IVolumeUpdateOutputPort
{
    void OnVolumeUpdated(AudioSettings settings);
    void OnVolumeUpdateFailed(string errorMessage);
    void OnTutorialCompleted(string tutorialName);
}

// Presenterが実装
public class VolumePresenter : IVolumeUpdateOutputPort
{
    public void OnVolumeUpdated(AudioSettings settings)
    {
        // 実装...
    }
    
    public void OnVolumeUpdateFailed(string errorMessage)
    {
        // 実装...
    }
    
    public void OnTutorialCompleted(string tutorialName)
    {
        // 実装...
    }
}

// Interactorはインターフェース経由で依存
public class UpdateBgmVolumeInteractor : IUpdateBgmVolumeInputPort
{
    private readonly IVolumeUpdateOutputPort outputPort;
    
    [Inject]
    public UpdateBgmVolumeInteractor(IVolumeUpdateOutputPort outputPort)
    {
        this.outputPort = outputPort;
    }
}
```

**ポイント**:
- Interactorは`IVolumeUpdateOutputPort`に依存（具体的なPresenterクラスには依存しない）
- これにより、Interactorは外側の層（Presenter）を知らずに済む
- テスト時にモックPresenterに差し替え可能

---

## 全体の処理フロー（再掲）

### 例：BGM音量を50%に変更する

```
【1. View】
ユーザーがスライダーを動かす
↓
bgmSlider.OnValueChangedAsObservable() が発火
↓ Observable<float> で 0.5f を流す

【2. Controller】
view.OnBgmVolumeChanged.Subscribe(volume => ...)
↓
OnBgmVolumeChanged(0.5f) が呼ばれる
↓
データを検証・加工: Mathf.Clamp01(0.5f) = 0.5f
↓
updateBgmVolumeUseCase.Execute(0.5f) を呼び出す

【3. Interactor】
Execute(0.5f) が呼ばれる
↓
ビジネスロジック実行:
  - 現在の設定を取得
  - 妥当性チェック
  - エンティティ更新
  - オーディオマネージャーに反映
  - データベースに保存
  - 初回チェック
↓
outputPort.OnVolumeUpdated(settings) を呼び出す

【4. Presenter】
OnVolumeUpdated(settings) が呼ばれる
↓
表示用データに変換:
  - settings.BgmVolume (0.5f) → 50%
↓
Viewに表示を指示:
  - settingsView.SetBgmVolume(0.5f)
  - toastView.Show("音量を変更しました")

【5. View】
SetBgmVolume(0.5f) が呼ばれる
↓
画面を更新:
  - スライダーの位置を更新
  - テキストを「BGM: 50%」に更新
```

---

## 依存関係の方向

```
View ──→ Controller ──→ InputPort ←── Interactor ──→ OutputPort ←── Presenter ──→ View
(外側)                    (境界)      (内側)          (境界)                    (外側)
```

**重要なポイント**:
- Interactor（内側）は、ControllerやPresenter（外側）を直接知らない
- InputPort/OutputPortというインターフェースを通じて依存関係を逆転させる
- これにより、Interactorは純粋なビジネスロジックに集中できる

---

## 各コンポーネントの比較表

| コンポーネント | 層 | 役割 | 依存先 | Unity依存 |
|--------------|-----|------|--------|-----------|
| **View** | FrameworkAndDriver | 入力受付・画面表示 | Controller, Presenter | ○（MonoBehaviour） |
| **Controller** | InterfaceAdapter | 入力の窓口・データ加工 | InputPort（Interactor） | × |
| **Interactor** | UseCase | ビジネスロジック実行 | Entity, OutputPort（Presenter） | × |
| **Presenter** | InterfaceAdapter | 表示の窓口・データ変換 | View | × |
| **InputPort** | UseCase | Interactorのインターフェース | - | × |
| **OutputPort** | UseCase | Presenterのインターフェース | - | × |

---

## よくある質問

### Q1: ControllerとPresenterの違いは？

**A**: 
- **Controller**：入力の窓口（View → Interactor）
- **Presenter**：出力の窓口（Interactor → View）

```
入力: View → Controller → Interactor
出力: Interactor → Presenter → View
```

### Q2: Interactorが複数のPresenterを呼んでもいい？

**A**: はい、問題ありません。

```csharp
public class UpdateBgmVolumeInteractor
{
    private readonly IVolumeUpdateOutputPort volumeOutputPort;
    private readonly IAchievementOutputPort achievementOutputPort;
    
    public void Execute(float volume)
    {
        // ビジネスロジック...
        
        // 複数のPresenterに通知
        volumeOutputPort.OnVolumeUpdated(settings);
        
        if (isFirstTimeAdjustment)
        {
            achievementOutputPort.OnAchievementUnlocked("音量調整マスター");
        }
    }
}
```

### Q3: 1つのControllerが複数のInteractorを呼んでもいい？

**A**: はい、問題ありません。

```csharp
public class SettingsController
{
    private readonly IUpdateBgmVolumeInputPort updateBgmVolumeUseCase;
    private readonly IUpdateSeVolumeInputPort updateSeVolumeUseCase;
    private readonly ISaveSettingsInputPort saveSettingsUseCase;
    
    private void OnSaveButtonClicked()
    {
        // 複数のUseCaseを呼び出す
        saveSettingsUseCase.Execute();
    }
}
```

### Q4: Presenterがビジネスロジックを持つのはNG？

**A**: はい、NGです。Presenterは「表示のための変換」のみを担当します。

**悪い例**:
```csharp
// NG: Presenterがビジネスロジックを持っている
public class VolumePresenter
{
    public void OnVolumeUpdated(float volume)
    {
        // NG: 音量の妥当性チェック（ビジネスロジック）
        if (volume < 0 || volume > 1)
        {
            return;
        }
        
        // NG: データベースへの保存（ビジネスロジック）
        SaveToDatabase(volume);
        
        // OK: 表示のための変換
        settingsView.SetBgmVolume(volume);
    }
}
```

**良い例**:
```csharp
// OK: Presenterは表示のための変換のみ
public class VolumePresenter
{
    public void OnVolumeUpdated(AudioSettings settings)
    {
        // OK: 表示用に変換
        float volumePercent = settings.BgmVolume;
        string displayText = $"BGM: {Mathf.RoundToInt(volumePercent * 100)}%";
        
        // OK: Viewに表示を指示
        settingsView.SetBgmVolume(volumePercent);
        settingsView.SetBgmVolumeText(displayText);
    }
}
```

### Q5: 小規模プロジェクトでもこの構成が必要？

**A**: プロジェクト規模に応じて調整してOKです。

**小規模プロジェクトの場合**:
- ControllerとViewを統合してもOK
- Presenterを省略してInteractorが直接Viewを呼んでもOK
- ただし、ビジネスロジックはInteractorに集約することを推奨

**大規模プロジェクトの場合**:
- 完全なクリーンアーキテクチャを採用
- テストの重要性が高いため、各層を明確に分離

---

## まとめ

### 各コンポーネントの役割（一言で）

- **View**: 「入力を受け取る」「画面に表示する」
- **Controller**: 「入力を加工してUseCaseに渡す」
- **Interactor（UseCase）**: 「ビジネスロジックを実行する」
- **Presenter**: 「結果を表示用に変換してViewに渡す」

### 処理の流れ（再掲）

```
Controller → Interactor → Presenter
   ↑                          ↓
  View ←──────────────────── View
```

### 依存の方向

```
外側（View, Controller, Presenter）
    ↓ 依存
内側（Interactor, Entity）
```

**内側は外側を知らない**ことが最も重要です。

---

## 次のステップ

1. まずは1つの機能（例：BGM音量変更）で実装してみる
2. 処理の流れを追って、各コンポーネントの役割を体感する
3. 慣れてきたら他の機能にも適用する
4. テストを書いて、アーキテクチャの恩恵を実感する

この構成に慣れると、「どこに何を書けばいいか」が明確になり、保守性の高いコードが書けるようになります。

