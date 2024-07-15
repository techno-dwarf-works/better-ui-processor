using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Misc;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class GroupModule : Module
    {
        [HideLabel] [Select]
        [SerializeReference] private List<Module> _modules;

        public GroupModule()
        {
            _modules = new();
        }

        public void ForceReOrder()
        {
            _modules = _modules.OrderBy(m => m.Priority).ToList();
        }

        public void Add(Module module)
        {
            _modules.Add(module);
            ForceReOrder();
        }

        public bool Contains(Module module)
        {
            return _modules.Contains(module);
        }

        public bool Remove(Module module)
        {
            return _modules.Remove(module);
        }

        protected internal override Task OnEnqueuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnEnqueuedTransition(processor, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnTransitionStarted(processor, fromElement, transitionInfo))
                .WhenAll();
        }

        protected internal override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
        {
            foreach (var module in _modules)
            {
                var result = await module.TryGetTransitionElement(processor, transitionInfo);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return ProcessResult<IElement>.Unsuccessful;
        }

        protected internal override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            foreach (var module in _modules)
            {
                var result = await module.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return ProcessResult<Sequence>.Unsuccessful;
        }

        protected internal override Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnTransitionCompleted(processor, openedElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnTransitionCanceled(processor, transitionInfo))
                .WhenAll();
        }

        protected internal override async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
        {
            foreach (var module in _modules)
            {
                var released = await module.TryReleaseElement(processor, element);
                if (released)
                {
                    return true;
                }
            }

            return false;
        }

        protected internal override Task OnElementReleased(UIProcessor processor)
        {
            return _modules.Select(m => m.OnElementReleased(processor))
                .WhenAll();
        }

        protected internal override Task OnDequeuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _modules.Select(m => m.OnDequeuedTransition(processor, transitionInfo))
                .WhenAll();
        }
    }
}