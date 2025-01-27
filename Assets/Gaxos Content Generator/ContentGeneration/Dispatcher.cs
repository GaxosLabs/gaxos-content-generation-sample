using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContentGeneration
{
    public interface IDispatcher
    {
        void ToMainThread(Action action);
        Coroutine StartCoroutine(IEnumerator routine);
    }
    public class Dispatcher : MonoBehaviour, IDispatcher
    {
        [RuntimeInitializeOnLoadMethod]
        static void Instantiate()
        {
            if(instance == null)
            {
                var gameObject = new GameObject(nameof(Dispatcher));
                DontDestroyOnLoad(gameObject);
                instance = gameObject.AddComponent<Dispatcher>();
            }
        }
        
        public static IDispatcher instance { get; private set; }
        #if UNITY_EDITOR
        public static void UseEditorDispatcher(IDispatcher dispatcher)
        {
            if (!Application.isPlaying)
            {
                instance = dispatcher;
            }
        }
        #endif
        
        public void ToMainThread(Action action)
        {
            _delayedActions.Add(action);
        }
        
        readonly List<Action> _delayedActions = new();
        void Update()
        {
            if (_delayedActions.Count > 0)
            {
                var aux = _delayedActions.ToArray();
                _delayedActions.Clear();
                foreach (var action in aux)
                {
                    action.Invoke();
                }
            }
        }
    }
}