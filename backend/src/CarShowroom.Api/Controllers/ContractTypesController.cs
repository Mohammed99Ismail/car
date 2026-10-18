using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Lookups;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

/// <summary>
/// انواع العقود — readonly without any crud operations
/// </summary>
[Route("api/contract-types")]
public class ContractTypesController(ILookupService lookupService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.ContractTypes.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupDto>>>> GetContractTypes(
        CancellationToken cancellationToken)
    {
        var result = await lookupService.GetContractTypesAsync(cancellationToken);
        return CustomResult(result);
    }
}
