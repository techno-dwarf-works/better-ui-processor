using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Components.UI;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Fragments
{
    public abstract class Fragment : UIMonoBehaviour, IFragment
    {
        public bool Initialized { get; set; }

        async Task IFragment.InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var message = $"{nameof(cancellationToken)} cannot be cancelled";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }

            if (Initialized)
            {
                var message = $"Already {nameof(Initialized)}";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }
            
            Initialized = true;
            await OnInitializedAsync();
        }

        protected virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnSnappedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}