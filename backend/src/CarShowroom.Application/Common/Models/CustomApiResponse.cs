namespace CarShowroom.Application.Common.Models;

public interface ICustomApiResponse<T>
{
    /// <summary>the result status</summary>
    ResultStatusEnum ResultStatus { get; set; }

    /// <summary>message to show</summary>
    string? Message { get; set; }

    /// <summary>the result data</summary>
    T? Data { get; set; }
}

/// <summary>
/// Unifies the application API response structure (adapted from Shared/CustomAPIResponse).
/// </summary>
/// <typeparam name="T">the type of the response result</typeparam>
public class CustomApiResponse<T> : ICustomApiResponse<T>
{
    /// <summary>the result data</summary>
    public T? Data { get; set; }

    /// <summary>the result status</summary>
    public ResultStatusEnum ResultStatus { get; set; }

    /// <summary>message to show</summary>
    public string? Message { get; set; }

    /// <summary>validation errors keyed by field name</summary>
    public IDictionary<string, string[]>? Errors { get; set; }

    public string? CorrelationId { get; set; }

    public bool IsSuccess => ResultStatus is ResultStatusEnum.Ok or ResultStatusEnum.Created;

    // Factories
    public static CustomApiResponse<T> Success(T data, string? message = null)
        => new() { ResultStatus = ResultStatusEnum.Ok, Data = data, Message = message };

    public static CustomApiResponse<T> Created(T data, string? message = null)
        => new() { ResultStatus = ResultStatusEnum.Created, Data = data, Message = message };

    public static CustomApiResponse<T> Fail(ResultStatusEnum status, string message, IDictionary<string, string[]>? errors = null)
        => new() { ResultStatus = status, Message = message, Errors = errors };

    public static CustomApiResponse<T> NotFound(string message) => Fail(ResultStatusEnum.NotFound, message);
    public static CustomApiResponse<T> Conflict(string message) => Fail(ResultStatusEnum.Conflict, message);
    public static CustomApiResponse<T> BadRequest(string message, IDictionary<string, string[]>? errors = null)
        => Fail(ResultStatusEnum.BadRequest, message, errors);
    public static CustomApiResponse<T> Unauthorized(string message) => Fail(ResultStatusEnum.Unauthorized, message);
}

/// <summary>Empty payload for responses without data (adapted from Shared/Model/Nothing).</summary>
public sealed class Nothing
{
    public static readonly Nothing Value = new();
}
