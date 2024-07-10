using System.Threading;
using System.Threading.Tasks;

namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IElement : ISequencable
    {
        public Task InitializeAsync(CancellationToken cancellationToken);
    }

    public interface IElement<TModel> : IElement
        where TModel : IModel
    {
        public void SetModel(TModel model);
    }
}