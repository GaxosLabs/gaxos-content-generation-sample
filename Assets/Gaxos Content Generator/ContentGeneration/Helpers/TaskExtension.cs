using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ContentGeneration.Helpers
{
    public static class TaskExtension
    {
        public static void CatchAndLog(this Task t, Object context = null)
        {
            t.ContinueWith(taskResult =>
            {
                if (taskResult.IsFaulted)
                {
                    Debug.LogException(taskResult.Exception!.InnerException, context);
                }
            });
        }
        
        public static void Finally(this Task t,
            Action continuationAction, Object context = null)
        {
            t.ContinueWith(taskResult =>
            {
                if (taskResult.IsFaulted)
                {
                    Debug.LogException(taskResult.Exception!.InnerException, context);
                }
                Dispatcher.instance.ToMainThread(continuationAction);
            });
        }

        public static void ContinueInMainThreadWith(this Task t,
            Action<Task> continuationAction)
        {
            t.ContinueWith(tResult =>
            {
                Dispatcher.instance.ToMainThread(() =>
                {
                    continuationAction(tResult);
                });
            });
        }
        public static void ContinueInMainThreadWith<T>(this Task<T> t,
            Action<Task<T>> continuationAction)
        {
            t.ContinueWith(tResult =>
            {
                Dispatcher.instance.ToMainThread(() =>
                {
                    continuationAction(tResult);
                });
            });
        }
    }
}