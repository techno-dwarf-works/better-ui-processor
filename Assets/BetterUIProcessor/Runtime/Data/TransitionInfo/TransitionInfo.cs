using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Data
{
    public abstract class TransitionInfo<TElement>
        where TElement : IElement
    {
        private HashSet<UIProcessor<TElement>> _processors;
        private CancellationTokenSource _cancellationTokenSource;

        public Type SequenceType { get; private set; }
        public bool OverridenSequence { get; private set; }
        public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public int LockCount { get; private set; }
        public bool Mutable => LockCount <= 0;

        protected TransitionInfo(CancellationToken cancellationToken = default)
        {
            _processors = new();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        public TransitionInfo<TElement> AddProcessor(UIProcessor<TElement> processor)
        {
            if (ValidateMutable(true))
            {
                _processors.Add(processor);
            }

            return this;
        }

        public TransitionInfo<TElement> RemoveProcessor(UIProcessor<TElement> processor)
        {
            if (ValidateMutable(true))
            {
                _processors.Remove(processor);
            }

            return this;
        }

        public TransitionInfo<TElement> OverrideSequence<TSequence>()
            where TSequence : Sequence
        {
            if (ValidateMutable(true))
            {
                OverridenSequence = true;
                SequenceType = typeof(TSequence);
            }

            return this;
        }

        internal TransitionInfo<TElement> TakeLock()
        {
            LockCount++;
            return this;
        }

        internal TransitionInfo<TElement> ReleaseLock()
        {
            LockCount--;
            LockCount = Math.Max(LockCount, 0);

            return this;
        }

        public virtual TransitionInfo<TElement> Cancel()
        {
            _cancellationTokenSource.Cancel();
            return this;
        }

        protected bool ValidateMutable(bool targetState, bool logException = true)
        {
            var isValid = Mutable == targetState;
            if (!isValid && logException)
            {
                var reason = targetState ? "must be mutable" : "must be immutable";
                var message = "Not valid, " + reason;
                DebugUtility.LogException<AccessViolationException>(message);
            }

            return isValid;
        }

        public virtual void CollectInfo(ref StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine(GetType().Name)
                .AppendFieldLine(nameof(SequenceType), SequenceType)
                .AppendFieldLine(nameof(OverridenSequence), OverridenSequence)
                .AppendLine()
                .AppendFieldLine(nameof(LockCount), LockCount)
                .AppendFieldLine(nameof(Mutable), Mutable)
                .AppendLine()
                .AppendFieldLine(nameof(IsCancellationRequested), IsCancellationRequested)
                .AppendFieldLine(nameof(CancellationToken), CancellationToken);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            CollectInfo(ref stringBuilder);

            return stringBuilder.ToString();
        }
    }
}