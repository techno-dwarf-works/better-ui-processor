using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEditor.Animations;
using UnityEngine;

#if UNITY_EDITOR
using Better.Commons.EditorAddons.Extensions;
#endif

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public abstract class AnimatorSequence : Sequence
    {
        [SerializeField] private RuntimeAnimatorController _runtimeAnimatorController;

        // [Dropdown()]
        [SerializeField] private string _preparingTrigger;

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

            if (sequencable.RectTransform.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 1f;
            }

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
            if (sequencable.RectTransform.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 0f;
            }

            return true;
        }

        public override Sequence GetInverseSequence()
        {
            return Clone();
        }

        public IEnumerable<string> METHOD_NAME()
        {
#if UNITY_EDITOR
            if (_runtimeAnimatorController is AnimatorController animatorController)
            {
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
                // animatorController.GetAllIntegerNames();
            }
#endif

            return Array.Empty<string>();
        }
    }
}