using System;
using System.Threading.Tasks;
using Better.Commons.Runtime.Components.UI;
using Better.Commons.Runtime.Utility;
using Better.UISystem.Runtime.Interfaces;
using Better.Validation.Runtime.Attributes;
using UnityEngine;

namespace Better.UISystem.Runtime.Elements
{
    [RequireComponent(typeof(SystemElementView))]
    public abstract class SystemElement : UIMonoBehaviour, ISystemElement
    {
        private SystemElementView _derivedView;

        protected virtual void Awake()
        {
            _derivedView = GetDerivedView();
        }

        protected virtual void OnDestroy()
        {
            
        }

        protected abstract void Rebuild();

        public abstract void SetModel(ElementModel model);

        protected virtual SystemElementView GetDerivedView()
        {
            return GetComponent<SystemElementView>();
        }

        #region ILayerElement

        Task ISystemElement.InitializeAsync()
        {
            _derivedView.Interactable = false;
            _derivedView.Displayed = false;

            return OnInitializeAsync();
        }

        Task ISystemElement.PrepareShowAsync()
        {
            return OnPrepareShowAsync();
        }

        async Task ISystemElement.ShowAsync()
        {
            _derivedView.Interactable = true;
            _derivedView.Displayed = true;
            await _derivedView.ShowAsync();
            await OnShowAsync();
        }

        Task ISystemElement.PrepareHideAsync()
        {
            _derivedView.Interactable = false;
            return OnPrepareHideAsync();
        }

        async Task ISystemElement.HideAsync()
        {
            _derivedView.Interactable = false;
            await _derivedView.HideAsync();
            await OnHideAsync();
            _derivedView.Displayed = false;
        }

        #endregion

        protected abstract Task OnInitializeAsync();
        protected abstract Task OnPrepareShowAsync();
        protected abstract Task OnShowAsync();
        protected abstract Task OnPrepareHideAsync();
        protected abstract Task OnHideAsync();
    }

    public abstract class SystemElement<TModel> : SystemElement
        where TModel : ElementModel
    {
        protected TModel Model { get; private set; }

        public override void SetModel(ElementModel model)
        {
            if (model is TModel bufferModel)
            {
                Model = bufferModel;
                Rebuild();
                return;
            }
            
            DebugUtility.LogException<ArgumentException>($"Model is not type of {typeof(TModel).Name}");
        }
    }

    public abstract class SystemElement<TModel, TView> : SystemElement<TModel>
        where TModel : ElementModel
        where TView : SystemElementView
    {
        [Header("REFERENCES")]
        [NotNull] [SerializeField]
        private TView _view;

        protected TView View => _view;

        protected override SystemElementView GetDerivedView() => View;
    }
}