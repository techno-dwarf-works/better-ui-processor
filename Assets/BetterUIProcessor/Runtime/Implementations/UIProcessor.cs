using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.Tweens.Runtime.Data;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;
using Better.UIProcessor.Runtime.Sequences;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [Serializable]
    public class UIProcessor : IModulesContainer
    {
        [SerializeField] private GroupModule _groupModule;
        [SerializeField] private ImplementationOverridable<Sequence> _defaultSequence;
        [SerializeField] private RectTransform _container;

        private Queue<TransitionInfo> _transitionsQueue;

        public bool Initialized { get; private set; }
        public RectTransform Container => _container;
        public IElement OpenedElement { get; private set; }
        public bool InTransition => _transitionsQueue is { Count: < 0 };
        public Sequence DefaultSequence => _defaultSequence.Value;

        public UIProcessor()
        {
            _groupModule = new();
            _defaultSequence = new ImplementationOverridable<Sequence>();
        }

        public Task InitializeAsync(RectTransform container)
        {
            if (!ValidateInitialized(false))
            {
                return Task.CompletedTask;
            }

            if (!ValidateContainer(container))
            {
                return Task.CompletedTask;
            }

            var settings = UIProcessorSettings.Instance.Current;

            _transitionsQueue ??= new();
            _defaultSequence.SetSource(settings.DefaultSequence);
            _container = container;
            _groupModule.Link(this);

            Initialized = true;
            return Task.CompletedTask;
        }

        public async Task InitializeAsync(RectTransform container, IElement prewarmedElement)
        {
            await InitializeAsync(container);
            await PrewarmOpenedElementAsync(prewarmedElement);
        }

        private async Task PrewarmOpenedElementAsync(IElement prewarmedElement)
        {
            if (!ValidateInitialized(true))
            {
                return;
            }

            if (InTransition)
            {
                var message = $"Cannot use {prewarmedElement} when in transition";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }

            var transitionInfo = new PrewarmTransitionInfo(this);
            _transitionsQueue.Enqueue(transitionInfo);

            await prewarmedElement.InitializeAsync(CancellationToken.None);
            await prewarmedElement.PrepareShowAsync(CancellationToken.None);
            await prewarmedElement.ShowAsync(CancellationToken.None);
            prewarmedElement.Displayed = true;
            
            OpenedElement = prewarmedElement;

            if (!_transitionsQueue.TryDequeue(out var dequeuedTransitionInfo)
                || dequeuedTransitionInfo != transitionInfo)
            {
                var message = $"Unexpected dequeue operation for {nameof(transitionInfo)}({transitionInfo})";
                DebugUtility.LogException<InvalidOperationException>(message);
            }
        }

        public UIProcessor SetDefaultSequence(Sequence value)
        {
            if (value == null)
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(value));
                return this;
            }

            _defaultSequence.Override(value);
            return this;
        }

        public bool TryAddModule(Module module)
        {
            return _groupModule.TryAddModule(module);
        }

        public bool ContainsModule(Module module)
        {
            return _groupModule.ContainsModule(module);
        }

        public bool RemoveModule(Module module)
        {
            return _groupModule.RemoveModule(module);
        }

        public async Task ReleaseElementAsync(IElement element)
        {
            await _groupModule.OnElementPreReleased(this, element);
            var releaseResult = await _groupModule.TryReleaseElement(this, element);
            if (!releaseResult)
            {
                element.RectTransform.DestroyGameObject();

                var message = $"No {nameof(Module)} to released element, was fallback destroy";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }

            await _groupModule.OnElementReleased(this);
        }

        internal async Task<bool> RunTransitionAsync(TransitionInfo transitionInfo)
        {
            this.TryInitialize();
            if (!ValidateInitialized(true))
            {
                return false;
            }

            if (!ValidateContainer())
            {
                return false;
            }

            if (_transitionsQueue.Contains(transitionInfo))
            {
                var message = $"{nameof(transitionInfo)} already queued, unexpected internal operation";
                throw new InvalidOperationException(message);
            }

            _transitionsQueue.Enqueue(transitionInfo);
            await _groupModule.OnEnqueuedTransition(this, transitionInfo);
            await AwaitTransitionQueue(transitionInfo);

            await _groupModule.OnTransitionStarted(this, OpenedElement, transitionInfo);
            var transitionResult = await ProcessTransitionAsync(OpenedElement, transitionInfo);
            if (transitionResult.IsSuccessful)
            {
                OpenedElement = transitionResult.Result;
                await _groupModule.OnTransitionCompleted(this, OpenedElement, transitionInfo);
            }
            else
            {
                await _groupModule.OnTransitionCanceled(this, transitionInfo);
            }

            if (!_transitionsQueue.Dequeue().Equals(transitionInfo))
            {
                var message = $"Unexpected internal dequeue, {transitionInfo}";
                throw new InvalidOperationException(message);
            }

            await _groupModule.OnDequeuedTransition(this, transitionInfo);
            return transitionResult.IsSuccessful;
        }

        private async Task<ProcessResult<IElement>> ProcessTransitionAsync(IElement fromElement, TransitionInfo transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            if (!ValidateContainer())
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            var elementResult = await TryGetTransitionElement(transitionInfo);
            if (!elementResult.IsSuccessful)
            {
                var message = $"No {nameof(Module)} to get element, process be unsuccessful";
                DebugUtility.LogException<InvalidOperationException>(message);

                return ProcessResult<IElement>.Unsuccessful;
            }

            var toElement = elementResult.Result;
            var sequenceResult = await ProcessSequenceAsync(fromElement, toElement, transitionInfo);
            if (!sequenceResult.IsSuccessful)
            {
                await ReleaseElementAsync(toElement);
                return ProcessResult<IElement>.Unsuccessful;
            }

            if (fromElement != null)
            {
                await ReleaseElementAsync(fromElement);
            }

            return new ProcessResult<IElement>(toElement);
        }

        private async Task<ProcessResult<IElement>> ProcessSequenceAsync(IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            var sequenceResult = await _groupModule.TryGetTransitionSequence(this, fromElement, toElement, transitionInfo);
            var sequence = sequenceResult.IsSuccessful ? sequenceResult.Result : DefaultSequence;

            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            await _groupModule.OnPreSequencePlay(this, sequence, fromElement, toElement, transitionInfo);
            await sequence.PlayAsync(Container, fromElement, toElement);
            await _groupModule.OnPostSequencePlay(this, sequence, fromElement, toElement, transitionInfo);

            return new ProcessResult<IElement>(toElement);
        }

        private async Task AwaitTransitionQueue(TransitionInfo transitionInfo)
        {
            if (_transitionsQueue.IsNullOrEmpty())
            {
                var message = $"{nameof(_transitionsQueue)} cannot be null or empty";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }

            await TaskUtility.WaitUntil(() => _transitionsQueue.Peek().Equals(transitionInfo));
        }

        private async Task<ProcessResult<IElement>> TryGetTransitionElement(TransitionInfo transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            var elementResult = await _groupModule.TryGetTransitionElement(this, transitionInfo);
            if (transitionInfo.IsCanceled)
            {
                if (elementResult.IsSuccessful)
                {
                    await ReleaseElementAsync(elementResult.Result);
                }

                return ProcessResult<IElement>.Unsuccessful;
            }

            return elementResult;
        }

        private bool ValidateInitialized(bool targetState, bool logException = true)
        {
            var isValid = Initialized == targetState;
            if (!isValid && logException)
            {
                var reason = targetState ? "must be initialized" : "must be not initialized";
                var message = "Not valid, " + reason;
                DebugUtility.LogException(message);
            }

            return isValid;
        }

        private bool ValidateContainer(RectTransform container, bool logException = true)
        {
            if (container == null)
            {
                if (logException)
                {
                    var message = $"{nameof(container)} not valid, cannot be null";
                    DebugUtility.LogException(message);
                }

                return false;
            }

            return true;
        }

        private bool ValidateContainer(bool logException = true)
        {
            return ValidateContainer(Container, logException);
        }
    }
}