# 📚 ドキュメント一覧

このフォルダには、プロジェクトの各種ドキュメントが格納されています。

---

## 📋 ドキュメント構成

### 🏗️ アーキテクチャ・設計

- **[CleanArchitecture.md](./CleanArchitecture.md)**
  - クリーンアーキテクチャの完全ガイド
  - 4層構造、処理フロー、各コンポーネントの詳細解説
  - デザインパターン（State Machine、DI、Repository、Factory、Observer）
  - 実装時のベストプラクティス
  - 新規画面の追加手順

### 🗺️ プロジェクト管理

- **[ProjectRoadmap.md](./ProjectRoadmap.md)**
  - プロジェクト全体のロードマップと進捗状況
  - Common/OutGame/InGameモジュールの機能リスト
  - Phase 1〜5の実装計画
  - 優先度付きタスクリスト
  - 進捗率の可視化

### 🛠️ 開発環境・ツール

- **[UnityEditorSetup.md](./UnityEditorSetup.md)**
  - Unity Editor上での作業手順書
  - ProjectContext/SceneContext の設定方法
  - Bootstrap/Transition/InGame/OutGameシーンの設定
  - 動作確認手順とトラブルシューティング

- **[GitReference.md](./GitReference.md)**
  - Git操作のリファレンス
  - cherry-pick、rebase、stash の使い方
  - 実用例とコマンド集

### 📄 その他

- **[Sample_Notion.txt](./Sample_Notion.txt)**
  - 元となったNotionドキュメント（アーカイブ）

---

## 🎯 ドキュメントの使い方

### 新規メンバーが参加したら

1. **[CleanArchitecture.md](./CleanArchitecture.md)** を読んで、プロジェクトのアーキテクチャを理解
2. **[ProjectRoadmap.md](./ProjectRoadmap.md)** で現在の進捗と次のタスクを確認
3. **[UnityEditorSetup.md](./UnityEditorSetup.md)** に従ってUnity Editorを設定

### 新機能を実装する時

1. **[ProjectRoadmap.md](./ProjectRoadmap.md)** で実装する機能を確認
2. **[CleanArchitecture.md](./CleanArchitecture.md)** の「新規画面の追加手順」を参照
3. 実装完了後、**[ProjectRoadmap.md](./ProjectRoadmap.md)** のチェックボックスを更新

### トラブルが発生したら

1. **[UnityEditorSetup.md](./UnityEditorSetup.md)** のトラブルシューティングセクションを確認
2. **[CleanArchitecture.md](./CleanArchitecture.md)** の実装時のベストプラクティスを確認

---

## 📝 ドキュメントの更新ルール

### CleanArchitecture.md
- アーキテクチャの変更や新しいパターンの導入時に更新
- ベストプラクティスの追加時に更新

### ProjectRoadmap.md
- 機能実装完了時にチェックボックスを更新
- 進捗率を再計算して更新
- 変更履歴セクションに記録

### UnityEditorSetup.md
- Unity Editorの設定手順が変わった時に更新
- 新しいトラブルシューティング情報を追加

---

## 🔗 関連リンク

### 外部リソース

- [Zenject (Extenject) - GitHub](https://github.com/modesttree/Zenject)
- [UniTask - GitHub](https://github.com/Cysharp/UniTask)
- [R3 - GitHub](https://github.com/Cysharp/R3)
- [DOTween - Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

### 参考記事

- [Unity ゲーム開発で使うクリーンアーキテクチャの勘所](https://zenn.dev/fig_codefactory/articles/09122552d41c1a)
- [Unityを利用した大規模なゲーム開発にクリーンアーキテクチャを採用した話](https://developers.wonderpla.net/entry/2021/02/18/121932)

---

## 📊 ドキュメント整理履歴

| 日付 | 変更内容 |
|------|----------|
| 2025/12/13 | ドキュメントを統合・整理。重複ファイルを削除し、5ファイルに集約 |
| 2025/12/12 | プロジェクト進捗レポート作成 |
| 2025/12/06 | プロジェクト開始、初期ドキュメント作成 |

---

整理前は14個のMarkdownファイルがありましたが、内容の重複を解消し、**5個の明確な役割を持つドキュメント**に整理しました。

- ✅ **アーキテクチャ**: 1ファイル（統合）
- ✅ **プロジェクト管理**: 1ファイル（統合）
- ✅ **開発環境**: 1ファイル
- ✅ **Git参考**: 1ファイル
- ✅ **アーカイブ**: 1ファイル

