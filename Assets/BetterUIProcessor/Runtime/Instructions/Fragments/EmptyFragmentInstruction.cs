using System;
using System.Collections.Generic;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public class EmptyFragmentInstruction : FragmentInstruction
    {
        public override bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments)
        {
            requestedFragments = default;
            return false;
        }

        public override bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes)
        {
            fragmentTypes = default;
            return false;
        }
    }
}