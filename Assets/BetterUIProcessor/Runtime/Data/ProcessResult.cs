namespace Better.UIProcessor.Runtime.Data
{
    public class ProcessResult<T>
    {
        public static readonly ProcessResult<T> Unsuccessful = new();

        public bool IsSuccessful { get; }
        public T Data { get; }

        public ProcessResult()
        {
            IsSuccessful = false;
            Data = default;
        }

        public ProcessResult(T data)
        {
            IsSuccessful = true;
            Data = data;
        }
    }
}