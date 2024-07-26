using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IFragment
    {
        public RectTransform RectTransform { get; }
        
        public Task InitializeAsync(CancellationToken cancellationToken);
        public Task OnSnappedAsync(CancellationToken cancellationToken);
    }
}