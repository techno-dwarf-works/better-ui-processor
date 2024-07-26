using System;
using System.Threading;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Data
{
    public class HistoricalTransitionInfo : TransitionInfo
    {
        public int Depth { get; private set; }
        public bool UseSafeDepth { get; private set; }

        public HistoricalTransitionInfo(UIProcessor processor, int depth, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
            Depth = Mathf.Max(depth, 0);
            UseSafeDepth = false;
        }

        public HistoricalTransitionInfo SetSafeDepth(bool value = true)
        {
            if (ValidateMutable(true))
            {
                UseSafeDepth = value;
            }

            return this;
        }
    }
}