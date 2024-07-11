// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Better.Commons.Runtime.Extensions;
// using Better.UIProcessor.Runtime.Data;
// using Better.UIProcessor.Runtime.Interfaces;
// using Better.UIProcessor.Runtime.Sequences;
// using UnityEngine;
//
// namespace Better.UIProcessor.Runtime.Modules
// {
//     [Serializable]
//     public class HistoricalModule : SingleModule
//     {
//         private struct HistoryPoint
//         {
//             public IHistoricalableElement Element { get; }
//             public Sequence RootSequence { get; }
//
//             public HistoryPoint(IHistoricalableElement element, Sequence rootSequence)
//             {
//                 Element = element;
//                 RootSequence = rootSequence;
//             }
//         }
//
//         public event Action Changed;
//         public event Action Cleared;
//
//         [SerializeField] private bool _autoClear;
//
//         private Stack<HistoryPoint> _history;
//         private Sequence _lastSequence;
//
//         public int Depth => _history?.Count ?? 0;
//         public bool IsEmpty => Depth == 0;
//         public bool AutoClear => _autoClear;
//
//         public HistoricalModule SetAutoClear(bool value = true)
//         {
//             _autoClear = value;
//             return this;
//         }
//
//         private void AddHistoryPoint(IHistoricalableElement historicalableElement, Sequence sequence)
//         {
//             var point = new HistoryPoint(historicalableElement, sequence);
//             _history ??= new();
//             _history.Push(point);
//
//             OnHistoryChanged();
//         }
//
//         private async Task<ProcessResult<HistoryPoint>> TryPopHistoryPointAsync()
//         {
//             if (!IsEmpty && _history.TryPop(out var historyPoint))
//             {
//                 if (Processor.OpenedElement != historyPoint.Element)
//                 {
//                     await historyPoint.Element.OnPopFromHistoryAsync();
//                 }
//
//                 return new ProcessResult<HistoryPoint>(historyPoint);
//             }
//
//             return ProcessResult<HistoryPoint>.Unsuccessful;
//         }
//
//         private async Task<ProcessResult<HistoryPoint>> TryPopHistoryPointAsync(int depth, bool safe)
//         {
//             if (!safe && Depth < depth)
//             {
//                 var message = $"Over {depth}({depth}), max can be used is {Depth}";
//                 Debug.LogWarning(message);
//                 return ProcessResult<HistoryPoint>.Unsuccessful;
//             }
//
//             if (depth < 1)
//             {
//                 return ProcessResult<HistoryPoint>.Unsuccessful;
//             }
//
//             depth = Math.Min(depth, Depth);
//             var popTasks = new Task<ProcessResult<HistoryPoint>>[depth];
//             for (int i = 0; i < depth; i++)
//             {
//                 popTasks[i] = TryPopHistoryPointAsync();
//             }
//
//             var results = await popTasks.WhenAll();
//             return results.Last();
//         }
//
//         protected virtual void OnHistoryChanged()
//         {
//             Changed?.Invoke();
//         }
//
//         public bool HistoryContains(IHistoricalableElement element)
//         {
//             if (_history == null)
//             {
//                 return false;
//             }
//
//             return _history.Any(p => p.Element == element);
//         }
//
//         protected internal override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, TransitionInfo transitionInfo)
//         {
//             var result = await base.TryGetTransitionElement(processor, transitionInfo);
//             if (result.IsSuccessful)
//             {
//                 return result;
//             }
//
//             if (transitionInfo is HistoricalTransitionInfo historicalTransitionInfo)
//             {
//                 var s = get
//             }
//
//             return ProcessResult<IElement>.Unsuccessful;
//         }
//
//         protected internal override async Task<ProcessResult<Sequence>> TryGetTransitionSequence(UIProcessor processor, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
//         {
//             var result = await base.TryGetTransitionSequence(processor, fromElement, toElement, transitionInfo);
//             if (result.IsSuccessful)
//             {
//                 return result;
//             }
//
//             if (transitionInfo is HistoricalTransitionInfo historicalTransitionInfo)
//             {
//                 xxxxxxxxxxxx
//             }
//
//             return ProcessResult<Sequence>.Unsuccessful;
//         }
//
//         protected internal override Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
//         {
//             _lastSequence = sequence;
//
//             return base.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);
//         }
//
//         protected internal override async Task<bool> TryReleaseElement(UIProcessor processor, IElement element)
//         {
//             var released = await base.TryReleaseElement(processor, element);
//             if (released)
//             {
//                 return true;
//             }
//
//             if (element is IHistoricalableElement historicalableElement
//                 && HistoryContains(historicalableElement))
//             {
//                 await historicalableElement.OnPushToHistoryAsync();
//                 return true;
//             }
//
//             return false;
//         }
//
//         protected internal override async Task OnTransitionCompleted(UIProcessor processor, IElement openedElement, TransitionInfo transitionInfo)
//         {
//             await base.OnTransitionCompleted(processor, openedElement, transitionInfo);
//
//             if (openedElement is IHistoricalableElement historicalableElement)
//             {
//                 AddHistoryPoint(historicalableElement, _lastSequence);
//             }
//             else if (AutoClear)
//             {
//                 Clear();
//             }
//         }
//
//         public HistoricalModule Clear()
//         {
//             if (IsEmpty)
//             {
//                 return this;
//             }
//
//             TryPopHistoryPoints(Depth, false);
//             OnHistoryCleared();
//
//             return this;
//         }
//
//         protected virtual void OnHistoryCleared()
//         {
//             Cleared?.Invoke();
//         }
//     }
// }