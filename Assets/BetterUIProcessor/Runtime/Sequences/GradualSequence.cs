using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class GradualSequence : SimpleSequence
    {
        public override async Task PlayAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            await base.PlayAsync(container, from, to);
            await TryHideAsync(from);
            await TryShowAsync(to);
        }

        public override Sequence Clone()
        {
            return new GradualSequence();
        }
    }
}