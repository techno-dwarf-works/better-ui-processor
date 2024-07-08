using System;
using System.Threading;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Data
{
    public class DirectedTransitionInfo<TElement> : TransitionInfo<TElement>
        where TElement : IElement
    {
        public Type ElementType { get; private set; }

        public DirectedTransitionInfo(UIProcessor<TElement> processor, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
        }

        public DirectedTransitionInfo<TElement> SetTargetElement<TValue>()
            where TValue : TElement
        {
            if (ValidateMutable(true))
            {
                ElementType = typeof(TValue);
            }

            return this;
        }
    }
}