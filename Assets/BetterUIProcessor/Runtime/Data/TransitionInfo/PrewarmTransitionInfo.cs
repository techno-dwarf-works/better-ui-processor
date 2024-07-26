using System.Threading;

namespace Better.UIProcessor.Runtime.Data
{
    internal class PrewarmTransitionInfo : TransitionInfo
    {
        internal PrewarmTransitionInfo(UIProcessor processor, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
        }
    }
}