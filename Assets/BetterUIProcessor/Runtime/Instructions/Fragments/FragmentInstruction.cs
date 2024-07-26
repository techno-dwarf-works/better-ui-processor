using System;
using System.Collections.Generic;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public abstract class FragmentInstruction
    {
        public abstract bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments);
        public abstract bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes);
    }
}