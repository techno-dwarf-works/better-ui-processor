﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Components.UI;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [RequireComponent(typeof(ElementView))]
    public abstract class Element<TView, TModel> : UIMonoBehaviour, IElement, IModelAssignable<TModel>
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

        void IModelAssignable<TModel>.AssignModel(TModel model)
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

        Task ISequencable.PreShowAsync(CancellationToken cancellationToken)
        {
            return OnPreShowAsync(cancellationToken);
        }

        protected abstract Task OnPreShowAsync(CancellationToken cancellationToken);

        async Task ISequencable.ShowAsync(CancellationToken cancellationToken)
        {
            View.Interactable = true;

            await View.ShowAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                await OnShowAsync(cancellationToken);
            }
        }

        protected abstract Task OnShowAsync(CancellationToken cancellationToken);
        
        Task ISequencable.PostShowAsync(CancellationToken cancellationToken)
        {
            View.Displayed = true;
            return OnPostShowAsync(cancellationToken);
        }

        protected abstract Task OnPostShowAsync(CancellationToken cancellationToken);

        
        Task ISequencable.PreHideAsync(CancellationToken cancellationToken)
        {
            View.Interactable = false;
            return OnPreHideAsync(cancellationToken);
        }

        protected abstract Task OnPreHideAsync(CancellationToken cancellationToken);

        async Task ISequencable.HideAsync(CancellationToken cancellationToken)
        {
            await View.HideAsync(cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                await OnHideAsync(cancellationToken);
            }
        }

        protected abstract Task OnHideAsync(CancellationToken cancellationToken);

        Task ISequencable.PostHideAsync(CancellationToken cancellationToken)
        {
            View.Displayed = false;
            return OnPostHideAsync(cancellationToken);
        }

        protected abstract Task OnPostHideAsync(CancellationToken cancellationToken);

        #endregion
    }
}