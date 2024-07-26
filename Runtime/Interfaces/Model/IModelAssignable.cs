namespace Better.UIProcessor.Runtime.Interfaces
{
    public interface IModelAssignable<TModel>
        where TModel : IModel
    {
        public void AssignModel(TModel model);
    }
}