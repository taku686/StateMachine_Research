# 🎮 放置系横スクロールゲーム 実装ロードマップ

> プラットフォーム: Android  
> ジャンル: 放置系横スクロールRPG  
> 最終更新: 2025年12月6日

---

## 📦 モジュール構成

```
Assets/Scripts/
├── Common/          # 🔄 共通モジュール（他ゲームでも再利用可能）
├── OutGame/         # 🏠 アウトゲーム（メニュー/UI）
└── InGame/          # ⚔️ インゲーム（バトル/ゲームプレイ）
```

| モジュール | 説明 | 再利用性 |
|-----------|------|----------|
| **Common** | 認証、課金、通知など汎用機能 | ⭐⭐⭐ 他ゲームでそのまま使用可能 |
| **OutGame** | ホーム、ガチャ、編成など | ⭐⭐ 一部カスタマイズで再利用可能 |
| **InGame** | バトル、放置システムなど | ⭐ ゲーム固有（ほぼ作り直し） |

---

# 🔄 Common（共通モジュール）

> **目的**: どのソシャゲでも必要になる汎用機能。パッケージ化して他プロジェクトにインポート可能。

## 1. 認証/アカウント (Auth)

```
Common/Auth/
├── IAuthService.cs
├── GuestAuthService.cs
├── GoogleAuthService.cs
├── AppleAuthService.cs
├── AccountLinkingService.cs
└── TokenManager.cs
```

### ログイン
- [ ] ゲストログイン（UUID生成）
- [ ] Google Play Games ログイン
- [ ] 自動ログイン（トークン保持）
- [ ] セッション管理

### アカウント管理
- [ ] アカウント連携UI
- [ ] 引き継ぎコード発行/入力
- [ ] アカウント削除（GDPR対応）

---

## 2. 課金/IAP (Billing)

```
Common/Billing/
├── IBillingService.cs
├── GooglePlayBillingService.cs
├── ProductCatalog.cs
├── PurchaseValidator.cs
└── ReceiptVerifier.cs
```

### 課金基盤
- [ ] Google Play Billing Library 統合
- [ ] 商品情報取得
- [ ] 購入フロー
- [ ] レシート検証（サーバー側）
- [ ] 購入復元
- [ ] 定期購入（月額パス）

### ショップUI
- [ ] 汎用ショップUI
- [ ] 商品カード表示
- [ ] 購入確認ダイアログ
- [ ] 購入完了演出

---

## 3. 通知 (Notification)

```
Common/Notification/
├── INotificationService.cs
├── LocalNotificationService.cs
├── PushNotificationService.cs
└── NotificationScheduler.cs
```

### プッシュ通知
- [ ] FCM 統合
- [ ] トピック購読
- [ ] ペイロード処理
- [ ] ディープリンク対応

### ローカル通知
- [ ] 通知スケジュール
- [ ] 通知キャンセル
- [ ] スタミナ回復通知
- [ ] イベント開始通知

---

## 4. 分析/ログ (Analytics)

```
Common/Analytics/
├── IAnalyticsService.cs
├── FirebaseAnalyticsService.cs
├── CustomEventLogger.cs
└── UserPropertyManager.cs
```

### イベントトラッキング
- [ ] Firebase Analytics 統合
- [ ] カスタムイベント送信
- [ ] ユーザープロパティ設定
- [ ] スクリーンビュー追跡

### クラッシュレポート
- [ ] Firebase Crashlytics 統合
- [ ] カスタムキー設定
- [ ] 非致命的エラーログ

---

## 5. リモートコンフィグ (RemoteConfig)

```
Common/RemoteConfig/
├── IRemoteConfigService.cs
├── FirebaseRemoteConfigService.cs
└── ConfigValues.cs
```

### 機能
- [ ] Firebase Remote Config 統合
- [ ] デフォルト値設定
- [ ] フェッチ/アクティベート
- [ ] A/Bテストパラメータ

---

## 6. ネットワーク (Network)

```
Common/Network/
├── IApiClient.cs
├── RestApiClient.cs
├── WebSocketClient.cs
├── RetryPolicy.cs
├── NetworkMonitor.cs
└── ResponseModels/
```

### HTTP通信
- [ ] REST API クライアント
- [ ] リクエスト/レスポンスシリアライズ
- [ ] 認証ヘッダー自動付与
- [ ] リトライ/タイムアウト
- [ ] エラーハンドリング

