using System.Threading;
using System.Threading.Tasks;
using Better.UISystem.Runtime.PopupsSystem.Interfaces;
using Better.UISystem.Runtime.PopupsSystem.Popups;
using Better.UISystem.Runtime.PopupsSystem.Transitions;

namespace Better.UISystem.Runtime
{
    public interface IModalSystem : IPopupSystem<IModal, Modal, ModalModel>
    {
    }
}