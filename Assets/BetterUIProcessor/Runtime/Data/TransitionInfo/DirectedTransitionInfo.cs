using System;
using System.Threading;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Data
{
    public class DirectedTransitionInfo<TElement> : TransitionInfo<TElement>
        where TElement : IElement
    {
        public Type ElementType { get; private set; }

        public DirectedTransitionInfo(CancellationToken cancellationToken = default) : base(cancellationToken)
        {
        }

        public TransitionInfo<TElement> SetTargetElement<TValue>()
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