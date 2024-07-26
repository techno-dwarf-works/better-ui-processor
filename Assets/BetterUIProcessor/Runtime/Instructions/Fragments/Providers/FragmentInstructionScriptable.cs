using Better.Attributes.Runtime.Select;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Instructions.Providers
{
    [CreateAssetMenu(menuName = MenuName)]
    public class FragmentInstructionScriptable : ScriptableObject
    {
        private const string MenuName = UIProcessorSettings.Path + "/" + nameof(Instructions) + "/" + nameof(FragmentInstructionScriptable);

        [Select]
        [SerializeReference] private FragmentInstruction _sourceInstruction;

        public FragmentInstruction SourceInstruction => _sourceInstruction;
    }
}