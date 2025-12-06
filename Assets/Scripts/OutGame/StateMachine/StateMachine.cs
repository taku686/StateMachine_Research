using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートマシーン本体（履歴管理機能付き）
    /// 遷移アニメーションをサポート
    /// </summary>
    public class StateMachine<TStateKey> where TStateKey : Enum
    {
        private readonly Dictionary<TStateKey, IState> _states = new();
        private readonly Stack<TStateKey> _stateHistory = new();
        private readonly HashSet<TStateKey> _rootStates = new();
        private readonly IStateTransitionAnimator _transitionAnimator;
        private IState _currentState;
        private TStateKey _currentStateKey;
        private bool _isTransitioning;

        public TStateKey CurrentStateKey => _currentStateKey;
        public bool CanGoBack => _stateHistory.Count > 0;
        public bool IsTransitioning => _isTransitioning;

        /// <summary>
        /// コンストラクタ（遷移アニメーターを注入）
        /// </summary>
        [Inject]
        public StateMachine([InjectOptional] IStateTransitionAnimator transitionAnimator = null)
        {
            _transitionAnimator = transitionAnimator;
        }

        /// <summary>
        /// ステートを登録
        /// </summary>
        /// <param name="key">ステートキー</param>
        /// <param name="state">ステート</param>
        /// <param name="isRootState">ルートステート（履歴をクリアする基準画面）かどうか</param>
        public void RegisterState(TStateKey key, IState state, bool isRootState = false)
        {
            if (_states.ContainsKey(key))
            {
                Debug.LogWarning($"State {key} is already registered.");
                return;
            }

            _states[key] = state;

            if (isRootState)
            {
                _rootStates.Add(key);
            }
        }

        /// <summary>
        /// ステートを変更（履歴に追加）
        /// アニメーション→プリロード→退出→クリーンアップ→入場→アニメーションの順で実行
        /// </summary>
        public async UniTask ChangeState(TStateKey newStateKey, bool addToHistory = true)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("State transition is already in progress.");
                return;
            }

            if (!_states.ContainsKey(newStateKey))
            {
                Debug.LogError($"State {newStateKey} is not registered.");
                return;
            }

            _isTransitioning = true;

            try
            {
                var newState = _states[newStateKey];

                Debug.Log($"=== State Transition: {_currentStateKey} → {newStateKey} ===");

                // ========== Phase 1: 遷移開始アニメーション ==========
                if (_transitionAnimator != null && _currentState != null)
                {
                    Debug.Log("[Phase 1] Playing exit transition animation...");
                    await _transitionAnimator.PlayExitTransition();
                }

                // ========== Phase 2: ローディング表示 + 新Stateの準備 ==========
                Debug.Log("[Phase 2] Preparing new state...");
                
                if (_transitionAnimator != null)
                {
                    await _transitionAnimator.ShowLoading();
                }

                await UniTask.WhenAll(
                    newState.OnPreloadAssets(),
                    newState.OnLoadData()
                );

                if (_transitionAnimator != null)
                {
                    await _transitionAnimator.HideLoading();
                }

                // ========== Phase 3: 現Stateの終了 ==========
                if (_currentState != null)
                {
                    Debug.Log($"[Phase 3] Exiting current state: {_currentStateKey}");

                    if (addToHistory && !_rootStates.Contains(_currentStateKey))
                    {
                        _stateHistory.Push(_currentStateKey);
                    }

                    await _currentState.OnExit();

                    await UniTask.WhenAll(
                        _currentState.OnCleanupAssets(),
                        _currentState.OnCleanupData()
                    );
                }

                if (_rootStates.Contains(newStateKey))
                {
                    _stateHistory.Clear();
                }

                // ========== Phase 4: 新Stateの開始 ==========
                Debug.Log($"[Phase 4] Entering new state: {newStateKey}");
                _currentState = newState;
                _currentStateKey = newStateKey;
                await _currentState.OnEnter();

                // ========== Phase 5: 遷移完了アニメーション ==========
                if (_transitionAnimator != null)
                {
                    Debug.Log("[Phase 5] Playing enter transition animation...");
                    await _transitionAnimator.PlayEnterTransition();
                }

                Debug.Log($"=== State Transition Complete: {newStateKey} ===");
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        /// 前のステートに戻る
        /// </summary>
        public async UniTask GoBack()
        {
            if (!CanGoBack)
            {
                Debug.LogWarning("No state history to go back to.");
                return;
            }

            var previousStateKey = _stateHistory.Pop();
            await ChangeState(previousStateKey, addToHistory: false);
        }

        /// <summary>
        /// 履歴をクリア
        /// </summary>
        public void ClearHistory()
        {
            _stateHistory.Clear();
        }

        /// <summary>
        /// 現在のステートの更新処理を実行
        /// </summary>
        public void Update()
        {
            _currentState?.OnUpdate();
        }
    }
}
