using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Misc;
using Better.Services.Runtime;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.Modules;
using Better.UISystem.Runtime.TransitionInfos;
using Better.UISystem.Runtime.TransitionRunners;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public abstract class UISystemService : MonoService, IUISystem
    {
        [HideLabel]
        [SerializeField] private UISystem _uiSystem;

        public bool Mutable => _uiSystem.Mutable;

        public bool IsRunning => _uiSystem.IsRunning;

        public ISystemElement OpenedElement => _uiSystem.OpenedElement;

        public RectTransform Container => _uiSystem.Container;

        public void Initialize()
        {
            _uiSystem.Initialize();
        }

        public Task AwaitTransitionActualization(TransitionInfo info)
        {
            return _uiSystem.AwaitTransitionActualization(info);
        }

        public void Deconstruct()
        {
            _uiSystem.Deconstruct();
        }

        public UISystem AddModule(SystemModule module)
        {
            return _uiSystem.AddModule(module);
        }

        public TModule AddModule<TModule>() where TModule : SystemModule, new()
        {
            return _uiSystem.AddModule<TModule>();
        }

        public TModule AddModule<TModule>(out TModule module) where TModule : SystemModule, new()
        {
            return _uiSystem.AddModule(out module);
        }

        public UISystem AddModule(IEnumerable<SystemModule> modules)
        {
            return _uiSystem.AddModule(modules);
        }

        public TModule GetModule<TModule>() where TModule : SystemModule
        {
            return _uiSystem.GetModule<TModule>();
        }

        public UISystem SetContainer(RectTransform container)
        {
            return _uiSystem.SetContainer(container);
        }

        public UISystem SetRuntimeRunner(TransitionRunner transitionRunner)
        {
            return _uiSystem.SetRuntimeRunner(transitionRunner);
        }

        public UISystem SetRuntimeRunner<TRunner>() where TRunner : TransitionRunner, new()
        {
            return _uiSystem.SetRuntimeRunner<TRunner>();
        }

        public UISystem AddOverrideSequence(Sequence sequence)
        {
            return _uiSystem.AddOverrideSequence(sequence);
        }

        public UISystem AddOverrideSequence(IEnumerable<Sequence> sequence)
        {
            return _uiSystem.AddOverrideSequence(sequence);
        }

        public bool TryGetSequence(Type sequenceType, out Sequence sequence)
        {
            return _uiSystem.TryGetSequence(sequenceType, out sequence);
        }

        public Sequence GetDefaultSequence()
        {
            return _uiSystem.GetDefaultSequence();
        }
    }
}