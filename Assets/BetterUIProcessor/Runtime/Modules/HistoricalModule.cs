using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class HistoricalModule : SingleModule
    {
        private class HistoryPoint
        {
            public static readonly HistoryPoint Empty = new();

            public readonly IHistoricalableElement Element;
            public readonly Sequence RootSequence;

            public HistoryPoint(IHistoricalableElement element, Sequence rootSequence)
            {
                Element = element;
                RootSequence = rootSequence;
            }

            private HistoryPoint()
            {
            }
        }

        public event Action Changed;
        public event Action Cleared;

        [SerializeField] private bool _autoClear;

        private Stack<HistoryPoint> _history;
        private List<HistoryPoint> _unreleasedBuffer;
        private HistoryPoint _currentPoint;

        public int Depth => _history?.Count ?? 0;
        public bool IsEmpty => Depth == 0;
        public bool AutoClear => _autoClear;
        public override int Priority => ModulePriority.Resolver;

        public HistoricalModule SetAutoClear(bool value = true)
        {
            _autoClear = value;
            return this;
        }

        protected internal override bool Link(UIProcessor processor)
        {
            var success = base.Link(processor);
            if (success)
            {
                _history = new();
                _unreleasedBuffer = new();
            }

            return success;
        }

        protected internal override bool Unlink(UIProcessor processor)
        {
            var success = base.Unlink(processor);
            if (success)
            {
                Clear();
            }

            return success;
        }

        protected internal override Task OnTransitionStarted(UIProcessor processor, IElement fromElement, TransitionInfo transitionInfo)
        {
            _currentPoint = HistoryPoint.Empty;
            return base.OnTransitionStarted(processor, fromElement, transitionInfo);
        }

        protected internal override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (transitionInfo is HistoricalTransitionInfo historicalTransitionInfo)
            {
                var popResult = await TryPopFromHistoryAsync(historicalTransitionInfo.Depth, historicalTransitionInfo.UseSafeDepth);
                if (popResult.IsSuccessful)
                {
                    _unreleasedBuffer.Add(popResult.Result);

                    var element = popResult.Result.Element;
                    element.RectTransform.SetParent(processor.Container);
                    element.RectTransform.SetAsLastSibling();
                    return new ProcessResult<IElement>(element);
                }
            }

            return ProcessResult<IElement>.Unsuccessful;
        }

        protected internal override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            if (transitionInfo.OverridenSequence)
            {
                return ProcessResult<Sequence>.Unsuccessful;
            }

            if (transitionInfo is HistoricalTransitionInfo && _unreleasedBuffer.Count > 0)
            {
                var lastBufferPoint = _unreleasedBuffer.Last();
                if (lastBufferPoint.Element == toElement)
                {
                    var inverseSequence = lastBufferPoint.RootSequence.GetInverseSequence();
                    return new ProcessResult<Sequence>(inverseSequence);
                }
            }

            return ProcessResult<Sequence>.Unsuccessful;
        }

        protected internal override Task OnPostSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            if (transitionInfo is not HistoricalTransitionInfo
                && fromElement is IHistoricalableElement historicalableElement
                && toElement is IHistoricalableElement)
            {
                _currentPoint = new(historicalableElement, sequence);
            }

            return base.OnPostSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);
        }

        protected internal override async Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCompleted(processor, openedElement, transitionInfo);

            if (_unreleasedBuffer.Count > 0)
            {
                _unreleasedBuffer = _unreleasedBuffer.SkipLast(1).ToList();
                await ReleaseBufferAsync();
            }
            else if (_currentPoint == HistoryPoint.Empty && AutoClear)
            {
                await ClearAsync();
            }

            _currentPoint = HistoryPoint.Empty;
        }

        protected internal override async Task OnTransitionCanceled(UIProcessor processor, TransitionInfo transitionInfo)
        {
            await base.OnTransitionCanceled(processor, transitionInfo);

            await PushToHistoryAsync(_unreleasedBuffer);

            _unreleasedBuffer.Clear();
            _currentPoint = HistoryPoint.Empty;
        }

        protected internal override async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
        {
            var released = await base.TryReleaseElement(processor, element);
            if (released)
            {
                return true;
            }

            if (_currentPoint != HistoryPoint.Empty && _currentPoint.Element == element)
            {
                await PushToHistoryAsync(_currentPoint);
                return true;
            }

            return false;
        }

        public async Task ClearAsync()
        {
            if (IsEmpty)
            {
                return;
            }

            var popResult = await TryPopFromHistoryAsync(Depth, true);
            if (popResult.IsSuccessful)
            {
                _unreleasedBuffer.Add(popResult.Result);
            }

            await ReleaseBufferAsync();
            OnHistoryCleared();
        }

        public HistoricalModule Clear()
        {
            ClearAsync().Forget();
            return this;
        }

        private async Task PushToHistoryAsync(HistoryPoint historyPoint)
        {
            _history.Push(historyPoint);

            await historyPoint.Element.OnPushToHistoryAsync(CancellationToken.None);
            OnHistoryChanged();
        }

        private Task PushToHistoryAsync(IEnumerable<HistoryPoint> historyPoints)
        {
            return historyPoints.Select(PushToHistoryAsync)
                .WhenAll();
        }

        private async Task<ProcessResult<HistoryPoint>> TryPopFromHistoryAsync()
        {
            if (IsEmpty)
            {
                return ProcessResult<HistoryPoint>.Unsuccessful;
            }

            if (_history.TryPop(out var historyPoint))
            {
                await historyPoint.Element.OnPopFromHistoryAsync(CancellationToken.None);
                OnHistoryChanged();

                return new ProcessResult<HistoryPoint>(historyPoint);
            }

            return ProcessResult<HistoryPoint>.Unsuccessful;
        }

        private async Task<ProcessResult<HistoryPoint>> TryPopFromHistoryAsync(int depth, bool safe)
        {
            if (!safe && Depth < depth)
            {
                var message = $"Over {depth}({depth}), expected max value is {Depth}";
                Debug.LogWarning(message);
                return ProcessResult<HistoryPoint>.Unsuccessful;
            }

            depth = Math.Min(depth, Depth);
            if (depth < 1)
            {
                return ProcessResult<HistoryPoint>.Unsuccessful;
            }

            var popTasks = new Task<ProcessResult<HistoryPoint>>[depth];
            for (int i = 0; i < popTasks.Length; i++)
            {
                popTasks[i] = TryPopFromHistoryAsync();
            }

            var poppedPoints = await popTasks.WhenAll();
            if (poppedPoints.Length > 1)
            {
                var bufferPoints = poppedPoints.Take(poppedPoints.Length - 1)
                    .Where(p => p.IsSuccessful).Select(p => p.Result);

                _unreleasedBuffer.AddRange(bufferPoints);
            }

            var lastPoint = poppedPoints.Last();
            if (lastPoint.IsSuccessful)
            {
                return new ProcessResult<HistoryPoint>(lastPoint.Result);
            }

            return ProcessResult<HistoryPoint>.Unsuccessful;
        }

        public Task ReleaseBufferAsync()
        {
            var elements = _unreleasedBuffer.Select(p => p.Element).ToArray();
            _unreleasedBuffer.Clear();

            return Processor.ReleaseElementsAsync(elements);
        }

        protected virtual void OnHistoryChanged()
        {
            Changed?.Invoke();
        }

        protected virtual void OnHistoryCleared()
        {
            Cleared?.Invoke();
        }
    }
}