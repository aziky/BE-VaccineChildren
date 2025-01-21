namespace VaccineChildren.Core.Base;

public class BaseResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static BaseResult<T> Ok(T data)
    {
        return new BaseResult<T> { Success = true, Data = data };
    }

    public static BaseResult<T> Fail(string message)
    {
        return new BaseResult<T> { Success = false, ErrorMessage = message };
    }
}