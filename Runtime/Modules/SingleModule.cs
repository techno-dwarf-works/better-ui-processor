using System;
using Better.UIProcessor.Runtime.Data;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public abstract class SingleModule : Module
    {
        protected UIProcessor Processor { get; private set; }

        protected internal override bool Link(UIProcessor processor)
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

        protected internal override bool Unlink(UIProcessor processor)
        {
            var result = base.Unlink(processor);
            if (result && Processor == processor)
            {
                Processor = null;
            }

            return result;
        }
    }

    public abstract class SingleModule<TTransitionInfo> : Module<TTransitionInfo>
        where TTransitionInfo : TransitionInfo
    {
        protected UIProcessor Processor { get; private set; }

        protected internal override bool Link(UIProcessor processor)
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

        protected internal override bool Unlink(UIProcessor processor)
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