using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    public abstract class ResourceRunner
    {
        public enum RunnerState { Created, Running, Completed, Canceled }

        public virtual RunnerState State { get; protected set; }
        public Type ExpectedResourceType { get; protected set; }

        private TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();

        public virtual Task Run ()
        {
            if (State != RunnerState.Created) Debug.LogWarning($"Resource runner started to run with state '{State}'.");
            State = RunnerState.Running;
            return Task.CompletedTask;
        }

        public virtual void Cancel ()
        {
            State = RunnerState.Canceled;
            completionSource.TrySetCanceled();
        }

        public TaskAwaiter GetAwaiter () => (completionSource.Task as Task).GetAwaiter();

        protected virtual void HandleOnCompleted ()
        {
            State = RunnerState.Completed;
            completionSource.TrySetResult(null);
        }
    }

    public class ResourceRunner<TResource> : ResourceRunner
    {
        public ResourceRunner ()
        {
            ExpectedResourceType = typeof(TResource);
        }
    }

    public class LoadResourceRunner<TResource> : ResourceRunner<TResource> where TResource : UnityEngine.Object
    {
        public Resource<TResource> Resource { get; protected set; }

        private TaskCompletionSource<Resource<TResource>> completionSource = new TaskCompletionSource<Resource<TResource>>();

        public new TaskAwaiter<Resource<TResource>> GetAwaiter () => completionSource.Task.GetAwaiter();

        public override void Cancel ()
        {
            base.Cancel();
            completionSource.TrySetCanceled();
        }

        protected override void HandleOnCompleted ()
        {
            base.HandleOnCompleted();
            completionSource.TrySetResult(Resource);
        }
    }

    public class LocateResourcesRunner<TResource> : ResourceRunner<TResource> where TResource : UnityEngine.Object
    {
        public List<Resource<TResource>> LocatedResources { get; protected set; } = new List<Resource<TResource>>();

        private TaskCompletionSource<List<Resource<TResource>>> completionSource = new TaskCompletionSource<List<Resource<TResource>>>();

        public new TaskAwaiter<List<Resource<TResource>>> GetAwaiter () => completionSource.Task.GetAwaiter();

        public override void Cancel ()
        {
            base.Cancel();
            completionSource.TrySetCanceled();
        }

        protected override void HandleOnCompleted ()
        {
            base.HandleOnCompleted();
            completionSource.TrySetResult(LocatedResources);
        }
    }

    public class LocateFoldersRunner : ResourceRunner<Folder>
    {
        public List<Folder> LocatedFolders { get; protected set; } = new List<Folder>();

        private TaskCompletionSource<List<Folder>> completionSource = new TaskCompletionSource<List<Folder>>();

        public new TaskAwaiter<List<Folder>> GetAwaiter () => completionSource.Task.GetAwaiter();

        public override void Cancel ()
        {
            base.Cancel();
            completionSource.TrySetCanceled();
        }

        protected override void HandleOnCompleted ()
        {
            base.HandleOnCompleted();
            completionSource.TrySetResult(LocatedFolders);
        }
    }
}
