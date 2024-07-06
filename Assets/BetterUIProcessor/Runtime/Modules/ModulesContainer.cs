using System.Linq;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Modules
{
    public class ModulesContainer<TElement> : Locator<Module<TElement>>
        where TElement : IElement
    {
        public Task OnEnqueuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnEnqueuedTransition(processor, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionStarted(UIProcessor<TElement> processor, TElement fromElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionStarted(processor, fromElement, transitionInfo))
                .WhenAll();
        }

        public async Task<ProcessResult<TElement>> TryGetTransitionElement(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            var modules = GetElements();
            foreach (var module in modules)
            {
                var result = await module.TryGetTransitionElement(processor, transitionInfo);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return ProcessResult<TElement>.Unsuccessful;
        }

        public async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            var modules = GetElements();
            foreach (var module in modules)
            {
                var result = await module.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return ProcessResult<Sequence>.Unsuccessful;
        }

        public Task OnPreSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnPreSequencePlay(processor, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        public Task OnPostSequencePlay(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnPostSequencePlay(processor, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionCompleted(UIProcessor<TElement> processor, TElement openedElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionCompleted(processor, openedElement, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionCanceled(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionCanceled(processor, transitionInfo))
                .WhenAll();
        }

        public async Task<bool> TryReleaseElement(UIProcessor<TElement> processor, TElement element)
        {
            var modules = GetElements();
            foreach (var module in modules)
            {
                var released = await module.TryReleaseElement(processor, element);
                if (released)
                {
                    return true;
                }
            }

            return false;
        }

        public Task OnElementReleased(UIProcessor<TElement> processor)
        {
            return GetElements()
                .Select(m => m.OnElementReleased(processor))
                .WhenAll();
        }

        public Task OnDequeuedTransition(UIProcessor<TElement> processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnDequeuedTransition(processor, transitionInfo))
                .WhenAll();
        }
    }
}