using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Components.UI;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [RequireComponent(typeof(ElementView))]
    public abstract class Element<TView, TModel> : UIMonoBehaviour, IElement<TModel>
        where TView : ElementView
        where TModel : ElementModel
    {
        [SerializeField] private TView _view;

        protected TModel Model { get; private set; }
        protected TView View => _view;

        Task IElement.InitializeAsync(CancellationToken cancellationToken)
        {
            View.Interactable = false;
            View.Displayed = false;

            return OnInitializeAsync(cancellationToken);
        }

        protected abstract Task OnInitializeAsync(CancellationToken cancellationToken);

        void IElement<TModel>.SetModel(TModel model)
        {
            if (model == null)
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(model));
                return;
            }

            Model = model;
            Rebuild();
        }

        protected abstract void Rebuild();

        #region ISequencable

        Task ISequencable.PrepareShowAsync(CancellationToken cancellationToken)
        {
            return OnPrepareShowAsync(cancellationToken);
        }

        protected abstract Task OnPrepareShowAsync(CancellationToken cancellationToken);

        async Task ISequencable.ShowAsync(CancellationToken cancellationToken)
        {
            View.Interactable = true;
            View.Displayed = true;

            await View.ShowAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                await OnShowAsync(cancellationToken);
            }
        }

        protected abstract Task OnShowAsync(CancellationToken cancellationToken);

        Task ISequencable.PrepareHideAsync(CancellationToken cancellationToken)
        {
            View.Interactable = false;
            return OnPrepareHideAsync(cancellationToken);
        }

        protected abstract Task OnPrepareHideAsync(CancellationToken cancellationToken);

        async Task ISequencable.HideAsync(CancellationToken cancellationToken)
        {
            View.Interactable = false;

            await View.HideAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                await OnHideAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;

                View.Displayed = false;
            }
        }

        protected abstract Task OnHideAsync(CancellationToken cancellationToken);

        #endregion
    }
}