using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace AvatarViewer
{
    public class MainThreadDispatcher : MonoBehaviour
    {

        private static ConcurrentQueue<Action> updateActions = new();

        void Update()
        {
            if (updateActions.TryDequeue(out var action))
                action.Invoke();
        }

        public static void AddOnUpdate(Action action) => updateActions.Enqueue(action);
    }
}
