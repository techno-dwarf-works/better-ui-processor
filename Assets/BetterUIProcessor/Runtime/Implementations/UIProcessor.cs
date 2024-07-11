using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.Commons.Runtime.Utility;
using Better.Tweens.Runtime.Data;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Modules;
using Better.UIProcessor.Runtime.Sequences;
using Better.UIProcessor.Runtime.Settings;
using UnityEngine;

namespace Better.UIProcessor.Runtime
{
    [Serializable]
    public class UIProcessor
    {
        // TODO: Create Module-Elements release contract

        [SerializeField] private ModulesContainer _modules;
        [SerializeField] private ImplementationOverridable<Sequence> _defaultSequence;
        [SerializeField] private RectTransform _container;

        private Queue<TransitionInfo> _transitionsQueue;

        public bool Initialized { get; private set; }
        public RectTransform Container => _container;
        public IElement OpenedElement { get; private set; }
        public bool InTransition => _transitionsQueue is { Count: < 0 };
        public Sequence DefaultSequence => _defaultSequence.Value;

        private SettingsData Settings => UIProcessorSettings.Instance.Current;

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
            _modules.Initialize();
            Initialized = true;
        }

        public UIProcessor SetContainer(RectTransform value)
        {
            if (ValidateContainer(value))
            {
                _container = value;
            }

            return this;
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

        public bool AddModule(Module module)
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

        public bool TryGetModule(Type type, out Module module)
        {
            return _modules.TryGet(type, out module);
        }

        public bool RemoveModule(Module module)
        {
            if (!_modules.Remove(module))
            {
                return false;
            }

            return module.Unlink(this);
        }

        public async Task ReleaseElementAsync(IElement element)
        {
            var releaseResult = await _modules.TryReleaseElement(this, element);
            if (!releaseResult)
            {
                element.RectTransform.DestroyGameObject();
            }

            await _modules.OnElementReleased(this);
        }

        internal async Task<bool> RunTransitionAsync(TransitionInfo transitionInfo)
        {
            this.TryInitialize();
            if (!ValidateInitialized(true))
            {
                return false;
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
                OpenedElement = transitionResult.Result;
                await _modules.OnTransitionCompleted(this, OpenedElement, transitionInfo);
            }
            else
            {
                await _modules.OnTransitionCanceled(this, transitionInfo);
            }

            if (!_transitionsQueue.Dequeue().Equals(transitionInfo))
            {
                var message = $"Unexpected internal dequeue, {transitionInfo}";
                throw new InvalidOperationException(message);
            }

            await _modules.OnDequeuedTransition(this, transitionInfo);
            return transitionResult.IsSuccessful;
        }

        private async Task<ProcessResult<IElement>> ProcessTransitionAsync(IElement fromElement, TransitionInfo transitionInfo)
        {
            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            var elementResult = await TryGetTransitionElement(transitionInfo);
            if (!elementResult.IsSuccessful)
            {
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

            var sequenceResult = await _modules.TryGetTransitionSequence(this, fromElement, toElement, transitionInfo);
            var sequence = sequenceResult.IsSuccessful ? sequenceResult.Result : DefaultSequence;

            if (transitionInfo.IsCanceled)
            {
                return ProcessResult<IElement>.Unsuccessful;
            }

            await _modules.OnPreSequencePlay(this, sequence, fromElement, toElement, transitionInfo);
            await sequence.PlayAsync(fromElement, toElement);
            await _modules.OnPostSequencePlay(this, sequence, fromElement, toElement, transitionInfo);

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

            var elementResult = await _modules.TryGetTransitionElement(this, transitionInfo);
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