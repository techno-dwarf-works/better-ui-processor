using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Modules
{
    public abstract class Module<TElement> where TElement : IElement
    {
        public int LinksCount { get; private set; }
        public bool IsLinked => LinksCount > 0;

        protected internal virtual bool Link(UIProcessor<TElement> processor)
        {
            LinksCount++;
            return true;
        }

        protected internal virtual bool Unlink(UIProcessor<TElement> processor)
        {
            LinksCount--;
            return true;
        }

        protected internal virtual Task OnEnqueuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionStarted(UIProcessor<TElement> processor, TElement fromElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task<ProcessResult<TElement>> TryGetTransitionElement(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<TElement>.Unsuccessful);
        }

        protected internal virtual Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<Sequence>.Unsuccessful);
        }

        protected internal virtual Task OnPreSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnPostSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionCompleted(UIProcessor<TElement> processor, TElement openedElement, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnTransitionCanceled(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task<bool> TryReleaseElement(UIProcessor<TElement> processor, TElement element)
        {
            return Task.FromResult(false);
        }

        protected internal virtual Task OnElementReleased(UIProcessor<TElement> processor)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnDequeuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class Module<TElement, TTransitionInfo> : Module<TElement>
        where TElement : IElement
        where TTransitionInfo : TransitionInfo
    {
        protected internal sealed override async Task OnEnqueuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            await base.OnEnqueuedTransition(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnEnqueuedTransition(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnEnqueuedTransition(UIProcessor<TElement> processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionStarted(UIProcessor<TElement> processor, TElement fromElement, TransitionInfo transitionInfo)
        {
            await base.OnTransitionStarted(processor, fromElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionStarted(processor, fromElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionStarted(UIProcessor<TElement> processor, TElement fromElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task<ProcessResult<TElement>> TryGetTransitionElement(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (!result.IsSuccessful && transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                result = await TryGetTransitionElement(processor, castedTransitionInfo);
            }

            return result;
        }

        protected virtual Task<ProcessResult<TElement>> TryGetTransitionElement(UIProcessor<TElement> processor, TTransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<TElement>.Unsuccessful);
        }

        protected internal sealed override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
            if (!result.IsSuccessful && transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                result = await TryGetTransitionSequence(processor, fromElement, toElement, castedTransitionInfo);
            }

            return result;
        }

        protected virtual Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.FromResult(ProcessResult<Sequence>.Unsuccessful);
        }

        protected internal sealed override async Task OnPreSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPreSequencePlay(processor, fromElement, toElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnPreSequencePlay(processor, fromElement, toElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnPreSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnPostSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPostSequencePlay(processor, fromElement, toElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnPostSequencePlay(processor, fromElement, toElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnPostSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionCompleted(UIProcessor<TElement> processor, TElement openedElement, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCompleted(processor, openedElement, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionCompleted(processor, openedElement, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionCompleted(UIProcessor<TElement> processor, TElement openedElement, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnTransitionCanceled(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCanceled(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnTransitionCanceled(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnTransitionCanceled(UIProcessor<TElement> processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }

        protected internal sealed override async Task OnDequeuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            await base.OnDequeuedTransition(processor, transitionInfo);

            if (transitionInfo is TTransitionInfo castedTransitionInfo)
            {
                await OnDequeuedTransition(processor, castedTransitionInfo);
            }
        }

        protected virtual Task OnDequeuedTransition(UIProcessor<TElement> processor, TTransitionInfo transitionInfo)
        {
            return Task.CompletedTask;
        }
    }
}