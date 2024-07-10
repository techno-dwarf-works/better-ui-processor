namespace Better.UIProcessor.Runtime.Data
{
    public class ProcessResult<T>
    {
        public static readonly ProcessResult<T> Unsuccessful = new();

        public bool IsSuccessful { get; }
        public T Result { get; } // TODO: Update usage

        public ProcessResult()
        {
            IsSuccessful = false;
            Result = default;
        }

        public ProcessResult(T result)
        {
            IsSuccessful = true;
            Result = result;
        }
    }
}