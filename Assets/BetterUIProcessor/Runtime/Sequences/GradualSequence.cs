using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class GradualSequence : SimpleSequence
    {
        protected override async Task ProcessAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            await TryHideAsync(from);
            await TryShowAsync(to);
        }

        public override Sequence Clone()
        {
            return new GradualSequence();
        }
    }
}