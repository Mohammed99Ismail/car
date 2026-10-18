using System.Net;

namespace CarShowroom.Application.Common.Models;

public enum ResultStatusEnum
{
    /// <summary>Http Status 200</summary>
    Ok = (int)HttpStatusCode.OK,

    /// <summary>Http Status 201</summary>
    Created = (int)HttpStatusCode.Created,

    /// <summary>Http Status 400</summary>
    BadRequest = (int)HttpStatusCode.BadRequest,

    /// <summary>Http Status 401</summary>
    Unauthorized = (int)HttpStatusCode.Unauthorized,

    /// <summary>Http Status 403</summary>
    Forbidden = (int)HttpStatusCode.Forbidden,

    /// <summary>Http Status 404</summary>
    NotFound = (int)HttpStatusCode.NotFound,

    /// <summary>Http Status 409</summary>
    Conflict = (int)HttpStatusCode.Conflict,

    /// <summary>Http Status 429</summary>
    TooManyRequests = (int)HttpStatusCode.TooManyRequests,

    /// <summary>Http Status 500</summary>
    Error = (int)HttpStatusCode.InternalServerError,
}
