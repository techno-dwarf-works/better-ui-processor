using Better.Internal.Core.Runtime;
using Better.Singletons.Runtime.Attributes;
using Better.UISystem.Runtime.PopupsSystem;

namespace Better.UISystem.Runtime
{
    [ScriptableCreate(Path)]
    public class ModalSystemSettings : PopupSystemSettings<ModalSystemSettings, Modal, ModalSequence>
    {
        public const string Path = PrefixConstants.BetterPrefix + "/" + "Modals System";

        public override ModalSequence FallbackSequence { get; } = new SimpleModalSequence();
    }
}