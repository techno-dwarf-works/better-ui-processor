using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Data
{
    public abstract class TransitionInfo
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TaskCompletionSource<bool> _completionSource;

        public Type SequenceType { get; private set; }
        public bool OverridenSequence { get; private set; }
        public bool IsCanceled => _cancellationTokenSource.IsCancellationRequested;
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public bool Used { get; private set; }
        public bool Mutable => !Used;
        
        protected UIProcessor Processor { get; }

        protected TransitionInfo(UIProcessor processor, CancellationToken cancellationToken = default)
        {
            Processor = processor;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _completionSource = new(CancellationToken);
        }

        public TransitionInfo Run()
        {
            RunningAsync().Forget();
            return this;
        }

        protected virtual async Task RunningAsync()
        {
            if (Used) return;
            Used = true;

            var result = await Processor.RunTransitionAsync(this);
            _completionSource.TrySetResult(result);
        }

        public Task<bool> Await()
        {
            return _completionSource.Task;
        }

        public TransitionInfo OverrideSequence<TSequence>()
            where TSequence : Sequence
        {
            if (ValidateMutable(true))
            {
                OverridenSequence = true;
                SequenceType = typeof(TSequence);
            }

            return this;
        }

        public virtual TransitionInfo Cancel()
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
                .AppendFieldLine(nameof(Used), Used)
                .AppendFieldLine(nameof(Mutable), Mutable)
                .AppendFieldLine(nameof(IsCanceled), IsCanceled);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            CollectInfo(ref stringBuilder);

            return stringBuilder.ToString();
        }
    }
}