using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.CarColors;
using CarShowroom.Application.Features.CarColors.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Authorize]
[Route("api/car-colors")]
public class CarColorsController(ICarColorService carColorService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<CarColorDto>>>> GetCarColors(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await carColorService.GetCarColorsAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomApiResponse<CarColorDto>>> GetCarColorById(
        int id, CancellationToken cancellationToken)
    {
        var result = await carColorService.GetCarColorByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.CarColors.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarColorDto>>> CreateCarColor(
        [FromBody] CreateCarColorRequest request, CancellationToken cancellationToken)
    {
        var result = await carColorService.CreateCarColorAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarColors.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarColorDto>>> UpdateCarColor(
        int id, [FromBody] UpdateCarColorRequest request, CancellationToken cancellationToken)
    {
        var result = await carColorService.UpdateCarColorAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarColors.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteCarColor(
        int id, CancellationToken cancellationToken)
    {
        var result = await carColorService.DeleteCarColorAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
