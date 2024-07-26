using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Interfaces;

namespace TEST
{
    class SettingsScreen : Screen<SettingsView, SettingsModel>, IHistoricalableElement
    {
        public Task OnPushToHistoryAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnPopFromHistoryAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}