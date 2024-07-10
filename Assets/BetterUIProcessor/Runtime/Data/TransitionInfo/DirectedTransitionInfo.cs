using System;
using System.Threading;
using System.Threading.Tasks;
using Better.UIProcessor.Runtime.Extensions;
using Better.UIProcessor.Runtime.Interfaces;

namespace Better.UIProcessor.Runtime.Data
{
    public class DirectedTransitionInfo<TElement> : TransitionInfo
        where TElement : IElement
    {
        public Type ElementType { get; }

        public DirectedTransitionInfo(UIProcessor processor, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
            ElementType = typeof(TElement);
        }

        public new DirectedTransitionInfo<TElement> Run()
        {
            base.Run();
            return this;
        }

        public new async Task<ProcessResult<TElement>> Await()
        {
            var successful = await base.Await();
            if (successful && Processor.TryGetOpened<TElement>(out var element))
            {
                return new ProcessResult<TElement>(element);
            }

            return ProcessResult<TElement>.Unsuccessful;
        }
    }

    public class DirectedTransitionInfo<TElement, TModel> : DirectedTransitionInfo<TElement>
        where TElement : IElement<TModel>
        where TModel : IModel
    {
        public TModel Model { get; }

        public DirectedTransitionInfo(UIProcessor processor, TModel model, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
            Model = model;
        }
    }
}