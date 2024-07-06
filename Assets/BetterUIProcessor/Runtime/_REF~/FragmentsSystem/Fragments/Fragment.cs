using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.UIComposite.Runtime.Common;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    public abstract class Fragment : UIMonoBehaviour
    {
        [SerializeField] private Identifier _identifier;

        [Dropdown(FragmentServiceLibrary.EditorHolderIdsGetter)] [SerializeField]
        private string _desiredHolderId;

        public Identifier Identifier => _identifier;
        public string DesiredHolderId => _desiredHolderId;

        public virtual Task OnLink()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnUnlink()
        {
            return Task.CompletedTask;
        }
    }
}