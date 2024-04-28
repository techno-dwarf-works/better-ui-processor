using System;
using System.Collections.Generic;
using Better.UISystem.Runtime.PopupsSystem;
using Better.UISystem.Runtime.PopupsSystem.Transitions;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    [Serializable]
    internal class InternalModalSystem : InternalPopupSystem<IModal, Modal, ModalModel>, IModalSystem
    {
        private readonly ModalSystemSettings _settings;

        public InternalModalSystem(RectTransform container) : base(container)
        {
            _settings = ModalSystemSettings.Instance;
        }

        protected override void OnScheduleAdded<TTransitionInfo>(TTransitionInfo info)
        {
        }

        protected override void OnScheduleRemoved<TTransitionInfo>(TTransitionInfo info)
        {
        }

        protected override void OnOpened(IModal opened)
        {
        }

        protected override Modal[] GetPrefabs()
        {
            return _settings.Prefabs;
        }

        protected override PopupsSequence GetTransitionSequence(PopupTransitionInfo info)
        {
            if (!info.OverridenSequence || !_settings.TryGetOverridenSequence(info.SequenceType, out var sequence))
            {
                sequence = _settings.GetDefaultSequence();
            }

            return sequence;
        }
    }
}