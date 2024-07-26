using System.Threading;
using System.Threading.Tasks;

namespace Better.UIProcessor.Runtime.Fragments
{
    public abstract class UnderlayFragment : Fragment
    {
        public override Task OnSnappedAsync(CancellationToken cancellationToken)
        {
            RectTransform.SetAsFirstSibling();
            return base.OnSnappedAsync(cancellationToken);
        }
    }
}