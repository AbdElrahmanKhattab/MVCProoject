namespace MVC.Services
{
    public class Result
    {
        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }

        public static Result Ok()
        {
            return new Result(true, null);
        }

        public static Result Fail(string error)
        {
            return new Result(false, error);
        }
    }

    public class Result<T> : Result
    {
        private Result(bool isSuccess, T? value, string? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public T? Value { get; }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(true, value, null);
        }

        public static new Result<T> Fail(string error)
        {
            return new Result<T>(false, default, error);
        }
    }
}
