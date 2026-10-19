
using ClinicAPI.Shared.Enums;

namespace ClinicAPI.Shared
{
    public  interface ICustomApiResponse<T>
    {
        /// <summary>
        /// the result status
        /// </summary>
        public ResultStatusEnum ResultStatus { get; set; }
        /// <summary>
        /// message to show
        /// </summary>
        public string? Message { get; set; }
        /// <summary>
        /// the result data
        /// </summary>
        public T? Data { get; set; }
    }
}
