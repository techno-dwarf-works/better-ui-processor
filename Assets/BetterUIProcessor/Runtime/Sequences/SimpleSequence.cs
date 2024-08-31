using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public abstract class SimpleSequence : Sequence
    {
        public override async Task PlayAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            var preparedTasks = new List<Task>(2);

            if (from != null)
            {
                var prepareHideTask = from.PrepareHideAsync(CancellationToken.None);
                preparedTasks.Add(prepareHideTask);
            }

            if (to != null)
            {
                var prepareShowTask = to.PrepareShowAsync(CancellationToken.None);
                preparedTasks.Add(prepareShowTask);
            }

            await preparedTasks.WhenAll();
        }

        protected virtual async Task ShowAsync(ISequencable sequencable)
        {
            sequencable.Displayed = true;
            await sequencable.ShowAsync(CancellationToken.None);
        }

        protected Task TryShowAsync(ISequencable sequencable)
        {
            if (sequencable == null)
            {
                return Task.CompletedTask;
            }

            return ShowAsync(sequencable);
        }

        protected virtual async Task HideAsync(ISequencable sequencable)
        {
            await sequencable.HideAsync(CancellationToken.None);
            sequencable.Displayed = false;
        }

        protected Task TryHideAsync(ISequencable sequencable)
        {
            if (sequencable == null)
            {
                return Task.CompletedTask;
            }

            return HideAsync(sequencable);
        }

        public override Sequence GetInverseSequence()
        {
            return Clone();
        }
    }
}