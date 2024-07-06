using System;
using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.TransitionRunners
{
    [Serializable]
    public abstract class TransitionRunner
    {
        public abstract Task<Result<IElement>> RunAsync(IUISystem system, IElement element, TransitionInfo info, CancellationToken cancellationToken);
    }
}