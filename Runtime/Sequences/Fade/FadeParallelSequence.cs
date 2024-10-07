#if BETTER_TWEENS
using System;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Tweens.Runtime;
using Better.UIProcessor.Runtime.Interfaces;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Sequences
{
    [Serializable]
    public class FadeParallelSequence : ParallelSequence
    {
        [Min(default)]
        [SerializeField] private float _duration;

        public FadeParallelSequence(float duration)
        {
            _duration = duration;
        }

        public FadeParallelSequence() : this(default)
        {
        }

        protected override Task ShowAsync(ISequencable sequencable)
        {
            var tasks = new Task[2];
            tasks[0] = base.ShowAsync(sequencable);
            tasks[1] = Fade(sequencable, 1f);

            return tasks.WhenAll();
        }

        protected override Task HideAsync(ISequencable sequencable)
        {
            var tasks = new Task[2];
            tasks[0] = base.HideAsync(sequencable);
            tasks[1] = Fade(sequencable, 0f);

            return tasks.WhenAll();
        }

        private Task Fade(ISequencable sequencable, float alpha)
        {
            if (sequencable.RectTransform.TryGetComponent(out CanvasGroup canvasGroup))
            {
                return canvasGroup.TweenFade(_duration, alpha).AwaitPlaying();
            }

            return Task.CompletedTask;
        }

        public override Sequence Clone()
        {
            return new FadeParallelSequence(_duration);
        }
    }
}
#endif