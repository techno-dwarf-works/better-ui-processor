using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.TransitionInfos;
using Better.UIComposite.Runtime.TransitionRunners;

namespace Better.UIComposite.Runtime.Modules.Historical
{
    public class HistoryTransitionInfo : TransitionInfo
    {
        private ITransitionRunner Runner { get; }
        public int HistoryDepth { get; private set; }
        public bool AllowExceptions { get; private set; }
        public bool UseSafeDepth { get; private set; }

        public HistoryTransitionInfo(ITransitionRunner runner, int historyDepth, CancellationToken cancellationToken) : base(cancellationToken)
        {
            Runner = runner;
            HistoryDepth = historyDepth;
            AllowExceptions = true;
            UseSafeDepth = false;
        }

        public Task RunAsync()
        {
            ValidateMutable();
            MakeImmutable();

            return Runner.RunAsync(this);
        }

        public HistoryTransitionInfo SuppressExceptions()
        {
            if (ValidateMutable())
            {
                AllowExceptions = false;
            }

            return this;
        }
        
        public HistoryTransitionInfo MakeUseSafeDepth()
        {
            if (ValidateMutable())
            {
                UseSafeDepth = true;
            }

            return this;
        }

        public void Run()
        {
            RunAsync().Forget();
        }
    }
}