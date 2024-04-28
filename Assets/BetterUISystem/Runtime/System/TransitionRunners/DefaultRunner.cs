using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UISystem.Runtime.Common;
using Better.UISystem.Runtime.Elements;
using Better.UISystem.Runtime.Interfaces;
using Better.UISystem.Runtime.TransitionInfos;

namespace Better.UISystem.Runtime.TransitionRunners
{
    public class DefaultRunner : TransitionRunner
    {
        public override async Task<Result<ISystemElement>> RunAsync(ISystemElement element, TransitionInfo info)
        {
            await ModulesContainer.RunStarted(info);
            
            if (!info.IsRelevant())
            {
                return Result<ISystemElement>.GetUnsuccessful();
            }
            
            var elementResult = await ModulesContainer.TryHandleOpen(info);
            
            if (!elementResult.IsSuccessful)
            {
                //TODO: Log
                await ModulesContainer.RunFailed(info);
                return Result<ISystemElement>.GetUnsuccessful();
            }
            
            if (!info.IsRelevant())
            {
                return Result<ISystemElement>.GetUnsuccessful();
            }

            await ModulesContainer.OpenHandled(elementResult.Data, info);

            var hasOpenedScreen = element != null;
            var openedElement = elementResult.Data;

            var sequenceResult = await ModulesContainer.TryGetTransitionSequence(info);

            if (!sequenceResult.IsSuccessful)
            {
                //TODO: Log
                await ModulesContainer.RunFailed(info);
                return Result<ISystemElement>.GetUnsuccessful();
            }

            var sequence = sequenceResult.Data;

            await ModulesContainer.BeforeSequencePlay(openedElement, info);
            if (hasOpenedScreen)
            {
                await sequence.DoPlay(element, openedElement);
            }
            else
            {
                await sequence.DoPlay(openedElement);
            }

            await ModulesContainer.AfterSequencePlay(openedElement, info);
            
            await ModulesContainer.ElementOpened(openedElement, info);
            
            if (hasOpenedScreen)
            {
                var result = await ModulesContainer.TryHandleClose(element, info);

                if (!result)
                {
                    element.RectTransform.DestroyGameObject();
                }

                await ModulesContainer.ElementClosed(info);
            }

            await ModulesContainer.RunCompleted(openedElement, info);
            return new Result<ISystemElement>(openedElement);
        }
    }
}