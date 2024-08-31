using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface ISequencable
    {
        public RectTransform RectTransform { get; }

        public Task PreShowAsync(CancellationToken cancellationToken);
        public Task ShowAsync(CancellationToken cancellationToken);
        public Task PostShowAsync(CancellationToken cancellationToken);
        
        public Task PreHideAsync(CancellationToken cancellationToken);
        public Task HideAsync(CancellationToken cancellationToken);
        public Task PostHideAsync(CancellationToken cancellationToken);
    }
}