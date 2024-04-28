using System.Threading;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionRunners;

namespace Better.UISystem.Runtime.Modules.Scheduling
{
    public class ScheduleElementTransitionInfo<TPresenter, TModel> : ScheduleTransitionInfo<TPresenter, TModel>
        where TPresenter : SystemElement<TModel>
        where TModel : ElementModel
    {
        public ScheduleElementTransitionInfo(ITransitionRunner runner, ScheduleModule scheduleModule, TModel model, CancellationToken cancellationToken)
            : base(scheduleModule, runner, model, cancellationToken)
        {
        }

        public ScheduleElementTransitionInfo<TPresenter, TModel> OverridePriority(int value)
        {
            SetPriority(value);
            return this;
        }

        public ScheduleElementTransitionInfo<TPresenter, TModel> AddReadinessCondition(Condition condition)
        {
            AddReadinessConditionInternal(condition);
            return this;
        }

        public ScheduleElementTransitionInfo<TPresenter, TModel> AddCancellationCondition(Condition condition)
        {
            AddCancellationConditionInternal(condition);
            return this;
        }

        public Task<Result<TPresenter>> RunAsync()
        {
            ValidateRun();
            return ScheduleModule.ScheduleRunTransition(this);
        }
    }
}