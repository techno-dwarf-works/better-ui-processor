using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.UIComposite.Runtime.Common;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    public class FragmentHolder : UIMonoBehaviour
    {
        [Dropdown(FragmentServiceLibrary.EditorHolderIdsGetter)] [SerializeField]
        private string _id;

        [SerializeReference] private IAnimation _animation;

        public string Id => _id;

        private void Awake()
        {
            _animation.Initialize(gameObject);
        }

        public Task Show(CancellationToken token)
        {
            return _animation.Forward(token);
        }

        public Task Hide(CancellationToken token)
        {
            return _animation.Backward(token);
        }
    }
}