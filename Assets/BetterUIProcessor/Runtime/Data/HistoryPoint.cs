using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Data
{
    public class HistoryPoint
    {
        public static readonly HistoryPoint Empty = new();

        public readonly IHistoricalableElement Element;
        public readonly Sequence RootSequence;
        
        public HistoryPoint(IHistoricalableElement element, Sequence rootSequence)
        {
            Element = element;
            RootSequence = rootSequence;
        }

        private HistoryPoint()
        {
        }
    }
}