using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class FragmentsModule : Module
    {
        private class HolderMeta
        {
            public IFragmentsHolder Holder { get; }
            public RectTransform BufferContainer { get; }
            public HashSet<IFragment> FragmentSet { get; }

            public HolderMeta(IFragmentsHolder holder, RectTransform bufferContainer)
            {
                Holder = holder;
                BufferContainer = bufferContainer;
                FragmentSet = new();
            }
        }

        [Select]
        [SerializeReference] private IFragmentDatabase _database;

        private Locator<IElement, HolderMeta> _locator;

        public FragmentsModule()
        {
        }

        public FragmentsModule(IFragmentDatabase database) : this()
        {
            _database = database;
        }

        protected internal override bool Link(UIProcessor processor)
        {
            var linked = base.Link(processor);
            if (linked)
            {
                _locator ??= new();
                TryCreateHolderMeta(processor.OpenedElement, processor.Container, out _);
            }

            return linked;
        }

        protected internal override bool Unlink(UIProcessor processor)
        {
            var unlinked = base.Unlink(processor);
            if (unlinked && processor.HasOpenedElement())
            {
                _locator.Remove(processor.OpenedElement);
            }

            return unlinked;
        }

        protected internal override async Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);

            if (transitionInfo.IsCanceled)
            {
                return;
            }

            var toLocated = TryCreateHolderMeta(toElement, processor.Container, out var toMeta);

            if (fromElement != null && _locator.TryGet(fromElement, out var fromMeta))
            {
                await ReleaseFragmentsAsync(fromMeta);
                await SnapFragmentsAsync(fromMeta.FragmentSet, fromMeta.Holder.RectTransform);
                _locator.Remove(fromElement);
            }

            if (toLocated)
            {
                await RequestFragmentsAsync(toMeta);
                await CreateFragmentsAsync(toMeta);
            }
        }

        protected internal override async Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            await base.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);

            if (toElement != null&& _locator.TryGet(toElement, out var toMeta))
            {
                await SnapFragmentsAsync(toMeta.FragmentSet, toMeta.Holder.RectTransform);
            }
        }

        private bool TryCreateHolderMeta(IElement element, RectTransform bufferContainer, out HolderMeta holderMeta)
        {
            if (element == null)
            {
                holderMeta = default;
                return false;
            }

            if (element is IFragmentsHolder fragmentsHolder || element.RectTransform.TryGetComponent(out fragmentsHolder))
            {
                holderMeta = new HolderMeta(fragmentsHolder, bufferContainer);
                return _locator.TryAdd(element, holderMeta);
            }

            holderMeta = default;
            return false;
        }

        private async Task ReleaseFragmentsAsync(HolderMeta sourceHolderMeta)
        {
            var holderMetas = _locator.GetElements().ToList();
            holderMetas.Remove(sourceHolderMeta);

            foreach (var holderMeta in holderMetas)
            {
                var instruction = holderMeta.Holder.Instruction;
                if (instruction == null)
                {
                    continue;
                }

                if (!instruction.TryVerifyRequest(holderMeta.FragmentSet, sourceHolderMeta.FragmentSet, out var requestedFragments))
                {
                    continue;
                }

                foreach (var requestedFragment in requestedFragments)
                {
                    sourceHolderMeta.FragmentSet.Remove(requestedFragment);
                    await sourceHolderMeta.Holder.OnFragmentUnlinkedAsync(requestedFragment, CancellationToken.None);

                    holderMeta.FragmentSet.Add(requestedFragment);
                    await holderMeta.Holder.OnFragmentLinkedAsync(requestedFragment, CancellationToken.None);
                    await SnapFragmentAsync(requestedFragment, holderMeta.BufferContainer);
                }
            }
        }

        private async Task RequestFragmentsAsync(HolderMeta targetHolderMeta)
        {
            var instruction = targetHolderMeta.Holder.Instruction;
            if (instruction == null)
            {
                return;
            }

            var holderMetas = _locator.GetElements()
                .Where(m => m.Holder.Priority < targetHolderMeta.Holder.Priority);

            foreach (var holderMeta in holderMetas)
            {
                if (!instruction.TryVerifyRequest(targetHolderMeta.FragmentSet, holderMeta.FragmentSet, out var requestedFragments))
                {
                    return;
                }

                foreach (var requestedFragment in requestedFragments)
                {
                    holderMeta.FragmentSet.Remove(requestedFragment);
                    await holderMeta.Holder.OnFragmentUnlinkedAsync(requestedFragment, CancellationToken.None);

                    targetHolderMeta.FragmentSet.Add(requestedFragment);
                    await targetHolderMeta.Holder.OnFragmentLinkedAsync(requestedFragment, CancellationToken.None);
                    await SnapFragmentAsync(requestedFragment, targetHolderMeta.BufferContainer);
                }
            }
        }

        private async Task CreateFragmentsAsync(HolderMeta holderMeta)
        {
            var container = holderMeta.Holder.RectTransform;
            var instruction = holderMeta.Holder.Instruction;

            if (instruction == null)
            {
                return;
            }

            if (!instruction.TryVerifyCreate(holderMeta.FragmentSet, out var fragmentTypes))
            {
                return;
            }

            foreach (var fragmentType in fragmentTypes)
            {
                var createResult = await _database.TryCreateFragmentAsync(container, fragmentType, CancellationToken.None);
                if (createResult.IsSuccessful)
                {
                    holderMeta.FragmentSet.Add(createResult.Result);
                    await holderMeta.Holder.OnFragmentLinkedAsync(createResult.Result, CancellationToken.None);
                    await SnapFragmentAsync(createResult.Result, container);
                }
                else
                {
                    var message = $"Not found {nameof(IFragment)} with {nameof(fragmentType)}:{fragmentType.Name} in {nameof(IFragmentDatabase)}";
                    DebugUtility.LogException<InvalidOperationException>(message);
                }
            }
        }

        private Task SnapFragmentAsync(IFragment fragment, RectTransform container)
        {
            fragment.RectTransform.SetParent(container, false);
            fragment.RectTransform.SetAsLastSibling();

            return fragment.OnSnappedAsync(CancellationToken.None);
        }

        private Task SnapFragmentsAsync(IEnumerable<IFragment> fragments, RectTransform container)
        {
            return fragments.Select(fragment => SnapFragmentAsync(fragment, container))
                .WhenAll();
        }
    }
}