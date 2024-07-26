using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface ISequencable
    {
        public RectTransform RectTransform { get; }
        public bool Displayed { get; set; }

        public Task PrepareShowAsync(CancellationToken cancellationToken);
        public Task ShowAsync(CancellationToken cancellationToken);
        public Task PrepareHideAsync(CancellationToken cancellationToken);
        public Task HideAsync(CancellationToken cancellationToken);
    }
}