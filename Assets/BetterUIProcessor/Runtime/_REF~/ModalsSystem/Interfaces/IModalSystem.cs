using System.Threading;
using System.Threading.Tasks;
using Better.UIComposite.Runtime.PopupsSystem.Interfaces;
using Better.UIComposite.Runtime.PopupsSystem.Popups;
using Better.UIComposite.Runtime.PopupsSystem.Transitions;

namespace Better.UIComposite.Runtime
{
    public interface IModalSystem : IPopupSystem<IModal, Modal, ModalModel>
    {
    }
}