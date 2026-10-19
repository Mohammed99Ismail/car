using ClinicAPI.Shared;
using ClinicAPI.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace MadinahInvestment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// This extension method returns the appropiate Action result type like OkResult, BadRequestResult, NotFoundResult, UnAuthorizedResult
        /// </summary>
        /// <typeparam name="T">this is auto detected from the type of CustomApiResponse</typeparam>
        /// <param name="response">the response object of type CustomApiResponse</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public ActionResult<CustomApiResponse<T>> ActionResult<T>(CustomApiResponse<T> response)
        {
            if (response != null && (int)response.ResultStatus != (int)ResultStatusEnum.Ok/* && response.CorrelationId.IsNullOrWhiteSpace()*/)
            {
                //TODO: inject here?
                //response.CorrelationId = ;
            }

            var result = new ObjectResult(response) { StatusCode = (int)(response?.ResultStatus ?? ResultStatusEnum.BadRequest) };
            return result;

        }

    }
}
