
using ClinicAPI.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAPI.Shared
{

    /// <summary>
    /// this class used to unifying application api response structure
    /// </summary>
    /// <typeparam name="T">the type of the response result</typeparam>
    public class CustomApiResponse<T> : ICustomApiResponse<T>
    {
        /// <summary>
        /// the result data
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// the result status
        /// </summary>
        public ResultStatusEnum ResultStatus { get; set; }

        /// <summary>
        /// message to show
        /// </summary>
        public string? Message { get; set; }

        public string? CorrelationId { get; set; }


        // Factories
        public static CustomApiResponse<T> Success(T data, string? message = null)
            => new() { ResultStatus = ResultStatusEnum.Ok, Data = data, Message = message };

        public static CustomApiResponse<T> Fail(ResultStatusEnum status, string message, Dictionary<string, string[]>? errors = null)
            => new() { ResultStatus = status, Message = message};

        
    }
}
