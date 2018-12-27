using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityCommon
{
    public static class AsyncExtensions
    {
        private class YieldInstructionRunner : CoroutineRunner
        {
            public YieldInstructionRunner (YieldInstruction yieldInstruction, MonoBehaviour coroutineContainer = null) 
                : base(coroutineContainer, yieldInstruction) { }

            protected override IEnumerator CoroutineLoop ()
            {
                yield return YieldInstruction;
                HandleOnCompleted();
            }
        }

        public static TaskAwaiter GetAwaiter (this YieldInstruction yieldInstruction, MonoBehaviour coroutineContainer)
        {
            return new YieldInstructionRunner(yieldInstruction, coroutineContainer).RunAsync().GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter (this YieldInstruction yieldInstruction) => GetAwaiter(yieldInstruction, null);

        public static TaskAwaiter<AsyncOperation> GetAwaiter (this AsyncOperation asyncOperation)
        {
            var taskCompletionSource = new TaskCompletionSource<AsyncOperation>();
            if (asyncOperation.isDone) taskCompletionSource.TrySetResult(asyncOperation);
            else asyncOperation.completed += op => taskCompletionSource.TrySetResult(op);
            return taskCompletionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter<UnityWebRequestAsyncOperation> GetAwaiter (this UnityWebRequestAsyncOperation webRequestOperation)
        {
            var taskCompletionSource = new TaskCompletionSource<UnityWebRequestAsyncOperation>();
            if (webRequestOperation.isDone) taskCompletionSource.TrySetResult(webRequestOperation);
            else webRequestOperation.completed += op => taskCompletionSource.TrySetResult(op as UnityWebRequestAsyncOperation);
            return taskCompletionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter<ResourceRequest> GetAwaiter (this ResourceRequest resourceRequest)
        {
            var taskCompletionSource = new TaskCompletionSource<ResourceRequest>();
            if (resourceRequest.isDone) taskCompletionSource.TrySetResult(resourceRequest);
            else resourceRequest.completed += op => taskCompletionSource.TrySetResult(op as ResourceRequest);
            return taskCompletionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter<AssetBundleCreateRequest> GetAwaiter (this AssetBundleCreateRequest createBundleRequest)
        {
            var taskCompletionSource = new TaskCompletionSource<AssetBundleCreateRequest>();
            if (createBundleRequest.isDone) taskCompletionSource.TrySetResult(createBundleRequest);
            else createBundleRequest.completed += op => taskCompletionSource.TrySetResult(op as AssetBundleCreateRequest);
            return taskCompletionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter<AssetBundleRequest> GetAwaiter (this AssetBundleRequest bundleRequest)
        {
            var taskCompletionSource = new TaskCompletionSource<AssetBundleRequest>();
            if (bundleRequest.isDone) taskCompletionSource.TrySetResult(bundleRequest);
            else bundleRequest.completed += op => taskCompletionSource.TrySetResult(op as AssetBundleRequest);
            return taskCompletionSource.Task.GetAwaiter();
        }

        /// <summary>
        /// Allows yielding async methods inside <see cref="Coroutine"/>.
        /// </summary>
        public static IEnumerator AsIEnumerator (this Task task)
        {
            while (!task.IsCompleted) yield return null;
            if (task.IsFaulted) ExceptionDispatchInfo.Capture(task.Exception).Throw();
        }

        /// <summary>
        /// Allows yielding async methods inside <see cref="Coroutine"/>.
        /// </summary>
        public static IEnumerator<T> AsIEnumerator<T> (this Task<T> task)
        {
            while (!task.IsCompleted) yield return default(T);
            if (task.IsFaulted) ExceptionDispatchInfo.Capture(task.Exception).Throw();
            yield return task.Result;
        }

        /// <summary>
        /// Allows to properly execute async methods from sync methods without waiting for <see cref="Task"/>.
        /// Required to receive exceptions from the underlying async method (otherwise it will fail silently).
        /// </summary>
        public static async void WrapAsync (this Task task) => await task;
    }
}
