using System;
using System.Collections.Generic;
using Better.Commons.Runtime.Utility;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Modules;
using Better.UISystem.Runtime.TransitionRunners;
using UnityEngine;

namespace Better.UISystem.Runtime
{
    public partial class UISystem
    {
        public UISystem AddModule(SystemModule module)
        {
            if (!ValidateMutable())
            {
                return this;
            }

            var type = module.GetType();
            if (!ModulesContainer.TryAddModule(module))
            {
                DebugUtility.LogException<InvalidOperationException>($"Module of type: {type} already added");
            }

            module.Initialize(this);
            return this;
        }

        public TModule AddModule<TModule>() where TModule : SystemModule, new()
        {
            AddModule<TModule>(out var module);
            return module;
        }

        public TModule AddModule<TModule>(out TModule module) where TModule : SystemModule, new()
        {
            module = new TModule();
            AddModule(module);
            return module;
        }

        public UISystem AddModule(IEnumerable<SystemModule> modules)
        {
            if (!ValidateMutable())
            {
                return this;
            }

            foreach (var module in modules)
            {
                AddModule(module);
            }

            return this;
        }

        public TModule GetModule<TModule>() where TModule : SystemModule
        {
            if (!ValidateMutable())
            {
                return null;
            }

            if (ModulesContainer.TryGetModule(typeof(TModule), out var module))
            {
                return (TModule)module;
            }

            DebugUtility.LogException<KeyNotFoundException>("Module not found");
            return null;
        }

        public UISystem SetContainer(RectTransform container)
        {
            if (ValidateMutable())
            {
                Container = container;
            }

            return this;
        }

        public UISystem SetRuntimeRunner(TransitionRunner transitionRunner)
        {
            if (ValidateMutable())
            {
                _runtimeRunner = transitionRunner;
                _runtimeRunner.Initialize(ModulesContainer);
            }

            return this;
        }

        public UISystem SetRuntimeRunner<TRunner>() where TRunner : TransitionRunner, new()
        {
            var transitionRunner = new TRunner();
            return SetRuntimeRunner(transitionRunner);
        }

        public UISystem AddOverrideSequence(Sequence sequence)
        {
            if (ValidateMutable())
            {
                OverridenSequences.Add(sequence);
            }

            return this;
        }

        public UISystem AddOverrideSequence(IEnumerable<Sequence> sequence)
        {
            if (ValidateMutable())
            {
                OverridenSequences.AddRange(sequence);
            }

            return this;
        }
    }
}