using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarModels.Dtos;

namespace CarShowroom.Application.Features.CarModels;

public interface ICarModelService
{
    Task<CustomApiResponse<PagedListModel<CarModelDto>>> GetCarModelsAsync(CarModelQueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarModelDto>> GetCarModelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarModelDto>> CreateCarModelAsync(CreateCarModelRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarModelDto>> UpdateCarModelAsync(int id, UpdateCarModelRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteCarModelAsync(int id, CancellationToken cancellationToken = default);
}
