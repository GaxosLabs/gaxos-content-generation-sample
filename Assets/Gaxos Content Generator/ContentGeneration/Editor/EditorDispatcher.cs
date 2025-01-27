using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace ContentGeneration.Editor
{
    public class EditorDispatcher : IDispatcher
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (!Application.isPlaying)
            {
                Dispatcher.UseEditorDispatcher(new EditorDispatcher());
            }
        }

        EditorDispatcher()
        {
            EditorCoroutineUtility.StartCoroutine(Update(), this);
        }
        
        public void ToMainThread(Action action)
        {
            _delayedActions.Add(action);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            EditorCoroutineUtility.StartCoroutine(routine, this);
            return null;
        }

        readonly List<Action> _delayedActions = new();
        IEnumerator Update()
        {
            while (true)
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
                yield return null;
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}