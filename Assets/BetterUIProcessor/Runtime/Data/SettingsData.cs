using System;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Interfaces;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Settings
{
    [Serializable]
    public class SettingsData : ICopyable<SettingsData>
    {
        [Select]
        [SerializeReference] private Sequence _defaultSequence;

        public Sequence DefaultSequence => _defaultSequence;

        public SettingsData()
        {
            _defaultSequence = new GradualSequence();
        }

        public void SetDefaultSequence(Sequence value)
        {
            if (value == null)
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(value));
                return;
            }

            _defaultSequence = value;
        }

        public void Copy(SettingsData source)
        {
            _defaultSequence = source.DefaultSequence.Clone();
        }
    }
}