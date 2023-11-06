namespace BackEnd.Models.Response;

public class ResponseResult
{
    public bool IsSuccess { get; set; }
    public required string Message { get; set; }
    public required object? Data { get; set; }
}
