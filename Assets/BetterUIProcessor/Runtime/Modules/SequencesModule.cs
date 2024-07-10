using System;
using System.Threading.Tasks;
using Better.Locators.Runtime;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class SequencesModule : Module
    {
        [SerializeField] private Locator<Sequence> _sequences;

        public SequencesModule()
        {
            _sequences = new();
        }

        public SequencesModule(Sequence[] sequences) : this()
        {
            foreach (var sequence in sequences)
            {
                _sequences.Add(sequence);
            }
        }

        protected internal override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (transitionInfo.OverridenSequence
                && _sequences.TryGet(transitionInfo.SequenceType, out var sequence))
            {
                return new ProcessResult<Sequence>(sequence);
            }

            return ProcessResult<Sequence>.Unsuccessful;
        }
    }
}