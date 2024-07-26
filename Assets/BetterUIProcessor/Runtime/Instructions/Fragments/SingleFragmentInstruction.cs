using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.DataStructures.SerializedTypes;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public class SingleFragmentInstruction : FragmentInstruction
    {
        [Select(typeof(IFragment))]
        [SerializeField] private SerializedType _fragmentType;

        public SingleFragmentInstruction()
        {
        }

        public SingleFragmentInstruction(SerializedType fragmentType)
        {
            _fragmentType = fragmentType;
        }

        public override bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments)
        {
            if (ownFragments.Any(f => f.GetType() == _fragmentType))
            {
                requestedFragments = default;
                return false;
            }

            foreach (var otherFragment in otherFragments)
            {
                if (otherFragment.GetType() == _fragmentType)
                {
                    requestedFragments = new[] { otherFragment };
                    return true;
                }
            }

            requestedFragments = default;
            return false;
        }

        public override bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes)
        {
            if (ownFragments.Any(f => f.GetType() == _fragmentType))
            {
                fragmentTypes = default;
                return false;
            }

            fragmentTypes = new Type[] { _fragmentType };
            return true;
        }
    }
}