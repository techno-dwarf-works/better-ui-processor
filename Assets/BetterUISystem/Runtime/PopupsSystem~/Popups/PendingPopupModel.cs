namespace Better.UISystem.Runtime.PopupsSystem.Popups
{
    //TODO: If popup removed from queue - set default result
    public abstract class PendingPopupModel<TResult> : PopupModel
    {
        public TResult DefaultResult { get; }

        protected PendingPopupModel(TResult defaultResult = default)
        {
            DefaultResult = defaultResult;
        }
    }

    public abstract class PendingPopupModel : PendingPopupModel<bool>
    {
    }
}