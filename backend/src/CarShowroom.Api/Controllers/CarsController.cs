using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Cars;
using CarShowroom.Application.Features.Cars.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class CarsController(ICarService carService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Cars.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<CarDto>>>> GetCars(
        [FromQuery] CarQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await carService.GetCarsAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Cars.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarDto>>> GetCarById(
        int id, CancellationToken cancellationToken)
    {
        var result = await carService.GetCarByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Cars.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarDto>>> CreateCar(
        [FromBody] CreateCarRequest request, CancellationToken cancellationToken)
    {
        var result = await carService.CreateCarAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Cars.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CarDto>>> UpdateCar(
        int id, [FromBody] UpdateCarRequest request, CancellationToken cancellationToken)
    {
        var result = await carService.UpdateCarAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Cars.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteCar(
        int id, CancellationToken cancellationToken)
    {
        var result = await carService.DeleteCarAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("{id:int}/image")]
    [PermissionAuthorize(PermissionKeysConstant.Cars.UPDATE_KEY)]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB max
    public async Task<ActionResult<CustomApiResponse<string>>> UploadImage(
        int id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return CustomResult(CustomApiResponse<string>.BadRequest(Msg.InvalidImage));
        }

        var ext = Path.GetExtension(file.FileName);
        await using var stream = file.OpenReadStream();
        var result = await carService.UploadCarImageAsync(id, stream, ext, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("upload-image")]
    [PermissionAuthorize(PermissionKeysConstant.Cars.CREATE_KEY)]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB max
    public async Task<ActionResult<CustomApiResponse<string>>> UploadImageStandalone(
        IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return CustomResult(CustomApiResponse<string>.BadRequest(Msg.InvalidImage));
        }

        var ext = Path.GetExtension(file.FileName);
        await using var stream = file.OpenReadStream();
        var result = await carService.UploadStandaloneImageAsync(stream, ext, cancellationToken);
        return CustomResult(result);
    }
}
