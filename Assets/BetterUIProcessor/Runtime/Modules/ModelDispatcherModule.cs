using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;
using Better.UIProcessor.Runtime.Sequences;

namespace Better.UIProcessor.Runtime.Modules
{
    [Serializable]
    public class ModelDispatcherModule : Module
    {
        protected internal override async Task OnPreSequencePlay(UIProcessor processor, Sequence sequence, IElement fromElement, IElement toElement, TransitionInfo transitionInfo)
        {
            if (transitionInfo is IModelEmitter modelTransitionInfo)
            {
                modelTransitionInfo.TryEmitModel(toElement);
            }

            await base.OnPreSequencePlay(processor, sequence, fromElement, toElement, transitionInfo);
        }
    }
}