using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.TransitionInfos;

namespace Better.UIComposite.Runtime.Modules.Historical
{
    [Serializable]
    public class HistoryModule : SystemModule<HistoryTransitionInfo>
    {
        private Stack<HistoryInfo> _historyStack;

        private Dictionary<HistoryTransitionInfo, Type> _infoSequences;

        public HistoryTransitionInfo CreateTransition(int historyDepth, CancellationToken cancellationToken = default)
        {
            return new HistoryTransitionInfo(System, historyDepth, cancellationToken);
        }
        
        protected override void OnInitialize()
        {
            _historyStack = new Stack<HistoryInfo>();
            _infoSequences = new Dictionary<HistoryTransitionInfo, Type>();
        }

        protected override void OnDeconstruct()
        {
        }

        protected override async Task<Result<ISystemElement>> TryHandleOpen(HistoryTransitionInfo info)
        {
            if (!TryValidateDepth(info, out var depth))
            {
                return Result<ISystemElement>.GetUnsuccessful();
            }

            var historyInfos = new List<HistoryInfo>(depth);
            for (var index = 0; index < depth - 1; index++)
            {
                var bufferInfo = _historyStack.Pop();
                historyInfos.Add(bufferInfo);
            }

            var historyInfo = _historyStack.Pop();

            foreach (var (screen, screenTransitionInfo) in historyInfos)
            {
                await screen.ReleasedFormStackAsync();
                screen.RectTransform.DestroyGameObject();
            }
            
            _infoSequences.Add(info, historyInfo.Info.SequenceType);
            
            return new Result<ISystemElement>(historyInfo.Stackable);
        }

        protected internal override async Task<bool> TryHandleClose(ISystemElement element, TransitionInfo info)
        {
            if (System.OpenedElement is IStackingProvider && element is IStackable stackable)
            {
                await stackable.PushStackAsync();
                _historyStack.Push(new HistoryInfo(stackable, info));
                return true;
            }

            return false;
        }

        protected override Task<Result<Sequence>> TryGetTransitionSequence(HistoryTransitionInfo info)
        {
            Sequence sequence;
            if (info.OverridenSequence)
            {
                if (!System.TryGetSequence(info.SequenceType, out sequence))
                {
                    sequence = System.GetDefaultSequence();
                }

                return Task.FromResult(new Result<Sequence>(sequence));
            }
            
            if (_infoSequences.TryGetValue(info, out var type))
            {
                if (!System.TryGetSequence(type, out sequence))
                {
                    sequence = System.GetDefaultSequence();
                }

                sequence = sequence.GetInverseSequence();
                _infoSequences.Remove(info);
            }
            else
            {
                sequence = System.GetDefaultSequence();
            }

            return Task.FromResult(new Result<Sequence>(sequence));
        }

        protected internal override Task ElementOpened(ISystemElement element, TransitionInfo info)
        {
            if (element is not IStackingProvider)
            {
                ClearStack();
            }

            return Task.CompletedTask;
        }

        private bool TryValidateDepth(HistoryTransitionInfo info, out int depth)
        {
            depth = info.HistoryDepth;
            if (depth <= _historyStack.Count)
            {
                return true;
            }

            if (info.UseSafeDepth)
            {
                depth = _historyStack.Count;
                return true;
            }

            if (!info.AllowExceptions)
            {
                return true;
            }

            var exceptionMessage = $"{nameof(depth)} is bigger than history stack";
            DebugUtility.LogException<IndexOutOfRangeException>(exceptionMessage);
            depth = -1;
            return false;
        }

        private void ClearStack()
        {
            foreach (var info in _historyStack)
            {
                info.Stackable.RectTransform.DestroyGameObject();
            }

            _historyStack.Clear();
        }
    }
}