using Better.Internal.Core.Runtime;
using Better.Singletons.Runtime.Attributes;
using Better.UIComposite.Runtime.PopupsSystem;

namespace Better.UIComposite.Runtime
{
    [ScriptableCreate(Path)]
    public class ModalSystemSettings : PopupSystemSettings<ModalSystemSettings, Modal, ModalSequence>
    {
        public const string Path = PrefixConstants.BetterPrefix + "/" + "Modals System";

        public override ModalSequence FallbackSequence { get; } = new SimpleModalSequence();
    }
}