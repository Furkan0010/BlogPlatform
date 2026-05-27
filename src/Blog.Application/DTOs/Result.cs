namespace Blog.Application.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string[] Errors { get; }

    private Result(bool isSuccess, T? value, string[] errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());
    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
}