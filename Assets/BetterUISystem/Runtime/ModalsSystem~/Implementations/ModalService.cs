#if BETTER_SERVICES
using System.Threading;
using System.Threading.Tasks;
using Better.Services.Runtime;
using Better.UISystem.Runtime.PopupsSystem.Interfaces;
using Better.UISystem.Runtime.PopupsSystem.Popups;
using Better.UISystem.Runtime.PopupsSystem.Transitions;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    [RequireComponent(typeof(RectTransform))]
    public class ModalService : MonoService, IModalSystem
    {
        private IModalSystem _internalSystem;

        protected override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            var rectTransform = GetComponent<RectTransform>();
            _internalSystem = new InternalModalSystem(rectTransform);
            return Task.CompletedTask;
        }

        protected override Task OnPostInitializeAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #region IModalSystem

        public RectTransform Container => _internalSystem.Container;

        public bool HasOpened => _internalSystem.HasOpened;

        public bool InTransition => _internalSystem.InTransition;

        public ForcePopupTransitionInfo<TPresenter, TModel> CreateForceTransition<TPresenter, TModel>(TModel model,
            CancellationToken cancellationToken = default)
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel
        {
            return _internalSystem.CreateForceTransition<TPresenter, TModel>(model, cancellationToken);
        }

        public SchedulePopupTransitionInfo<TPresenter, TModel> CreateScheduleTransition<TPresenter, TModel>(TModel model,
            CancellationToken cancellationToken = default)
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel
        {
            return _internalSystem.CreateScheduleTransition<TPresenter, TModel>(model, cancellationToken);
        }

        public Task ClosePopup(Modal popup)
        {
            return _internalSystem.ClosePopup(popup);
        }

        #endregion
    }
}
#endif