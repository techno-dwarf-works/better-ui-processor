using System.Threading;

namespace Better.UIProcessor.Runtime.Data
{
    public class CloseTransitionInfo : TransitionInfo
    {
        public CloseTransitionInfo(UIProcessor processor, CancellationToken cancellationToken = default) : base(processor, cancellationToken)
        {
        }
    }
}