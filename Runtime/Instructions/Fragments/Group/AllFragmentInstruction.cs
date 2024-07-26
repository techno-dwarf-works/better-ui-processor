using System;
using System.Collections.Generic;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public class AllFragmentInstruction : GroupFragmentInstruction
    {
        public override bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments)
        {
            var bufferFragments = new List<IFragment>();

            foreach (var subInstruction in SubInstructions)
            {
                if (subInstruction.TryVerifyRequest(ownFragments, otherFragments, out requestedFragments))
                {
                    bufferFragments.AddRange(requestedFragments);
                }
                else
                {
                    requestedFragments = default;
                    return false;
                }
            }

            requestedFragments = bufferFragments.ToArray();
            return !requestedFragments.IsNullOrEmpty();
        }

        public override bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes)
        {
            var bufferTypes = new List<Type>();

            foreach (var subInstruction in SubInstructions)
            {
                if (subInstruction.TryVerifyCreate(ownFragments, out fragmentTypes))
                {
                    bufferTypes.AddRange(fragmentTypes);
                }
                else
                {
                    fragmentTypes = default;
                    return false;
                }
            }

            fragmentTypes = bufferTypes.ToArray();
            return !fragmentTypes.IsNullOrEmpty();
        }
    }
}