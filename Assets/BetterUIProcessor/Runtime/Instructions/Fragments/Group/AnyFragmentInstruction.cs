using System;
using System.Collections.Generic;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public class AnyFragmentInstruction : GroupFragmentInstruction
    {
        public override bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments)
        {
            foreach (var subInstruction in SubInstructions)
            {
                if (subInstruction.TryVerifyRequest(ownFragments, otherFragments, out requestedFragments))
                {
                    return true;
                }
            }

            requestedFragments = default;
            return false;
        }

        public override bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes)
        {
            foreach (var subInstruction in SubInstructions)
            {
                if (subInstruction.TryVerifyCreate(ownFragments, out fragmentTypes))
                {
                    return true;
                }
            }

            fragmentTypes = default;
            return false;
        }
    }
}