### リアルタイム通信
- [ ] WebSocket クライアント
- [ ] 再接続ロジック
- [ ] ハートビート

### オフライン対応
- [ ] ネットワーク状態監視
- [ ] オフラインモード
- [ ] リクエストキューイング

---

## 7. データ永続化 (Storage)

```
Common/Storage/
├── IStorageService.cs
├── LocalStorageService.cs
├── EncryptedStorageService.cs
└── CloudSaveService.cs
```

### ローカル保存
- [x] PlayerPrefs ラッパー
- [ ] ファイル保存
- [ ] AES暗号化保存
- [ ] データマイグレーション

### クラウド保存
- [ ] Google Play Games セーブ
- [ ] サーバー同期
- [ ] コンフリクト解決

---

## 8. アセット管理 (Asset)

```
Common/Asset/
├── IAssetLoader.cs
├── AddressableLoader.cs
├── AssetBundleManager.cs
├── DownloadManager.cs
└── CacheManager.cs
```

### Addressables
- [x] 基本ロード/アンロード
- [x] プリロードシステム
- [ ] ダウンロード進捗UI
- [ ] バンドル分割戦略
- [ ] キャッシュ管理

### アセットバージョン
- [ ] カタログ更新チェック
- [ ] 差分ダウンロード
- [ ] 強制アップデート判定

---

## 9. UI共通 (UI)

```
Common/UI/
├── Dialog/
│   ├── IDialogService.cs
│   ├── ConfirmDialog.cs
│   ├── AlertDialog.cs
│   └── LoadingDialog.cs
├── Toast/
├── Transition/
│   ├── ITransitionAnimator.cs
│   └── FadeTransitionAnimator.cs
└── Components/
    ├── SafeAreaAdjuster.cs
    ├── AspectRatioFitter.cs
    └── ScrollOptimizer.cs
```

### ダイアログ
- [ ] 確認ダイアログ
- [ ] アラートダイアログ
- [ ] 入力ダイアログ
- [ ] ダイアログキュー管理

### 共通コンポーネント
- [x] 画面遷移アニメーション
- [x] ローディング表示
- [ ] トースト通知
- [ ] セーフエリア対応
- [ ] ノッチ対応

---

## 10. 時間管理 (Time)

```
Common/Time/
├── ITimeService.cs
├── ServerTimeService.cs
└── CountdownTimer.cs
```

### サーバー時間
- [ ] サーバー時間同期
- [ ] タイムゾーン処理
- [ ] 時間改ざん検知

### タイマー
- [ ] カウントダウンUI
- [ ] イベント開始/終了判定
- [ ] デイリーリセット判定

---

# 🏠 OutGame（アウトゲームモジュール）

> **目的**: ゲームプレイ以外のメニュー画面。装備強化、ガチャなど。

## 現在の実装状況

```
OutGame/
├── Application/       # ✅ 実装済み
├── Domain/           # ✅ 実装済み
├── Infrastructure/   # ✅ 実装済み
├── Presentation/     # ✅ 実装済み
├── StateMachine/     # ✅ 実装済み
├── States/           # ✅ 実装済み
└── Installers/       # ✅ 実装済み
```

## 1. ホーム画面

### 基本UI
- [x] ホーム画面表示
- [x] プレイヤーレベル表示
- [x] ゴールド表示
- [ ] ジェム表示
- [ ] スタミナ表示
- [ ] メインメニューボタン群

### ショートカット
- [ ] バトル開始ボタン
- [ ] 放置報酬受取ボタン
- [ ] お知らせバッジ

---

## 2. キャラクター/プレイヤー

```
OutGame/Features/Character/
├── Views/
├── Controllers/
├── States/
└── Models/
```

### プレイヤー情報
- [x] プレイヤーレベル
- [ ] 経験値/次レベルまで
- [ ] ステータス一覧
- [ ] 称号システム

### キャラ管理
- [ ] キャラ一覧画面
- [ ] キャラ詳細画面
- [ ] ソート/フィルター

---

## 3. 装備システム ⭐（本作のメイン要素）

```
OutGame/Features/Equipment/
├── Domain/
│   ├── Equipment.cs
│   ├── EquipmentSlot.cs
│   ├── EquipmentRarity.cs
│   └── EquipmentStat.cs
├── Views/
│   ├── EquipmentListView.cs
│   ├── EquipmentDetailView.cs
│   └── EquipmentEnhanceView.cs
├── Controllers/
└── States/
```

