using System.Threading.Tasks;

namespace Better.UIComposite.Runtime.PopupsSystem.Popups
{
    public abstract class PendingPopup<TResult, TModel, TView> : Popup<TModel, TView>
        where TModel : PendingPopupModel<TResult>
        where TView : PendingPopupView
    {
        private readonly TaskCompletionSource<TResult> _resultCompletionSource = new();
        private readonly TaskCompletionSource<bool> _closeCompilationSource = new();

        protected override Task OnHideAsync()
        {
            SetDefaultResult();
            return Task.CompletedTask;
        }

        private void OnDestroy()
        {
            Close();
        }

        protected override void Close()
        {
            _closeCompilationSource.TrySetResult(true);
            _resultCompletionSource.TrySetCanceled();
        }

        protected void CloseWithResult(TResult value)
        {
            SetResult(value);
            Close();
        }

        protected void SetResult(TResult value)
        {
            _resultCompletionSource.TrySetResult(value);
        }

        protected void SetDefaultResult()
        {
            SetResult(Model.DefaultResult);
        }

        public Task<TResult> AwaitResult()
        {
            return _resultCompletionSource.Task;
        }

        public Task AwaitClose()
        {
            return _closeCompilationSource.Task;
        }
    }

    public abstract class PendingPopup<TModel, TView> : PendingPopup<bool, TModel, TView>
        where TModel : PendingPopupModel
        where TView : PendingPopupView
    {
    }
}