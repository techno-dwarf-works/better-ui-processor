using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Misc;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class GroupModule : Module, IModulesContainer
    {
        [HideLabel] [Select]
        [SerializeReference] private List<Module> _subModules;

        private HashSet<UIProcessor> _linkedProcessors;

        public GroupModule()
        {
            _subModules = new();
        }

        #region IModulesContainer

        public bool TryAddModule(Module module)
        {
            if (ContainsModule(module))
            {
                return false;
            }

            if (_linkedProcessors != null && !module.Link(_linkedProcessors))
            {
                return false;
            }

            _subModules.Add(module);
            return true;
        }

        public bool ContainsModule(Module module)
        {
            return _subModules.Contains(module);
        }

        public bool RemoveModule(Module module)
        {
            if (!ContainsModule(module))
            {
                return false;
            }

            if (_linkedProcessors != null && !module.Unlink(_linkedProcessors))
            {
                return false;
            }

            _subModules.Remove(module);
            return true;
        }

        #endregion

        protected internal override bool Link(UIProcessor processor)
        {
            var result = base.Link(processor);
            if (!result)
            {
                return false;
            }

            var linked = _subModules.Link(processor);
            if (linked)
            {
                _linkedProcessors ??= new();
                _linkedProcessors.Add(processor);

                ForceRebuild();
            }

            return linked;
        }

        protected internal override bool Unlink(UIProcessor processor)
        {
            var result = base.Unlink(processor);
            if (!result)
            {
                return false;
            }

            var unlinked = _subModules.Unlink(processor);
            if (unlinked)
            {
                _linkedProcessors?.Remove(processor);
            }

            return unlinked;
        }

        public void ForceRebuild()
        {
            _subModules = _subModules.OrderByDescending(m => m.Priority).ToList();
        }

        public void Add(Module module)
        {
            _subModules.Add(module);
            ForceRebuild();
        }

        protected internal override Task OnEnqueuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnEnqueuedTransition(processor, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnTransitionStarted(processor, fromElement, transitionInfo))
                .WhenAll();
        }

        protected internal override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
        {
            foreach (var module in _subModules)
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
            foreach (var module in _subModules)
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
            return _subModules.Select(m => m.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnTransitionCompleted(processor, openedElement, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnTransitionCanceled(processor, transitionInfo))
                .WhenAll();
        }

        protected internal override Task OnElementPreReleased(UIProcessor processor, IElement element)
        {
            return _subModules.Select(m => m.OnElementPreReleased(processor, element))
                .WhenAll();
        }

        protected internal override async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
        {
            foreach (var module in _subModules)
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
            return _subModules.Select(m => m.OnElementReleased(processor))
                .WhenAll();
        }

        protected internal override Task OnDequeuedTransition(UIProcessor processor, TransitionInfo transitionInfo)
        {
            return _subModules.Select(m => m.OnDequeuedTransition(processor, transitionInfo))
                .WhenAll();
        }
    }
}