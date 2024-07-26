using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class ParallelDefaultSequence : DefaultSequence
    {
        protected override Task PostPreparedProcessAsync(ISequencable from, ISequencable to, CancellationToken cancellationToken)
        {
            var tasks = new Task[2];
            tasks[0] = TryHideAsync(from, cancellationToken);
            tasks[1] = TryShowAsync(to, cancellationToken);

            return tasks.WhenAll();
        }

        public override Sequence Clone()
        {
            return new ParallelDefaultSequence();
        }
    }
}