using CarShowroom.Application.Common.Caching;
using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarColors.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace CarShowroom.Application.Features.CarColors;

public class CarColorService(
    IApplicationDbContext context,
    HybridCache cache) : ICarColorService
{
    public async Task<CustomApiResponse<PagedListModel<CarColorDto>>> GetCarColorsAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.CarColors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(c => c.Name.ToLower().Contains(search) || (c.HexCode != null && c.HexCode.ToLower().Contains(search)));
        }

        var paged = await source.ToPagedListAsync(
            c => new CarColorDto(c.Id, c.Name, c.HexCode, context.Cars.Count(car => car.ColorId == c.Id)),
            query,
            nameof(CarColor.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<CarColorDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<CarColorDto>> GetCarColorByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var color = await context.CarColors
            .AsNoTracking()
            .Select(c => new CarColorDto(c.Id, c.Name, c.HexCode, context.Cars.Count(car => car.ColorId == c.Id)))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (color is null)
        {
            return CustomApiResponse<CarColorDto>.NotFound("لون السيارة غير موجود");
        }

        return CustomApiResponse<CarColorDto>.Success(color);
    }

    public async Task<CustomApiResponse<CarColorDto>> CreateCarColorAsync(
        CreateCarColorRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var exists = await context.CarColors.AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarColorDto>.Conflict("لون السيارة مسجل مسبقاً في النظام");
        }

        var color = new CarColor
        {
            Name = name,
            HexCode = request.HexCode?.Trim()
        };

        await context.CarColors.AddAsync(color, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarColors, cancellationToken);

        return CustomApiResponse<CarColorDto>.Created(new CarColorDto(color.Id, color.Name, color.HexCode, 0), Msg.Saved);
    }

    public async Task<CustomApiResponse<CarColorDto>> UpdateCarColorAsync(
        int id, UpdateCarColorRequest request, CancellationToken cancellationToken = default)
    {
        var color = await context.CarColors.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (color is null)
        {
            return CustomApiResponse<CarColorDto>.NotFound("لون السيارة غير موجود");
        }

        var name = request.Name.Trim();
        var exists = await context.CarColors.AnyAsync(
            c => c.Id != id && c.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarColorDto>.Conflict("لون السيارة مسجل مسبقاً في النظام");
        }

        color.Name = name;
        color.HexCode = request.HexCode?.Trim();
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarColors, cancellationToken);

        var count = await context.Cars.CountAsync(car => car.ColorId == id, cancellationToken);
        return CustomApiResponse<CarColorDto>.Success(new CarColorDto(color.Id, color.Name, color.HexCode, count), Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteCarColorAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var color = await context.CarColors.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (color is null)
        {
            return CustomApiResponse<Nothing>.NotFound("لون السيارة غير موجود");
        }

        var hasCars = await context.Cars.AnyAsync(c => c.ColorId == id, cancellationToken);
        if (hasCars)
        {
            return CustomApiResponse<Nothing>.Conflict("لا يمكن حذف لون السيارة لوجود سيارات مسجلة بهذا اللون في المعرض");
        }

        context.CarColors.Remove(color);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarColors, cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }
}
