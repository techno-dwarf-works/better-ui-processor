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
    public abstract class DefaultSequence : Sequence
    {
        public sealed override async Task PlayAsync(RectTransform container, ISequencable from, ISequencable to)
        {
            var preparedTasks = new List<Task>(2);

            if (from != null)
            {
                var prepareHideTask = from.PrepareHideAsync(CancellationToken.None);
                preparedTasks.Add(prepareHideTask);
            }

            if (to != null)
            {
                to.RectTransform.SetAsLastSibling();

                var prepareShowTask = to.PrepareShowAsync(CancellationToken.None);
                preparedTasks.Add(prepareShowTask);
            }

            await preparedTasks.WhenAll();
            await PostPreparedProcessAsync(from, to, CancellationToken.None);
        }

        protected abstract Task PostPreparedProcessAsync(ISequencable from, ISequencable to, CancellationToken cancellationToken);

        protected async Task<bool> TryShowAsync(ISequencable sequencable, CancellationToken cancellationToken)
        {
            if (sequencable == null)
            {
                return false;
            }

            sequencable.Displayed = true;
            await sequencable.ShowAsync(cancellationToken);
            return true;
        }

        protected async Task<bool> TryHideAsync(ISequencable sequencable, CancellationToken cancellationToken)
        {
            if (sequencable == null)
            {
                return false;
            }

            await sequencable.HideAsync(cancellationToken);
            sequencable.Displayed = false;

            return true;
        }

        public override Sequence GetInverseSequence()
        {
            return Clone();
        }
    }
}