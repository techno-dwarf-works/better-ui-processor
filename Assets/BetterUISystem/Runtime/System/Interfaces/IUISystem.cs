using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Modules;
using Better.UISystem.Runtime.TransitionInfos;
using Better.UISystem.Runtime.TransitionRunners;
using UnityEngine;

namespace Better.UISystem.Runtime.Interfaces
{
    public interface IUISystem
    {
        bool Initialized { get; }
        bool Mutable { get; }
        bool IsRunning { get; }
        ISystemElement OpenedElement { get; }
        RectTransform Container { get; }
        void Initialize();
        Task AwaitTransitionActualization(TransitionInfo info);
        void Deconstruct();
        UISystem AddModule(SystemModule module);
        TModule AddModule<TModule>() where TModule : SystemModule, new();
        TModule AddModule<TModule>(out TModule module) where TModule : SystemModule, new();
        UISystem AddModule(IEnumerable<SystemModule> modules);
        TModule GetModule<TModule>() where TModule : SystemModule;
        UISystem SetContainer(RectTransform container);
        UISystem SetRuntimeRunner(TransitionRunner transitionRunner);
        UISystem SetRuntimeRunner<TRunner>() where TRunner : TransitionRunner, new();
        UISystem AddOverrideSequence(Sequence sequence);
        UISystem AddOverrideSequence(IEnumerable<Sequence> sequence);
        bool TryGetSequence(Type sequenceType, out Sequence sequence);
        Sequence GetDefaultSequence();
    }
}