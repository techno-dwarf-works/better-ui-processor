using System;
using System.Threading;
using System.Threading.Tasks;
using Better.UISystem.Runtime.PopupsSystem.Popups;
using Better.UISystem.Runtime.PopupsSystem.Transitions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Better.UISystem.Runtime
{
    public class ModalSystem : IModalSystem
    {
        private readonly IModalSystem _internalSystem;
        private RectTransform _rectTransform;

        public ModalSystem(RectTransform container)
        {
            _internalSystem = new InternalModalSystem(container);
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