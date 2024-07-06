using System;
using System.Threading;
using System.Threading.Tasks;
using Better.Commons.Runtime.Extensions;
using Better.UIComposite.Runtime.Elements;
using Better.UIComposite.Runtime.Interfaces;
using Better.UIComposite.Runtime.TransitionInfos;
using Better.UIComposite.Runtime.TransitionRunners;

namespace Better.UIComposite.Runtime.Modules.OpenElements
{
    public abstract class OpenTransitionInfo : TransitionInfo
    {
        public Type PresenterType { get; }
        
        protected ITransitionRunner Runner { get; }

        public ElementModel DerivedModel { get; }

        protected OpenTransitionInfo(ITransitionRunner runner, ElementModel derivedModel, Type presenterType, CancellationToken cancellationToken) 
            : base(cancellationToken)
        {
            PresenterType = presenterType;
            Runner = runner;
            DerivedModel = derivedModel;
        }
    }
    
    public class OpenTransitionInfo<TPresenter, TModel> : OpenTransitionInfo
        where TPresenter : SystemElement<TModel>
        where TModel : ElementModel
    {
        public OpenTransitionInfo(ITransitionRunner runner, TModel model, CancellationToken cancellationToken)
            : base(runner, model, typeof(TPresenter), cancellationToken)
        {
        }

        public async Task<TPresenter> RunAsync()
        {
            ValidateRun();

            return (TPresenter) await Runner.RunAsync(this);
        }

        public void Run()
        {
            RunAsync().Forget();
        }
    }
}