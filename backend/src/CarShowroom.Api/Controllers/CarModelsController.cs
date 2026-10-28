using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.CarModels;
using CarShowroom.Application.Features.CarModels.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Authorize]
[Route("api/car-models")]
public class CarModelsController(ICarModelService carModelService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<CarModelDto>>>> GetCarModels(
        [FromQuery] CarModelQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await carModelService.GetCarModelsAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomApiResponse<CarModelDto>>> GetCarModelById(
        int id, CancellationToken cancellationToken)
    {
        var result = await carModelService.GetCarModelByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.CarModels.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarModelDto>>> CreateCarModel(
        [FromBody] CreateCarModelRequest request, CancellationToken cancellationToken)
    {
        var result = await carModelService.CreateCarModelAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarModels.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarModelDto>>> UpdateCarModel(
        int id, [FromBody] UpdateCarModelRequest request, CancellationToken cancellationToken)
    {
        var result = await carModelService.UpdateCarModelAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.CarModels.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteCarModel(
        int id, CancellationToken cancellationToken)
    {
        var result = await carModelService.DeleteCarModelAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
