namespace Better.UISystem.Runtime.Common
{
    public class Result<T>
    {
        private static readonly Result<T> Unsuccessful = new Result<T>();
        
        public bool IsSuccessful { get; }
        public T Data { get; }

        public Result()
        {
            IsSuccessful = false;
            Data = default;
        }

        public Result(T data)
        {
            IsSuccessful = false;
            Data = data;
        }
        
        public static Result<T> GetUnsuccessful()
        {
            return Unsuccessful;
        }
    }
}