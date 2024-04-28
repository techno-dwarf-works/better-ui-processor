using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionInfos;

namespace Better.UISystem.Runtime.Modules
{
    public class ModulesContainer
    {
        protected Dictionary<Type, SystemModule> Modules { get; private set; }

        public ModulesContainer(IEnumerable<SystemModule> modules)
        {
            foreach (var module in modules)
            {
                TryAddModule(module);
            }
        }

        public async Task<Result<ISystemElement>> TryHandleOpen(TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                var result = await module.TryHandleOpen(info);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return Result<ISystemElement>.GetUnsuccessful();
        }

        public async Task<bool> TryHandleClose(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                var result = await module.TryHandleClose(element, info);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task ElementClosed(TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.ElementClosed(info);
            }
        }

        public async Task<Result<Sequence>> TryGetTransitionSequence(TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                var result = await module.TryGetTransitionSequence(info);
                if (result.IsSuccessful)
                {
                    return result;
                }
            }

            return Result<Sequence>.GetUnsuccessful();
        }

        public async Task ElementOpened(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.ElementOpened(element, info);
            }
        }

        public async Task RunStarted(TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.RunStarted(info);
            }
        }

        public async Task RunFailed(TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.RunFailed(info);
            }
        }

        public async Task OpenHandled(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.OpenHandled(element, info);
            }
        }

        public async Task BeforeSequencePlay(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.BeforeSequencePlay(element, info);
            }
        }

        public async Task AfterSequencePlay(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.AfterSequencePlay(element, info);
            }
        }

        public async Task RunCompleted(ISystemElement element, TransitionInfo info)
        {
            foreach (var module in Modules.Values)
            {
                await module.RunCompleted(element, info);
            }
        }

        internal bool TryAddModule(SystemModule module)
        {
            return Modules.TryAdd(module.GetType(), module);
        }

        internal bool TryGetModule(Type type, out SystemModule module)
        {
            return Modules.TryGetValue(type, out module);
        }

        public void Deconstruct()
        {
            foreach (var module in Modules.Values)
            {
                module.Deconstruct();
            }
        }
    }
}