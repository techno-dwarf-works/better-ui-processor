using Better.UISystem.Runtime.PopupsSystem.Popups;
using Better.Validation.Runtime.Attributes;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public abstract class Modal : Popup, IModal
    {
        
    }
    
    public abstract class Modal<TModel> : Popup
        where TModel : PopupModel
    {
        protected TModel Model { get; private set; }

        public override void SetModel(PopupModel model)
        {
            if (model is TModel castedModel)
            {
                Model = castedModel;
                base.SetModel(model);
                return;
            }

            var message = $"[{GetType().Namespace}] {nameof(SetModel)}: unexpected {nameof(model)} type({model.GetType()})";
            Debug.LogError(message);
        }
    }

    public abstract class Modal<TModel, TView> : Popup<TModel>
        where TModel : PopupModel
        where TView : PopupView
    {
        [Header("REFERENCES")] 
        [NotNull] [SerializeField]
        private TView _view;

        protected TView View => _view;

        protected override PopupView GetDerivedView() => View;
    }
}