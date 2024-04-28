using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Manipulation;
using Better.Attributes.Runtime.Select;
using Better.Commons.Runtime.Utility;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.Modules;
using Better.UISystem.Runtime.TransitionInfos;
using Better.UISystem.Runtime.TransitionRunners;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    [Serializable]
    public partial class UISystem : IUISystem, ITransitionRunner
    {
        [Select] 
        [SerializeReference] private List<SystemModule> _modules;

        [HideInPlayMode] [Select] 
        [SerializeReference] private TransitionRunner _runner;

        [Select] 
        [SerializeReference] private Sequence _defaultSequence;

        [Select] 
        [SerializeReference] private Sequence[] _overridenSequences;

        [SerializeField] private RectTransform _container;
        
        private Sequence _fallbackSequence = new DefaultSequence();
        private Queue<TransitionInfo> _transitionsQueue;
        private TransitionRunner _runtimeRunner;

        public bool Initialized { get; private set; }
        public bool Mutable => Initialized && !IsRunning;
        public bool IsRunning => _transitionsQueue.Count < 0;
        public ISystemElement OpenedElement { get; private set; }
        protected ModulesContainer ModulesContainer { get; private set; }
        protected List<Sequence> OverridenSequences { get; private set; }
        public RectTransform Container { get; private set; }

        public void Initialize()
        {
            _transitionsQueue = new Queue<TransitionInfo>();
            ModulesContainer = new ModulesContainer(_modules);
            OverridenSequences = new List<Sequence>(_overridenSequences);
            Container = _container;
            _runtimeRunner = _runner ?? new DefaultRunner();
            OnInitialize();
            Initialized = true;
        }

        async Task<ISystemElement> ITransitionRunner.RunAsync(TransitionInfo info)
        {
            _transitionsQueue.Enqueue(info);
            await AwaitTransitionActualization(info);
            var result = await _runtimeRunner.RunAsync(OpenedElement, info);
            if (result.IsSuccessful)
            {
                OpenedElement = result.Data;
            }

            OnRunExit(info);
            return OpenedElement;
        }

        public async Task AwaitTransitionActualization(TransitionInfo info)
        {
            await TaskUtility.WaitUntil(() => _transitionsQueue.Peek().Equals(info));
        }

        private void OnRunExit(TransitionInfo info)
        {
            if (!_transitionsQueue.Dequeue().Equals(info))
            {
                var unexpectedDequeMessage = "Unexpected dequeue";
                throw new InvalidOperationException(unexpectedDequeMessage);
            }
        }

        protected virtual void OnInitialize()
        {
            
        }

        protected virtual void OnDeconstruct()
        {
        }

        public void Deconstruct()
        {
            ModulesContainer.Deconstruct();
            OnDeconstruct();
            Initialized = false;
        }
    }
}