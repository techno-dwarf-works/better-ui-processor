using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionInfos;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Better.UISystem.Runtime.Modules.Scheduling
{
    [Serializable]
    public class ScheduleModule : SystemModule
    {
        private TransitionInfo _currentTransition;
        private TaskCompletionSource<ISystemElement> _transitionSource;
        private SortedList<int, ScheduleTransitionInfo> _scheduledTransitions;
        public bool HasOpened => System.OpenedElement != null;
        public bool InTransition => _currentTransition != null;

        protected override void OnInitialize()
        {
            _scheduledTransitions = new();
            PlayerLoopUtility.SubscribeToLoop<PostLateUpdate>(LateUpdate);
        }

        protected override void OnDeconstruct()
        {
            PlayerLoopUtility.UnsubscribeFromLoop<PostLateUpdate>(LateUpdate);
        }

        private void LateUpdate()
        {
            UpdateScheduledTransitions();
            TryNextPopup();
        }

        private void UpdateScheduledTransitions()
        {
            for (int i = _scheduledTransitions.Count - 1; i >= 0; i--)
            {
                var popupTransitionInfo = _scheduledTransitions[i];
                if (popupTransitionInfo.IsRelevant()) continue;

                popupTransitionInfo.Cancel();
                _scheduledTransitions.RemoveAt(i);
            }
        }

        private void TryNextPopup()
        {
            if (HasOpened || InTransition) return;

            if (TryPopScheduledTransition(out var poppedTransitionInfo))
            {
                RunTransitionAsync(poppedTransitionInfo).Forget();
            }
        }

        private bool TryPopScheduledTransition(out TransitionInfo transitionInfo)
        {
            for (var index = _scheduledTransitions.Count - 1; index >= 0; index--)
            {
                var scheduledTransition = _scheduledTransitions[index];
                if (!scheduledTransition.IsReadiness()) continue;

                transitionInfo = scheduledTransition;
                return true;
            }

            transitionInfo = null;
            return false;
        }

        public async Task<Result<TPresenter>> ScheduleRunTransition<TPresenter, TModel>(ScheduleTransitionInfo<TPresenter, TModel> info)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            _scheduledTransitions.Add(info.Priority, info);
            UpdateScheduledTransitions();
            TryNextPopup();

            return await AwaitTransitionResult<TPresenter, TModel>(info);
        }

        public async Task<Result<TPresenter>> ForceRunTransition<TPresenter, TModel>(ForceScheduleTransitionInfo<TPresenter, TModel> info)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            RunTransitionAsync(info).Forget();

            return await AwaitTransitionResult<TPresenter, TModel>(info);
        }

        public ForceScheduleTransitionInfo<TPresenter, TModel> CreateForceTransition<TPresenter, TModel>(TModel model,
            CancellationToken cancellationToken = default)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            var transition = new ForceScheduleTransitionInfo<TPresenter, TModel>(this, System, model, cancellationToken);

            return transition;
        }

        public ScheduleElementTransitionInfo<TPresenter, TModel> CreateScheduleTransition<TPresenter, TModel>(TModel model,
            CancellationToken cancellationToken = default)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            var transition = new ScheduleElementTransitionInfo<TPresenter, TModel>(System, this, model, cancellationToken);

            return transition;
        }

        private async Task<Result<TPresenter>> AwaitTransitionResult<TPresenter, TModel>(TransitionInfo info)
            where TPresenter : SystemElement<TModel>
            where TModel : ElementModel
        {
            await System.AwaitTransitionActualization(info);
            if (info.IsRelevant())
            {
                if (System.OpenedElement is TPresenter presenter)
                {
                    return new Result<TPresenter>(presenter);
                }
            }

            return Result<TPresenter>.GetUnsuccessful();
        }

        protected internal override async Task RunStarted(TransitionInfo info)
        {
            if (info.Mutable)
            {
                var message = $"{info} cannot be mutable";
                Debug.LogError(message);
                info.Cancel();
                return;
            }

            await TaskUtility.WaitUntil(() => !InTransition);
            if (!info.IsRelevant())
            {
                info.Cancel();
            }
        }
    }
}