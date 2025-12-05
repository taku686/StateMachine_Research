# Gitのcherry-pick、rebase、stashの説明

## 1. **stash（一時保存）**

### 何をする機能？
作業中の変更を一時的に退避する機能です。コミットせずに別のブランチに切り替えたい時に使います。

### よくある使い方
```bash
# 変更を一時保存
git stash

# 別ブランチに切り替えて作業
git checkout other-branch

# 元のブランチに戻って変更を復元
git checkout original-branch
git stash pop
```

### 具体例
- **シナリオ**: `feature`ブランチで作業中、急に`main`ブランチのバグを直す必要が出た
  1. `git stash` で変更を退避
  2. `git checkout main` で切り替え
  3. バグ修正してコミット
  4. `git checkout feature` に戻る
  5. `git stash pop` で変更を復元

### `git stash pop` と `git stash apply` の違い

**重要な違い**：
- **`git stash pop`** - 変更を適用して、**stashから削除する**
- **`git stash apply`** - 変更を適用するが、**stashには残す**

#### 具体例で理解する

```bash
# 複数のstashがある場合
git stash list
# stash@{0}: WIP on feature: 最新の変更
# stash@{1}: WIP on feature: 2つ目の変更
# stash@{2}: WIP on feature: 3つ目の変更

# applyの場合：適用するがstashに残る
git stash apply stash@{0}
# → 変更が適用される
# → stash@{0}はまだ残っている

# popの場合：適用してstashから削除
git stash pop stash@{0}
# → 変更が適用される
# → stash@{0}は削除される
# → 次のstashがstash@{0}になる（stash@{1}がstash@{0}に繰り上がる）
```

#### 使い分けの目安

- **`git stash pop`** を使う場合：
  - 一度だけ適用すれば良い時
  - 通常の使い方（適用して削除）
  - 例：作業を一時中断して、戻ってきた時に復元

- **`git stash apply`** を使う場合：
  - 同じ変更を複数のブランチに適用したい時
  - 適用後にstashを残しておきたい時
  - 例：同じ修正を`feature-A`と`feature-B`の両方に適用したい

### 便利なコマンド
- `git stash list` - 保存した変更の一覧を表示
- `git stash apply` - 最新の変更を適用（削除しない）
- `git stash pop` - 最新の変更を適用して削除
- `git stash drop` - 最新の変更を削除（適用しない）
- `git stash clear` - すべてのstashを削除

---

## 2. **cherry-pick（特定のコミットを選んで取り込む）**

### 何をする機能？
別のブランチにある特定のコミットだけを、現在のブランチに取り込む機能です。

### よくある使い方
```bash
# コミットIDを指定して取り込む
git cherry-pick <コミットID>

# 複数のコミットを一度に
git cherry-pick <コミットID1> <コミットID2>
```

### 具体例
- **シナリオ**: `feature`ブランチのコミットAだけを`main`に取り込みたい
  1. `git log` でコミットIDを確認
  2. `git checkout main` で`main`に切り替え
  3. `git cherry-pick abc123` でコミットAを取り込む

### 注意点
- 同じ変更を複数のブランチに適用できる
- コミットIDが変わる（新しいコミットとして作成される）
- コンフリクトが発生することがある

---

## 3. **rebase（履歴を書き換える）**

### 何をする機能？
ブランチの基点を別のコミットに移動させて、履歴を一直線に整理する機能です。

### よくある使い方
```bash
# 現在のブランチをmainブランチの最新にrebase
git checkout feature
git rebase main

# 対話的にコミットを編集
git rebase -i HEAD~3
```

### 具体例
- **シナリオ**: `feature`ブランチを`main`の最新に合わせたい
  1. `git checkout feature`
  2. `git rebase main`
  3. 必要に応じてコンフリクトを解決
  4. `git rebase --continue`

### mergeとの違い
- **merge**: 履歴が分岐したまま（マージコミットができる）
- **rebase**: 履歴が一直線になる（見やすくなる）

### 注意点
- 既にpushしたコミットをrebaseすると履歴が変わる
- チームで共有しているブランチでは注意が必要
- `git rebase --abort` で中断・取り消し可能

---

## まとめ

| 機能 | 用途 | 履歴への影響 |
|------|------|------------|
| **stash** | 作業中の変更を一時保存 | 履歴に影響なし |
| **cherry-pick** | 特定のコミットだけ取り込む | 新しいコミットを作成 |
| **rebase** | ブランチの基点を移動して履歴を整理 | 既存のコミットを書き換える可能性 |

### 使い分けの目安
- 作業を一時中断したい → **stash**
- 特定の修正だけ別ブランチに適用したい → **cherry-pick**
- ブランチの履歴を整理したい → **rebase**
