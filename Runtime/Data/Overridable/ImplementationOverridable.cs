using System;
using Better.Attributes.Runtime.Select;
using UnityEngine;

namespace Better.Tweens.Runtime.Data
{
    [Serializable]
    public class ImplementationOverridable<TValue> : OverridableData<TValue>
    {
        [Select]
        [SerializeReference] private TValue _overridenValue;

        protected override TValue OverridenValue
        {
            get => _overridenValue;
            set => _overridenValue = value;
        }

        public ImplementationOverridable()
        {
        }

        public ImplementationOverridable(TValue overridenValue)
        {
            _overridenValue = overridenValue;
        }
    }
}