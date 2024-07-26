using System;
using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class GradualDefaultSequence : DefaultSequence
    {
        protected override async Task PostPreparedProcessAsync(ISequencable from, ISequencable to, CancellationToken cancellationToken)
        {
            await TryHideAsync(from, cancellationToken);
            await TryShowAsync(to, cancellationToken);
        }

        public override Sequence Clone()
        {
            return new GradualDefaultSequence();
        }
    }
}