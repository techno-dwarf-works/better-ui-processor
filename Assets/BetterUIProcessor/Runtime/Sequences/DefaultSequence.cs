using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class DefaultSequence : Sequence
    {
        public override async Task PlayAsync(ISequencable from, ISequencable to)
        {
            var preparedTasks = new List<Task>(2);

            if (from != null)
            {
                var prepareHideTask = from.PrepareHideAsync(CancellationToken.None);
                preparedTasks.Add(prepareHideTask);
            }

            if (to != null)
            {
                var prepareShowTask = to.PrepareShowAsync(CancellationToken.None);
                preparedTasks.Add(prepareShowTask);
            }

            await preparedTasks.WhenAll();

            if (from != null)
            {
                await from.HideAsync(CancellationToken.None);
            }

            if (to != null)
            {
                await to.ShowAsync(CancellationToken.None);
            }
        }

        public override Sequence GetInverseSequence()
        {
            return Clone();
        }

        public override Sequence Clone()
        {
            return new DefaultSequence();
        }
    }
}