### 装備一覧
- [ ] 装備インベントリ画面
- [ ] 装備スロット表示（武器/防具/アクセサリ）
- [ ] 装備比較表示
- [ ] ソート（レアリティ/レベル/ステータス）

### 装備強化
- [ ] レベルアップ（素材消費）
- [ ] 強化成功/失敗演出
- [ ] 強化費用表示
- [ ] 一括強化

### 装備進化
- [ ] 進化条件表示
- [ ] 進化素材確認
- [ ] 進化演出

### 装備合成
- [ ] 同レアリティ合成
- [ ] ランダムオプション付与
- [ ] 合成履歴

### 装備分解
- [ ] 不要装備選択
- [ ] 獲得素材プレビュー
- [ ] 一括分解

---

## 4. ガチャ

```
OutGame/Features/Gacha/
├── Domain/
│   ├── GachaBanner.cs
│   ├── GachaPool.cs
│   └── GachaResult.cs
├── Views/
│   ├── GachaTopView.cs
│   ├── GachaPullView.cs
│   └── GachaResultView.cs
└── Controllers/
```

### ガチャUI
- [ ] バナー一覧
- [ ] 排出確率表示
- [ ] 単発/10連ボタン
- [ ] 天井カウント表示

### ガチャ演出
- [ ] 引き演出アニメーション
- [ ] レアリティ別演出
- [ ] 結果表示画面
- [ ] スキップ機能

### ガチャ種類
- [ ] 装備ガチャ
- [ ] 無料ガチャ（広告視聴）
- [ ] チケットガチャ

---

## 5. ミッション/実績

```
OutGame/Features/Mission/
├── Domain/
│   ├── Mission.cs
│   ├── MissionType.cs
│   └── MissionReward.cs
├── Views/
└── Controllers/
```

### デイリーミッション
- [ ] ミッション一覧
- [ ] 進捗表示
- [ ] 報酬受取
- [ ] 全報酬一括受取

### 実績
- [ ] 実績一覧
- [ ] 達成条件表示
- [ ] 累計実績

---

## 6. お知らせ/イベント

### お知らせ
- [ ] お知らせ一覧
- [ ] 既読/未読管理
- [ ] 画像付きお知らせ

### イベントバナー
- [ ] イベントカルーセル
- [ ] 開催期間表示
- [ ] イベント詳細遷移

### ログインボーナス
- [ ] ログボ画面
- [ ] カレンダー表示
- [ ] 報酬受取演出

---

## 7. 設定

### 実装済み
- [x] BGM音量
- [x] SE音量
- [x] 設定画面UI

### 追加項目
- [ ] ボイス音量
- [ ] 画質設定
- [ ] フレームレート設定
- [ ] 通知設定
- [ ] 言語設定
- [ ] アカウント連携
- [ ] 引き継ぎ
- [ ] お問い合わせ
- [ ] 利用規約/プラポリ

---

# ⚔️ InGame（インゲームモジュール）

> **目的**: 実際のゲームプレイ部分。放置横スクロールバトル。

```
InGame/
├── Battle/           # バトルシステム
├── Character/        # キャラクター制御
├── Enemy/            # 敵キャラクター
├── Stage/            # ステージ管理
├── Idle/             # 放置システム
├── UI/               # バトルUI
└── Installers/
```

## 1. 横スクロールバトル

```
InGame/Battle/
├── BattleManager.cs
├── BattleState.cs
├── WaveController.cs
└── SpawnSystem.cs
```

### バトル基盤
- [ ] バトルシーン管理
- [ ] Wave（敵の波）システム
- [ ] 敵スポーン制御
- [ ] バトルタイマー

### 自動進行
- [ ] 自動移動（横スクロール）
- [ ] 自動攻撃
- [ ] 自動スキル発動

---

## 2. プレイヤーキャラクター

```
InGame/Character/
├── PlayerController.cs
├── PlayerStats.cs
├── PlayerAnimator.cs
├── SkillSystem.cs
└── EquipmentEffect.cs
```

### 移動/アニメーション
- [ ] 横方向自動移動
- [ ] 走りアニメーション
- [ ] 攻撃アニメーション
- [ ] 被ダメージアニメーション
- [ ] 死亡アニメーション

### 戦闘システム
- [ ] 通常攻撃
- [ ] スキル（装備依存）
- [ ] 必殺技ゲージ
- [ ] 装備効果反映

