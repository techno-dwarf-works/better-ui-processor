using System;
using Better.UISystem.Runtime.Common;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public partial class UISystem
    {
        public bool TryGetSequence(Type sequenceType, out Sequence sequence)
        {
            if (sequenceType != null)
            {
                for (var i = 0; i < OverridenSequences.Count; i++)
                {
                    sequence = OverridenSequences[i];
                    if (sequence.GetType() == sequenceType)
                    {
                        return true;
                    }
                }
            }

            sequence = default;
            return false;
        }

        public Sequence GetDefaultSequence()
        {
            if (_defaultSequence == null)
            {
                var message = $"{nameof(_defaultSequence)} is null, returned {nameof(_fallbackSequence)}({_fallbackSequence})";
                Debug.LogWarning(message);

                return _fallbackSequence;
            }

            return _defaultSequence;
        }
    }
}