using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

/// <summary>
/// انواع العقود — readonly without any crud operations
/// </summary>
[Authorize]
[Route("api/contract-types")]
public class ContractTypesController(ILookupService lookupService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupDto>>>> GetContractTypes(
        CancellationToken cancellationToken)
    {
        var result = await lookupService.GetContractTypesAsync(cancellationToken);
        return CustomResult(result);
    }
}
