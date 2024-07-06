using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.Attributes.Runtime.Misc;
using Better.Services.Runtime;
using Better.UIComposite.Runtime.Common;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.Modules;
using Better.UIComposite.Runtime.TransitionInfos;
using Better.UIComposite.Runtime.TransitionRunners;
using UnityEngine;

namespace Better.UIComposite.Runtime
{
    public abstract class UICompositeService : MonoService, IUISystem
    {
        xxxxxxxxxxxx
            // Зробити створення інстансу - поведінкою в системі, а не модулем
            // Прибрати OpenElementModule в Систему
            // Базові абстрактні імпл системи - Сінглтон та Сервіси
            // фрагмент модуль
            // базові екрани-попапи(модалки+тости) в екземпли
        
        [HideLabel]
        [SerializeField] private UIComposite _UIComposite;

        public bool Mutable => _UIComposite.Mutable;

        public bool IsRunning => _UIComposite.IsRunning;

        public ISystemElement OpenedElement => _UIComposite.OpenedElement;

        public RectTransform Container => _UIComposite.Container;

        public void Initialize()
        {
            _UIComposite.Initialize();
        }

        public Task AwaitTransitionActualization(TransitionInfo info)
        {
            return _UIComposite.AwaitTransitionActualization(info);
        }

        public void Deconstruct()
        {
            _UIComposite.Deconstruct();
        }

        public UIComposite AddModule(SystemModule module)
        {
            return _UIComposite.AddModule(module);
        }

        public TModule AddModule<TModule>() where TModule : SystemModule, new()
        {
            return _UIComposite.AddModule<TModule>();
        }

        public TModule AddModule<TModule>(out TModule module) where TModule : SystemModule, new()
        {
            return _UIComposite.AddModule(out module);
        }

        public UIComposite AddModule(IEnumerable<SystemModule> modules)
        {
            return _UIComposite.AddModule(modules);
        }

        public TModule GetModule<TModule>() where TModule : SystemModule
        {
            return _UIComposite.GetModule<TModule>();
        }

        public UIComposite SetContainer(RectTransform container)
        {
            return _UIComposite.SetContainer(container);
        }

        public UIComposite SetRuntimeRunner(TransitionRunner transitionRunner)
        {
            return _UIComposite.SetRuntimeRunner(transitionRunner);
        }

        public UIComposite SetRuntimeRunner<TRunner>() where TRunner : TransitionRunner, new()
        {
            return _UIComposite.SetRuntimeRunner<TRunner>();
        }

        public UIComposite AddOverrideSequence(Sequence sequence)
        {
            return _UIComposite.AddOverrideSequence(sequence);
        }

        public UIComposite AddOverrideSequence(IEnumerable<Sequence> sequence)
        {
            return _UIComposite.AddOverrideSequence(sequence);
        }

        public bool TryGetSequence(Type sequenceType, out Sequence sequence)
        {
            return _UIComposite.TryGetSequence(sequenceType, out sequence);
        }

        public Sequence GetDefaultSequence()
        {
            return _UIComposite.GetDefaultSequence();
        }
    }
}