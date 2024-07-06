using System.Threading;
using System.Threading.Tasks;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.TransitionRunners;

namespace Better.UIComposite.Runtime.Modules.Scheduling
{
    public class ForceScheduleTransitionInfo<TPresenter, TModel> : ScheduleTransitionInfo<TPresenter, TModel> 
        where TPresenter : SystemElement<TModel> 
        where TModel : ElementModel
    {
        public ForceScheduleTransitionInfo(ScheduleModule scheduleModule, ITransitionRunner runner, TModel model, CancellationToken cancellationToken)
            : base(scheduleModule, runner, model, cancellationToken)
        {
        }
        
        public Task<Result<TPresenter>> RunAsync()
        {
            ValidateRun();
            return ScheduleModule.ForceRunTransition(this);
        }
    }
}