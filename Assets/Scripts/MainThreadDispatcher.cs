using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace AvatarViewer
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        public static MainThreadDispatcher Instance { get; private set; }

        private ConcurrentQueue<Action> updateActions = new();

        private void Awake()
        {
            if (Instance != null && Instance == this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        private void Update()
        {
            if (updateActions.TryDequeue(out var action))
                action.Invoke();
        }

        public void AddOnUpdate(Action action) => updateActions.Enqueue(action);
    }
}
