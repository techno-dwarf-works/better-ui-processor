using System;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Data;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.TransitionRunners
{
    [Serializable]
    public class DefaultTransitionRunner : TransitionRunner
    {
        public override async Task<Result<IElement>> RunAsync(IUISystem system, IElement element, TransitionInfo info, CancellationToken cancellationToken)
        {
            await ModulesContainer.RunStarted(info);

            if (!info.IsRelevant())
            {
                return Result<IElement>.Unsuccessful();
            }

            var elementResult = await ModulesContainer.TryHandleOpen(info);

            if (!elementResult.IsSuccessful)
            {
                //TODO: Log
                await ModulesContainer.RunFailed(info);
                return Result<IElement>.Unsuccessful();
            }

            if (!info.IsRelevant())
            {
                return Result<IElement>.Unsuccessful();
            }

            await ModulesContainer.OpenHandled(elementResult.Data, info);

            var hasOpenedScreen = element != null;
            var openedElement = elementResult.Data;

            var sequenceResult = await ModulesContainer.TryGetTransitionSequence(info);

            if (!sequenceResult.IsSuccessful)
            {
                //TODO: Log
                await ModulesContainer.RunFailed(info);
                return Result<IElement>.Unsuccessful();
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
            return new Result<IElement>(openedElement);
        }
    }
}