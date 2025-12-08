using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace InGame.StateMachine
{
    /// <summary>
    /// InGameのStateMachine
    /// </summary>
    public class InGameStateMachine
    {
        private readonly Dictionary<InGameStateKey, IInGameState> states = new Dictionary<InGameStateKey, IInGameState>();
        private IInGameState currentState;
        private InGameStateKey currentStateKey;

        public InGameStateKey CurrentStateKey => currentStateKey;

        public void RegisterState(InGameStateKey key, IInGameState state)
        {
            if (!states.ContainsKey(key))
            {
                states.Add(key, state);
                Debug.Log($"Registered state: {key}");
            }
        }

        public async UniTask ChangeState(InGameStateKey newStateKey)
        {
            if (!states.TryGetValue(newStateKey, out var newState))
            {
                Debug.LogError($"State {newStateKey} not found!");
                return;
            }

            Debug.Log($"State transition: {currentStateKey} → {newStateKey}");

            // 現在のステート終了
            if (currentState != null)
            {
                await currentState.OnExit();
            }

            // 新しいステート開始
            currentState = newState;
            currentStateKey = newStateKey;
            await currentState.OnEnter();
        }

        public void Update()
        {
            currentState?.OnUpdate();
        }
    }
}

