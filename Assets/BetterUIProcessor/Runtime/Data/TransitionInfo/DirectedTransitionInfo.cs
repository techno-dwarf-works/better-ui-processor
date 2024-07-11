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
            if (successful && Processor.TryGetOpenedElement<TElement>(out var element))
            {
                return new ProcessResult<TElement>(element);
            }

            return ProcessResult<TElement>.Unsuccessful;
        }
    }

    public class DirectedTransitionInfo<TElement, TModel> : DirectedTransitionInfo<TElement>, IModelEmitter
        where TElement : IElement, IModelAssignable<TModel>
        where TModel : IModel
    {
        private IModelEmitter _modelAdapterImplementation;
        public TModel Model { get; }

        public DirectedTransitionInfo(UIProcessor processor, TModel model, CancellationToken cancellationToken = default)
            : base(processor, cancellationToken)
        {
            Model = model;
        }

        bool IModelEmitter.TryEmitModel(object attractor)
        {
            if (attractor == null)
            {
                return false;
            }

            if (attractor is TElement assignableAttractor)
            {
                assignableAttractor.AssignModel(Model);
                return true;
            }

            return false;
        }
    }
}