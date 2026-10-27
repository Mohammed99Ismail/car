using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Authorize]
public class LookupsController(ILookupService lookupService) : BaseApiController
{
    [HttpGet("makes")]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupItemDto>>>> GetMakes(
        [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await lookupService.GetMakesAsync(search, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("makes/{makeId:int}/models")]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupItemDto>>>> GetModels(
        int makeId, [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await lookupService.GetModelsAsync(makeId, search, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("years")]
    public ActionResult<CustomApiResponse<IReadOnlyList<int>>> GetYears()
    {
        var result = lookupService.GetManufactureYears();
        return CustomResult(result);
    }

    [HttpGet("branches")]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupItemDto>>>> GetBranches(
        [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await lookupService.GetBranchesLookupAsync(search, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("colors")]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupColorDto>>>> GetColors(
        [FromQuery] string? search, CancellationToken cancellationToken)
    {
        var result = await lookupService.GetColorsAsync(search, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<LookupDto>>>> GetRoles(
        CancellationToken cancellationToken)
    {
        var result = await lookupService.GetRolesLookupAsync(cancellationToken);
        return CustomResult(result);
    }
}
