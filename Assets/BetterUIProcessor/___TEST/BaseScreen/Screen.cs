using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.UIProcessor.Runtime;
using Better.UIProcessor.Runtime.Instructions;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace TEST
{
    public class Screen<TView, TModel> : Element<TView, TModel>, IFragmentsHolder
        where TView : ScreenView
        where TModel : ScreenModel
    {
        public bool LongInit;

        
        [Select]
        [SerializeReference] private FragmentInstruction _instruction;

        public FragmentInstruction Instruction => _instruction;
        public int Priority => GetComponent<FragmentPriority>().Priority;

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: OnInitialize");
            
            if (!LongInit) return;

            for (int i = 0; i < 5; i++)
            {
                Debug.Log("initialize: " + i);
                await Task.Delay(i * 1000);
            }
        }

        protected override void Rebuild()
        {
            Debug.Log($"{name}: Rebuild");
        }

        protected override Task OnPrepareShowAsync(CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: OnPrepare Show");
            return Task.CompletedTask;
        }

        protected override Task OnShowAsync(CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: OnShow");
            return Task.CompletedTask;
        }

        protected override Task OnPrepareHideAsync(CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: OnPrepare Hide");
            return Task.CompletedTask;
        }

        protected override Task OnHideAsync(CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: OnHide");
            return Task.CompletedTask;
        }

        public Task OnFragmentLinkedAsync(IFragment fragment, CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: fragment({fragment}) linked");
            return Task.CompletedTask;
        }

        public Task OnFragmentUnlinkedAsync(IFragment fragment, CancellationToken cancellationToken)
        {
            Debug.Log($"{name}: fragment({fragment}) unlinked");
            return Task.CompletedTask;
        }
    }
}