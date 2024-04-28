using System.Threading.Tasks;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.Modules;
using Better.UISystem.Runtime.TransitionInfos;

namespace Better.UISystem.Runtime.TransitionRunners
{
    public abstract class TransitionRunner
    {
        protected ModulesContainer ModulesContainer { get; private set; }
        
        public virtual void Initialize(ModulesContainer modulesContainer)
        {
            ModulesContainer = modulesContainer;
        }
        
        public abstract Task<Result<ISystemElement>> RunAsync(ISystemElement element, TransitionInfo info);
    }
}