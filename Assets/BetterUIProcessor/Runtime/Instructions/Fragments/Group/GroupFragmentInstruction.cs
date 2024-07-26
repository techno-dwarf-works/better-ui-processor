using System;
using System.Collections.Generic;
using System.Linq;
using Better.Attributes.Runtime.Select;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Instructions
{
    [Serializable]
    public abstract class GroupFragmentInstruction : FragmentInstruction
    {
        [Select]
        [SerializeReference] private FragmentInstruction[] _subInstructions;

        protected FragmentInstruction[] SubInstructions => _subInstructions;

        public GroupFragmentInstruction(IEnumerable<FragmentInstruction> subInstructions)
        {
            _subInstructions = subInstructions.ToArray();
        }

        public GroupFragmentInstruction() : this(Array.Empty<FragmentInstruction>())
        {
        }
    }
}