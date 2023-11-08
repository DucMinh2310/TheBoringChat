namespace BackEnd.Models.Response;

public class ResponseResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public ResponseResult() { }
    public ResponseResult(bool isSuccess, string message, object? data)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }

    public static ResponseResult Success<T>(T data) => new(true, "Thành công", data);
    public static ResponseResult Success() => new(true, "Thành công", null);
    public static ResponseResult Failure() => new(false, "Thất bại", null);
    public static ResponseResult Failure(string message) => new(false, message, null);
}