using System;
using System.Text;
using System.Threading;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.Modules.OpenElements;
using Better.UIComposite.Runtime.TransitionInfos;
using Better.UIComposite.Runtime.TransitionRunners;

namespace Better.UIComposite.Runtime.Modules.Scheduling
{
    public abstract class ScheduleTransitionInfo : OpenTransitionInfo
    {
        public int Priority { get; private set; }
        protected ScheduleModule ScheduleModule { get; }

        private ConditionIterator _readinessConditions;
        private ConditionIterator _cancellationConditions;

        public ScheduleTransitionInfo(ScheduleModule scheduleModule, ITransitionRunner runner, ElementModel model, Type presenterType,
            CancellationToken cancellationToken) 
            : base(runner, model, presenterType, cancellationToken)
        {
            ScheduleModule = scheduleModule;
            _readinessConditions = new ConditionIterator();
            _cancellationConditions = new ConditionIterator();
        }

        protected void SetPriority(int value)
        {
            Priority = value;
        }

        #region Conditions

        protected void AddReadinessConditionInternal(Condition condition)
        {
            ValidateMutable();

            _readinessConditions.Add(condition);
        }

        protected void AddCancellationConditionInternal(Condition condition)
        {
            ValidateMutable();

            _cancellationConditions.Add(condition);
        }

        public override bool IsRelevant()
        {
            if (_cancellationConditions.Any(true))
            {
                Cancel();
            }

            return base.IsRelevant();
        }

        #endregion

        protected override StringBuilder BuildLogInfo()
        {
            var builder = base.BuildLogInfo();
            builder.AppendLine()
                .AppendFormat("{0}:{1}", nameof(PresenterType), PresenterType.Name)
                .AppendLine()
                .AppendFormat("{0}:{1}", nameof(_readinessConditions), _readinessConditions.Count.ToString())
                .AppendLine()
                .AppendFormat("{0}:{1}", nameof(_cancellationConditions), _cancellationConditions.Count.ToString());

            return builder;
        }
    }

    public abstract class ScheduleTransitionInfo<TPresenter, TModel> : ScheduleTransitionInfo
        where TPresenter : SystemElement<TModel>
        where TModel : ElementModel
    {
        public ScheduleTransitionInfo(ScheduleModule scheduleModule, ITransitionRunner runner, ElementModel model, CancellationToken cancellationToken)
            : base(scheduleModule, runner, model, typeof(TPresenter), cancellationToken)
        {
        }
    }
}