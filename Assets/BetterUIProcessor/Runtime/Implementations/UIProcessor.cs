using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.Tweens.Runtime.Data;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;
using Better.UIProcessor.Runtime.Sequences;
using Better.UIProcessor.Runtime.Settings;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [Serializable]
    public class UIProcessor<TElement>
        where TElement : IElement
    {
        [SerializeField] private ModulesContainer<TElement> _modules;
        [SerializeField] private ImplementationOverridable<Sequence> _defaultSequence;
        [SerializeField] private RectTransform _container;

        private Queue<TransitionInfo<TElement>> _transitionsQueue;

        public bool Initialized { get; private set; }
        public RectTransform Container => _container;
        public TElement OpenedElement { get; private set; }
        public bool InTransition => _transitionsQueue is { Count: < 0 };
        private SettingsData Settings => UIProcessorSettings.Instance.Current;
        public Sequence DefaultSequence => _defaultSequence.Value;

        public UIProcessor()
        {
            _defaultSequence = new ImplementationOverridable<Sequence>(Settings.DefaultSequence);
        }

        public void Initialize()
        {
            if (!ValidateInitialized(false))
            {
                return;
            }

            if (!ValidateContainer())
            {
                return;
            }

            _transitionsQueue = new();
            _defaultSequence.SetSource(Settings.DefaultSequence);

            Initialized = true;
        }

        public bool TryInitialize()
        {
            if (Initialized)
            {
                return false;
            }

            Initialize();
            return Initialized;
        }

        public UIProcessor<TElement> SetContainer(RectTransform value)
        {
            if (ValidateContainer(value))
            {
                _container = value;
            }

            return this;
        }

        public UIProcessor<TElement> SetDefaultSequence(Sequence value)
        {
            if (value == null)
            {
                DebugUtility.LogException<ArgumentNullException>(nameof(value));
                return this;
            }

            _defaultSequence.Override(value);
            return this;
        }

        public bool AddModule(Module<TElement> module)
        {
            if (!module.Link(this))
            {
                return false;
            }

            if (!_modules.TryAdd(module))
            {
                module.Unlink(this);
                return false;
            }

            return true;
        }

        public bool TryGetModule(Type type, out Module<TElement> module)
        {
            return _modules.TryGet(type, out module);
        }

        public bool RemoveModule(Module<TElement> module)
        {
            if (!_modules.Remove(module))
            {
                return false;
            }

            return module.Unlink(this);
        }

        internal async Task<TElement> RunTransitionAsync(TransitionInfo<TElement> transitionInfo)
        {
            TryInitialize();
            if (!ValidateInitialized(true))
            {
                transitionInfo.Cancel();
                return default;
            }

            if (_transitionsQueue.Contains(transitionInfo))
            {
                var message = $"{nameof(transitionInfo)} already queued, unexpected internal operation";
                throw new InvalidOperationException(message);
            }

            _transitionsQueue.Enqueue(transitionInfo);
            await _modules.OnEnqueuedTransition(this, transitionInfo);
            await AwaitTransitionQueue(transitionInfo);

            await _modules.OnTransitionStarted(this, OpenedElement, transitionInfo);
            var transitionResult = await ProcessTransitionAsync(OpenedElement, transitionInfo);
            if (transitionResult.IsSuccessful)
            {
                OpenedElement = transitionResult.Data;
                await _modules.OnTransitionCompleted(this, OpenedElement, transitionInfo);
            }
            else
            {
                transitionInfo.Cancel();
                await _modules.OnTransitionCanceled(this, transitionInfo);
            }

            if (!_transitionsQueue.Dequeue().Equals(transitionInfo))
            {
                var message = $"Unexpected internal dequeue, {transitionInfo}";
                throw new InvalidOperationException(message);
            }

            await _modules.OnDequeuedTransition(this, transitionInfo);
            return OpenedElement;
        }

        private async Task<ProcessResult<TElement>> ProcessTransitionAsync(TElement fromElement, TransitionInfo<TElement> transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<TElement>.Unsuccessful;
            }

            var elementResult = await TryGetTransitionElement(transitionInfo);
            if (!elementResult.IsSuccessful)
            {
                return ProcessResult<TElement>.Unsuccessful;
            }

            var toElement = elementResult.Data;
            var sequenceResult = await ProcessSequenceAsync(fromElement, toElement, transitionInfo);
            if (!sequenceResult.IsSuccessful)
            {
                await ReleaseElementAsync(toElement);
                return ProcessResult<TElement>.Unsuccessful;
            }

            if (fromElement != null)
            {
                await ReleaseElementAsync(fromElement);
            }

            return new ProcessResult<TElement>(toElement);
        }


        private async Task<ProcessResult<TElement>> ProcessSequenceAsync(TElement fromElement, TElement toElement, TransitionInfo<TElement> transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<TElement>.Unsuccessful;
            }

            var sequenceResult = await _modules.TryGetTransitionSequence(this, fromElement, toElement, transitionInfo);
            var sequence = sequenceResult.IsSuccessful ? sequenceResult.Data : DefaultSequence;

            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<TElement>.Unsuccessful;
            }

            await _modules.OnPreSequencePlay(this, fromElement, toElement, transitionInfo);
            await sequence.PlayAsync(fromElement, toElement);
            await _modules.OnPostSequencePlay(this, fromElement, toElement, transitionInfo);

            return new ProcessResult<TElement>(toElement);
        }

        private async Task AwaitTransitionQueue(TransitionInfo<TElement> transitionInfo)
        {
            if (_transitionsQueue.IsNullOrEmpty())
            {
                var message = $"{nameof(_transitionsQueue)} cannot be null or empty";
                DebugUtility.LogException<InvalidOperationException>(message);
                return;
            }

            await TaskUtility.WaitUntil(() => _transitionsQueue.Peek().Equals(transitionInfo));
        }

        private async Task<ProcessResult<TElement>> TryGetTransitionElement(TransitionInfo<TElement> transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<TElement>.Unsuccessful;
            }

            var elementResult = await _modules.TryGetTransitionElement(this, transitionInfo);
            if (transitionInfo.IsCanceled)
            {
                if (elementResult.IsSuccessful)
                {
                    await ReleaseElementAsync(elementResult.Data);
                }

                return ProcessResult<TElement>.Unsuccessful;
            }

            return elementResult;
        }

        private async Task ReleaseElementAsync(TElement element)
        {
            var releaseResult = await _modules.TryReleaseElement(this, element);
            if (!releaseResult)
            {
                element.RectTransform.DestroyGameObject();
            }

            await _modules.OnElementReleased(this);
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

    public class UIProcessor : UIProcessor<IElement>
    {
    }
}