using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.CarMakes;
using CarShowroom.Application.Features.CarMakes.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Route("api/car-makes")]
public class CarMakesController(ICarMakeService carMakeService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.CarMakes.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<CarMakeDto>>>> GetCarMakes(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await carMakeService.GetCarMakesAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarMakes.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarMakeDto>>> GetCarMakeById(
        int id, CancellationToken cancellationToken)
    {
        var result = await carMakeService.GetCarMakeByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.CarMakes.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarMakeDto>>> CreateCarMake(
        [FromBody] CreateCarMakeRequest request, CancellationToken cancellationToken)
    {
        var result = await carMakeService.CreateCarMakeAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarMakes.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarMakeDto>>> UpdateCarMake(
        int id, [FromBody] UpdateCarMakeRequest request, CancellationToken cancellationToken)
    {
        var result = await carMakeService.UpdateCarMakeAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarMakes.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteCarMake(
        int id, CancellationToken cancellationToken)
    {
        var result = await carMakeService.DeleteCarMakeAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
