using System;
using System.Threading.Tasks;
using Better.Commons.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public abstract class Sequence : ICloneable<Sequence>
    {
        public abstract Task PlayAsync(ISequencable from, ISequencable to);
        public abstract Sequence GetInverseSequence();
        public abstract Sequence Clone();
    }
}