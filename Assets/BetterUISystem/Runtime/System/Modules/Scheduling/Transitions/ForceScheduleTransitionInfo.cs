using System.Threading;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionRunners;

namespace Better.UISystem.Runtime.Modules.Scheduling
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