using System;
using System.Collections.Generic;
using Better.UIProcessor.Runtime.Instructions.Providers;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public class ScriptableFragmentInstruction : FragmentInstruction
    {
        [SerializeField] private FragmentInstructionScriptable _scriptableInstruction;

        public  ScriptableFragmentInstruction()
        {
        }

        public override bool TryVerifyRequest(IEnumerable<IFragment> ownFragments, IEnumerable<IFragment> otherFragments, out IFragment[] requestedFragments)
        {
            return _scriptableInstruction.SourceInstruction.TryVerifyRequest(ownFragments, otherFragments, out requestedFragments);
        }

        public override bool TryVerifyCreate(IEnumerable<IFragment> ownFragments, out Type[] fragmentTypes)
        {
            return _scriptableInstruction.SourceInstruction.TryVerifyCreate(ownFragments, out fragmentTypes);
        }
    }
}