using System;
using System.Threading;
using Better.UISystem.Runtime.Elements;

namespace Better.UISystem.Runtime.Modules.OpenElements
{
    [Serializable]
    public class OpenElementModule : SystemModule
    {
        public OpenTransitionInfo<TPresenter, TModel> CreateTransition<TPresenter, TModel>(TModel model, CancellationToken cancellationToken = default)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var transition = new OpenTransitionInfo<TPresenter, TModel>(System, model, cancellationToken);
            return transition;
        }

        public bool IsOpened<TPresenter, TModel>()
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            return System.OpenedElement is TPresenter;
        }

        public bool TryGetOpened<TPresenter, TModel>(out TPresenter screen)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            if (System.OpenedElement is TPresenter castedScreen)
            {
                screen = castedScreen;
                return true;
            }

            screen = default;
            return false;
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDeconstruct()
        {
        }
    }
}