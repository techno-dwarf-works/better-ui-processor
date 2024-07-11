using System.Threading.Tasks;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IHistoricalableElement : IElement
    {
        public void OnPushToHistoryAsync();
        public void OnPopFromHistoryAsync();
    }
}