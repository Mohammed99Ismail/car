using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarColors.Dtos;

namespace CarShowroom.Application.Features.CarColors;

public interface ICarColorService
{
    Task<CustomApiResponse<PagedListModel<CarColorDto>>> GetCarColorsAsync(
        QueryParameters query, CancellationToken cancellationToken = default);

    Task<CustomApiResponse<CarColorDto>> GetCarColorByIdAsync(
        int id, CancellationToken cancellationToken = default);

    Task<CustomApiResponse<CarColorDto>> CreateCarColorAsync(
        CreateCarColorRequest request, CancellationToken cancellationToken = default);

    Task<CustomApiResponse<CarColorDto>> UpdateCarColorAsync(
        int id, UpdateCarColorRequest request, CancellationToken cancellationToken = default);

    Task<CustomApiResponse<Nothing>> DeleteCarColorAsync(
        int id, CancellationToken cancellationToken = default);
}
