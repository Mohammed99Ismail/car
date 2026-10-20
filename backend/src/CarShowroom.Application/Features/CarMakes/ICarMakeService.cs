using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarMakes.Dtos;

namespace CarShowroom.Application.Features.CarMakes;

public interface ICarMakeService
{
    Task<CustomApiResponse<PagedListModel<CarMakeDto>>> GetCarMakesAsync(QueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarMakeDto>> GetCarMakeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarMakeDto>> CreateCarMakeAsync(CreateCarMakeRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarMakeDto>> UpdateCarMakeAsync(int id, UpdateCarMakeRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteCarMakeAsync(int id, CancellationToken cancellationToken = default);
}
