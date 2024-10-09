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
        public sealed override async Task PlayAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            await PreProcessAsync(container, from, to);
            await ProcessAsync(container, from, to);
            await PostProcessAsync(container, from, to);
        }

        protected virtual async Task PreProcessAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            var tasks = new List<Task>(2);

            if (from != null)
            {
                var preHideTask = from.PreHideAsync(CancellationToken.None);
                tasks.Add(preHideTask);
            }

            if (to != null)
            {
                var preShowTask = to.PreShowAsync(CancellationToken.None);
                tasks.Add(preShowTask);
            }

            await tasks.WhenAll();
        }

        protected abstract Task ProcessAsync(RectTransform container, ISequencable from, ISequencable to);

        protected virtual async Task PostProcessAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            var tasks = new List<Task>(2);

            if (from != null)
            {
                var postHideTask = from.PostHideAsync(CancellationToken.None);
                tasks.Add(postHideTask);
            }

            if (to != null)
            {
                var postShowTask = to.PostShowAsync(CancellationToken.None);
                tasks.Add(postShowTask);
            }

            await tasks.WhenAll();
        }

        protected virtual Task ShowAsync(ISequencable sequencable)
        {
            return sequencable.ShowAsync(CancellationToken.None);
        }

        protected Task TryShowAsync(ISequencable sequencable)
        {
            if (sequencable == null)
            {
                return Task.CompletedTask;
            }

            return ShowAsync(sequencable);
        }

        protected virtual Task HideAsync(ISequencable sequencable)
        {
            return sequencable.HideAsync(CancellationToken.None);
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