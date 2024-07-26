using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Instructions;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IFragmentsHolder
    {
        // TODO: Impl FragmentLocator

        public RectTransform RectTransform { get; }
        public FragmentInstruction Instruction { get; }
        public virtual int Priority => FragmentPriority.Default;

        public Task OnFragmentLinkedAsync(IFragment fragment, CancellationToken cancellationToken);
        public Task OnFragmentUnlinkedAsync(IFragment fragment, CancellationToken cancellationToken);
    }
}