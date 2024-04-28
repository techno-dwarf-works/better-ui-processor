using System;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionInfos;
using Better.UISystem.Runtime.TransitionRunners;

namespace Better.UISystem.Runtime.Modules
{
    [Serializable]
    public abstract class SystemModule
    {
        protected UISystem System { get; private set; }

        internal void Initialize(UISystem system)
        {
            System = system;
            OnInitialize();
        }

        internal void Deconstruct()
        {
            OnDeconstruct();
        }

        protected abstract void OnInitialize();

        protected abstract void OnDeconstruct();

        protected internal virtual Task<Result<ISystemElement>> TryHandleOpen(TransitionInfo info)
        {
            return Task.FromResult(Result<ISystemElement>.GetUnsuccessful());
        }

        protected internal virtual Task<bool> TryHandleClose(ISystemElement element, TransitionInfo info)
        {
            return Task.FromResult(false);
        }

        protected internal virtual Task<Result<Sequence>> TryGetTransitionSequence(TransitionInfo info)
        {
            return Task.FromResult(Result<Sequence>.GetUnsuccessful());
        }

        protected internal virtual Task ElementClosed(TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task ElementOpened(ISystemElement element, TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task RunStarted(TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task RunFailed(TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OpenHandled(ISystemElement element, TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task BeforeSequencePlay(ISystemElement element, TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task AfterSequencePlay(ISystemElement element, TransitionInfo info)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task RunCompleted(ISystemElement element, TransitionInfo info)
        {
            return Task.CompletedTask;
        }
        
        protected internal Task<ISystemElement> RunTransitionAsync(TransitionInfo info)
        {
            return ((ITransitionRunner)System).RunAsync(info);
        }
    }

    public abstract class SystemModule<TInfo> : SystemModule where TInfo : TransitionInfo
    {
        protected internal override Task<Result<ISystemElement>> TryHandleOpen(TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return TryHandleOpen(transitionInfo);
            }

            return Task.FromResult(Result<ISystemElement>.GetUnsuccessful());
        }

        protected internal override Task ElementOpened(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnElementOpened(element, transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task<bool> TryHandleClose(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return TryHandleClose(element, transitionInfo);
            }

            return Task.FromResult(false);
        }

        protected internal override Task ElementClosed(TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnElementClosed(transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task<Result<Sequence>> TryGetTransitionSequence(TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return TryGetTransitionSequence(transitionInfo);
            }

            return Task.FromResult(Result<Sequence>.GetUnsuccessful());
        }

        protected internal override Task RunStarted(TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnRunStarted(transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task RunFailed(TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnRunFailed(transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task OpenHandled(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnOpenHandled(element, transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task BeforeSequencePlay(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnBeforeSequencePlay(element, transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task AfterSequencePlay(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnAfterSequencePlay(element, transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected internal override Task RunCompleted(ISystemElement element, TransitionInfo info)
        {
            if (info is TInfo transitionInfo)
            {
                return OnRunCompleted(element, transitionInfo);
            }

            return Task.CompletedTask;
        }

        protected virtual Task<Result<ISystemElement>> TryHandleOpen(TInfo info)
        {
            return Task.FromResult(Result<ISystemElement>.GetUnsuccessful());
        }

        protected virtual Task OnElementOpened(ISystemElement element, TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<bool> TryHandleClose(ISystemElement element, TInfo info)
        {
            return Task.FromResult(false);
        }

        protected virtual Task OnElementClosed(TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<Result<Sequence>> TryGetTransitionSequence(TInfo info)
        {
            return Task.FromResult(Result<Sequence>.GetUnsuccessful());
        }

        protected virtual Task OnRunStarted(TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnRunFailed(TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnOpenHandled(ISystemElement element, TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnBeforeSequencePlay(ISystemElement element, TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnAfterSequencePlay(ISystemElement element, TInfo info)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnRunCompleted(ISystemElement element, TInfo info)
        {
            return Task.CompletedTask;
        }
    }
}