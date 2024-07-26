using System;
using UnityEngine;

namespace Better.Tweens.Runtime.Data
{
    [Serializable]
    public abstract class OverridableData<TValue>
    {
        [SerializeField] private bool _overriden;

        private TValue _sourceValue;

        public TValue Value => Overriden ? OverridenValue : _sourceValue;
        protected abstract TValue OverridenValue { get; set; }

        public bool Overriden
        {
            get => _overriden;
            set => _overriden = value;
        }

        public void SetSource(TValue value)
        {
            _sourceValue = value;
        }

        public void Override(TValue value)
        {
            Overriden = true;
            OverridenValue = value;
        }
    }
}