### ステータス
- [ ] HP/MP管理
- [ ] 攻撃力/防御力
- [ ] クリティカル
- [ ] 装備ステータス反映

---

## 3. 敵キャラクター

```
InGame/Enemy/
├── EnemyController.cs
├── EnemyAI.cs
├── EnemyStats.cs
└── EnemyData/
```

### 敵AI
- [ ] 移動パターン
- [ ] 攻撃パターン
- [ ] ターゲット選択

### 敵種類
- [ ] 通常敵
- [ ] エリート敵
- [ ] ボス敵
- [ ] 敵データマスター

### ドロップ
- [ ] ドロップテーブル
- [ ] ドロップ演出
- [ ] 自動回収

---

## 4. ステージ/レベル

```
InGame/Stage/
├── StageManager.cs
├── StageData.cs
├── BackgroundScroller.cs
└── ObstacleSpawner.cs
```

### ステージ管理
- [ ] ステージ選択
- [ ] 難易度設定
- [ ] 背景スクロール
- [ ] 進捗表示

### 環境
- [ ] 障害物生成
- [ ] 背景パララックス
- [ ] エリア遷移演出

---

## 5. 放置システム ⭐（本作のメイン要素）

```
InGame/Idle/
├── IdleManager.cs
├── IdleRewardCalculator.cs
├── OfflineProgressService.cs
└── IdleBonus.cs
```

### オフライン報酬
- [ ] 放置時間計算
- [ ] 報酬計算ロジック
- [ ] 最大放置時間制限
- [ ] 放置報酬画面

### 自動周回
- [ ] ステージループ
- [ ] 自動リトライ
- [ ] ドロップ累計表示
- [ ] 周回数表示

### 放置ボーナス
- [ ] VIP/課金ボーナス
- [ ] 広告視聴ボーナス
- [ ] 放置時間延長

---

## 6. バトルUI

```
InGame/UI/
├── BattleHUD.cs
├── DamagePopup.cs
├── HealthBar.cs
├── SkillButton.cs
└── ResultScreen.cs
```

### HUD
- [ ] HPバー（自分）
- [ ] 敵HPバー
- [ ] スキルボタン
- [ ] 必殺技ボタン
- [ ] Wave表示
- [ ] タイマー表示

### エフェクト
- [ ] ダメージポップアップ
- [ ] ヒットエフェクト
- [ ] スキルエフェクト
- [ ] クリティカル演出

### リザルト
- [ ] クリア/失敗画面
- [ ] ドロップ一覧
- [ ] 経験値獲得
- [ ] 続ける/戻るボタン

---

## 7. 倍速/スキップ

### 再生速度
- [ ] 1x / 2x / 3x 切り替え
- [ ] 設定保存

### スキップ
- [ ] スキップチケット消費
- [ ] 即時完了
- [ ] 結果表示

---

# 🎯 実装優先度

## Phase 1: プロトタイプ（2-3週間）

### Common
- [ ] ゲストログイン
- [ ] ローカルセーブ

### InGame
- [ ] 横スクロール移動
- [ ] 基本攻撃
- [ ] 敵スポーン
- [ ] 簡易バトル

### OutGame
- [x] 画面遷移基盤
- [ ] 仮のホーム画面

---

## Phase 2: コアループ（3-4週間）

### InGame
- [ ] 放置報酬システム
- [ ] Wave/ステージシステム
- [ ] ドロップシステム

### OutGame
- [ ] 装備一覧
- [ ] 装備強化
- [ ] 装備装着

---

## Phase 3: マネタイズ（2-3週間）

### Common
- [ ] Google Play Billing
- [ ] 広告SDK（AdMob/Unity Ads）

### OutGame
- [ ] ガチャシステム
- [ ] ショップUI

---

## Phase 4: リテンション（2-3週間）

### Common
- [ ] プッシュ通知
- [ ] 分析基盤

### OutGame
- [ ] ログインボーナス
- [ ] デイリーミッション
- [ ] お知らせ機能

---

## Phase 5: ポリッシュ（2週間）

- [ ] 演出強化
- [ ] サウンド追加
- [ ] バグ修正
- [ ] パフォーマンス最適化

---

# 📝 更新履歴

| 日付 | 更新内容 |
|------|----------|
| 2025/12/06 | 初版作成（モジュール分割対応） |


