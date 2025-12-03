using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.StateMachine
{
    /// <summary>
    /// ステートマシーン本体
    /// </summary>
    public class StateMachine<TStateKey> where TStateKey : Enum
    {
        private readonly Dictionary<TStateKey, IState> _states = new();
        private IState _currentState;
        private TStateKey _currentStateKey;

        public TStateKey CurrentStateKey => _currentStateKey;

        /// <summary>
        /// ステートを登録
        /// </summary>
        public void RegisterState(TStateKey key, IState state)
        {
            if (_states.ContainsKey(key))
            {
                Debug.LogWarning($"State {key} is already registered.");
                return;
            }

            _states[key] = state;
        }

        /// <summary>
        /// ステートを変更
        /// </summary>
        public async UniTask ChangeState(TStateKey newStateKey)
        {
            if (!_states.ContainsKey(newStateKey))
            {
                Debug.LogError($"State {newStateKey} is not registered.");
                return;
            }

            // 現在のステートから退出
            if (_currentState != null)
            {
                await _currentState.OnExit();
            }

            // 新しいステートに入る
            _currentState = _states[newStateKey];
            _currentStateKey = newStateKey;
            await _currentState.OnEnter();
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

