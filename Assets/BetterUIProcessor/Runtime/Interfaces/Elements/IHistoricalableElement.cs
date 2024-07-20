using System.Threading;
using System.Threading.Tasks;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IHistoricalableElement : IElement
    {
        public Task OnPushToHistoryAsync(CancellationToken cancellationToken);
        public Task OnPopFromHistoryAsync(CancellationToken cancellationToken);
    }
}