using System;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    [Serializable]
    public abstract class DurationModalSequence : ModalSequence
    {
        [Min(0)] [SerializeField] protected float _duration;
    }
}