using System;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class ParallelSequence : SimpleSequence
    {
        public override async Task PlayAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            await base.PlayAsync(container, from, to);

            var tasks = new Task[2];
            tasks[0] = TryHideAsync(from);
            tasks[1] = TryShowAsync(to);
            await tasks.WhenAll();
        }

        public override Sequence Clone()
        {
            return new ParallelSequence();
        }
    }
}