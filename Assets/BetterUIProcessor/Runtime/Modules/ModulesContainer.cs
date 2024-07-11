using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better.Commons.Runtime.DataStructures.SerializedTypes;
using Better.Commons.Runtime.Extensions;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class ModulesContainer : Locator<Module>
    {
        private List<Module> _sortedElements;

        public void Initialize()
        {
            // TODO: Check (serialized and code-injected) elements order after Locator`1 updated
            _sortedElements = GetElements().ToList();
        }

        protected override void OnAdded(SerializedType key, Module module)
        {
            base.OnAdded(key, module);

            _sortedElements.Add(module);
        }

        protected override void OnRemoved(SerializedType key, Module module)
        {
            base.OnRemoved(key, module);

            _sortedElements.Remove(module);
        }

        public Task OnEnqueuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnEnqueuedTransition(processor, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionStarted(processor, fromElement, transitionInfo))
                .WhenAll();
        }

        public async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
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

            return ProcessResult<IElement>.Unsuccessful;
        }

        public async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
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

        public Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        public Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionCompleted(processor, openedElement, transitionInfo))
                .WhenAll();
        }

        public Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnTransitionCanceled(processor, transitionInfo))
                .WhenAll();
        }

        public async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
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

        public Task OnElementReleased(UIProcessor processor)
        {
            return GetElements()
                .Select(m => m.OnElementReleased(processor))
                .WhenAll();
        }

        public Task OnDequeuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return GetElements()
                .Select(m => m.OnDequeuedTransition(processor, transitionInfo))
                .WhenAll();
        }
    }
}