namespace syncora_server.Utils;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; } = default(T)!;

    public static ApiResponse<T> Create(int statusCode, bool success, string? message, T data)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = success,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> SuccessResponse(int statusCode, string? message, T? data)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> FailedResponse(int statusCode, string? message)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Success = false,
            Message = message
        };
    }
}