using System.Net;

namespace ClinicAPI.Shared.Enums
{
    public enum ResultStatusEnum
    {
        /// <summary>
        /// Http Status 200
        /// </summary>
        Ok = (int)HttpStatusCode.OK, //200,

        /// <summary>
        /// Http Status 500
        /// </summary>
        Error = (int)HttpStatusCode.InternalServerError, //500,

        /// <summary>
        /// Http Status 404
        /// </summary>
        NotFound = (int)HttpStatusCode.NotFound, //404,

        /// <summary>
        /// Http Status 401
        /// </summary>
        Unauthorized = (int)HttpStatusCode.Unauthorized, //401,


        /// <summary>
        /// Http Status 400
        /// </summary>
        BadRequest = (int)HttpStatusCode.BadRequest, //400

        /// <summary>
        /// Http Status 400
        /// </summary>
        Conflict = (int)HttpStatusCode.Conflict, //409
    }
}
