using System;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;

namespace Better.UISystem.Runtime.Common
{
    [Serializable]
    public abstract class Sequence
    {
        public abstract Task DoPlay(ISystemElement to);

        public abstract Task DoPlay(ISystemElement from, ISystemElement to);

        public abstract Sequence GetInverseSequence();
    }
}