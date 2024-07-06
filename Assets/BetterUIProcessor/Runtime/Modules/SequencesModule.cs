using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class SequencesModule<TElement> : Module<TElement>
        where TElement : IElement
    {
        // TODO: Add Serialization, handle corner use-cases

        [SerializeField] private List<Sequence> _sequences;

        protected internal override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor<TElement> processor, TElement fromElement, TElement toElement, TransitionInfo<TElement> transitionInfo)
        {
            var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (transitionInfo.OverridenSequence
                && TryGetSequence(transitionInfo.SequenceType, out var sequence))
            {
                return new ProcessResult<Sequence>(sequence);
            }

            return ProcessResult<Sequence>.Unsuccessful;
        }

        private bool TryGetSequence(Type sequenceType, out Sequence sequence)
        {
            foreach (var item in _sequences)
            {
                if (item.GetType() == sequenceType)
                {
                    sequence = item;
                    return true;
                }
            }

            sequence = default;
            return false;
        }
    }
}