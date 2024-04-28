using System;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    [Serializable]
    public abstract class DurationModalSequence : ModalSequence
    {
        [Min(0)] [SerializeField] protected float _duration;
    }
}