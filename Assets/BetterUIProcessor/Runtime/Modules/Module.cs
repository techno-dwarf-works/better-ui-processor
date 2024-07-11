using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public abstract class Module
    {
        public int LinksCount { get; private set; }
        public bool IsLinked => LinksCount > 0;

        protected internal virtual bool Link(UIProcessor processor)
        {
            LinksCount++;
            return true;
        }

        protected internal virtual bool Unlink(UIProcessor processor)
        {
            LinksCount--;
            return true;
        }

        protected internal virtual Task OnEnqueuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<IElement>.Unsuccessful);
        }

        protected internal virtual Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<Sequence>.Unsuccessful);
        }

        protected internal virtual Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
        {
            return Task.FromResult(false);
        }

        protected internal virtual Task OnElementReleased(UIProcessor processor)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnDequeuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class Module<TTransitionInfo> : Module
        where TTransitionInfo : TransitionInfo
    {
        protected internal sealed override async Task OnEnqueuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            await base.OnEnqueuedTransition(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnEnqueuedTransition(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnEnqueuedTransition(UIProcessor processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            await base.OnTransitionStarted(processor, fromElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionStarted(processor, fromElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (!result.IsSuccessful && transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                result = await TryGetTransitionElement(processor, castedTransitionInfo);
            }

            return result;
        }

        protected virtual Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TTransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<IElement>.Unsuccessful);
        }

        protected internal sealed override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
            if (!result.IsSuccessful && transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                result = await TryGetTransitionSequence(processor, fromElement, toElement, castedTransitionInfo);
            }

            return result;
        }

        protected virtual Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<Sequence>.Unsuccessful);
        }

        protected internal sealed override async Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnPreSequencePlay(processor, sequence, fromElement, toElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnPostSequencePlay(processor, sequence, fromElement, toElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCompleted(processor, openedElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionCompleted(processor, openedElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCanceled(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionCanceled(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionCanceled(UIProcessor processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnDequeuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            await base.OnDequeuedTransition(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnDequeuedTransition(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnDequeuedTransition(UIProcessor processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }
    }
}