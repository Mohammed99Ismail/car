using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Cars.Dtos;

namespace CarShowroom.Application.Features.Cars;

public interface ICarService
{
    Task<CustomApiResponse<PagedListModel<CarDto>>> GetCarsAsync(CarQueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarDto>> GetCarByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarDto>> CreateCarAsync(CreateCarRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CarDto>> UpdateCarAsync(int id, UpdateCarRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteCarAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<string>> UploadCarImageAsync(int carId, Stream imageStream, string extension, CancellationToken cancellationToken = default);
}
