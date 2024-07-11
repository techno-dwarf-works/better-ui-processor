using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class CloseElementModule : Module<CloseTransitionInfo>
    {
        protected override async Task<ProcessResult<IElement>> TryGetTransitionElement(UIProcessor processor, CloseTransitionInfo transitionInfo)
        {
            var result = await base.TryGetTransitionElement(processor, transitionInfo);
            if (result.IsSuccessful)
            {
                return result;
            }

            return new ProcessResult<IElement>(default);
        }
    }
}