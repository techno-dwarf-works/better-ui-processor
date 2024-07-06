using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime;
using Better.Attributes.Runtime.Manipulation;
using Better.Extensions.Runtime;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    public abstract class FragmentsScreen<TModel, TView> : Screen<TModel, TView>, IFragmentContainer where TView : ScreenView where TModel : ScreenModel
    {
        [SerializeReference] private FragmentCondition _condition;
        [ReadOnly] [SerializeField] private FragmentHolder[] _holders;
        [SerializeField] private int _priority;

        private ServiceContainer<FragmentService> _fragmentsService = new ServiceContainer<FragmentService>();

        private IFragmentContainer _subContainer;

        public int Priority => _priority;

        public FragmentCondition[] GetFragmentConditions()
        {
            var array = new[] { _condition };
            if (_subContainer != null)
            {
                array = array.Concat(_subContainer.GetFragmentConditions()).ToArray();
            }

            return array;
        }

        public FragmentHolder[] GetHolders()
        {
            var array = _holders;
            if (_subContainer != null)
            {
                array = array.Concat(_subContainer.GetHolders()).ToArray();
            }

            return array;
        }

        public FragmentHolder GetHolderById(string id)
        {
            var holders = GetHolders();
            return holders.FirstOrDefault(holder => holder.Id == id);
        }

        protected override Task OnInitializeAsync()
        {
            _fragmentsService.Service.RegisterContainer(this);

            return Task.CompletedTask;
        }

        protected override async Task OnShowAsync()
        {
            await GetHolders().Select(holder => holder.Show(CancellationToken.None)).WhenAll();
            await _fragmentsService.Service.Link(this);
        }

        protected override Task OnPrepareShowAsync()
        {
            return _fragmentsService.Service.RequestFragments(this);
        }

        protected override async Task OnHideAsync()
        {
            await _fragmentsService.Service.UnregisterContainer(this);
            await GetHolders().Select(holder => holder.Hide(CancellationToken.None)).WhenAll();
        }

        protected override Task OnPrepareHideAsync()
        {
            return _fragmentsService.Service.ReleaseFragments(this);
        }

        protected void SetSubContainer(IFragmentContainer container)
        {
            _subContainer = container;
            _fragmentsService.Service.RegisterContainer(this);
        }

        [EditorButton("COLLECT HOLDERS")]
        private void CollectHolders()
        {
            _holders = GetComponentsInChildren<FragmentHolder>();
        }
    }
}