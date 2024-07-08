using System;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public abstract class SingleModule<TElement> : Module<TElement>
        where TElement : IElement
    {
        protected UIProcessor<TElement> Processor { get; private set; }

        protected internal override bool Link(UIProcessor<TElement> processor)
        {
            if (IsLinked)
            {
                var message = $"Already linked to {processor}";
                Debug.LogWarning(message);

                return false;
            }

            var result = base.Link(processor);
            if (result)
            {
                Processor = processor;
            }

            return result;
        }

        protected internal override bool Unlink(UIProcessor<TElement> processor)
        {
            var result = base.Unlink(processor);
            if (result && Processor == processor)
            {
                Processor = null;
            }

            return result;
        }
    }

    public abstract class SingleModule<TElement, TTransitionInfo> : Module<TElement, TTransitionInfo>
        where TElement : IElement
        where TTransitionInfo : TransitionInfo<TElement>
    {
        protected UIProcessor<TElement> Processor { get; private set; }

        protected internal override bool Link(UIProcessor<TElement> processor)
        {
            if (IsLinked)
            {
                var message = $"Already linked to {processor}";
                Debug.LogWarning(message);

                return false;
            }

            var result = base.Link(processor);
            if (result)
            {
                Processor = processor;
            }

            return result;
        }

        protected internal override bool Unlink(UIProcessor<TElement> processor)
        {
            var result = base.Unlink(processor);
            if (result && Processor == processor)
            {
                Processor = null;
            }

            return result;
        }
    }
}