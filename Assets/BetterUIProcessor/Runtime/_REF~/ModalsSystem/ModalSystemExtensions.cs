using System.Threading.Tasks;
using Better.UIComposite.Runtime.PopupsSystem.Popups;

namespace Better.UIComposite.Runtime
{
    public static class ModalSystemExtensions
    {
        public static Task<TransitionResult<TPresenter>> ForceOpen<TPresenter, TModel>(this IModalSystem self) 
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel, new()
        {
            var model = new TModel();
            return self.CreateForceTransition<TPresenter, TModel>(model).RunAsync();
        }

        public static Task<TransitionResult<TPresenter>> ForceOpen<TPresenter, TModel>(this IModalSystem self, TModel model)
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel
        {
            return self.CreateForceTransition<TPresenter, TModel>(model).RunAsync();
        }

        public static Task<TransitionResult<TPresenter>> ScheduleOpen<TPresenter, TModel>(this IModalSystem self, int priority = default) 
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel, new()
        {
            var model = new TModel();
            return self.CreateScheduleTransition<TPresenter, TModel>(model)
                .OverridePriority(priority)
                .RunAsync();
        }

        public static Task<TransitionResult<TPresenter>> ScheduleOpen<TPresenter, TModel>(this IModalSystem self, TModel model, int priority)
            where TPresenter : Popup<TModel>, IModal
            where TModel : ModalModel
        {
            return self.CreateScheduleTransition<TPresenter, TModel>(model)
                .OverridePriority(priority)
                .RunAsync();
        }
    }
}