using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートマシーン本体（履歴管理機能付き）
    /// </summary>
    public class StateMachine<TStateKey> where TStateKey : Enum
    {
        private readonly Dictionary<TStateKey, IState> _states = new();
        private readonly Stack<TStateKey> _stateHistory = new();
        private readonly HashSet<TStateKey> _rootStates = new();
        private IState _currentState;
        private TStateKey _currentStateKey;

        public TStateKey CurrentStateKey => _currentStateKey;
        public bool CanGoBack => _stateHistory.Count > 0;

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
        /// </summary>
        public async UniTask ChangeState(TStateKey newStateKey, bool addToHistory = true)
        {
            if (!_states.ContainsKey(newStateKey))
            {
                Debug.LogError($"State {newStateKey} is not registered.");
                return;
            }

            // 現在のステートから退出
            if (_currentState != null)
            {
                // 履歴に追加（ルートステートでない場合のみ）
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

