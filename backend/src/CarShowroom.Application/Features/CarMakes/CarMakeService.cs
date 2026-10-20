using CarShowroom.Application.Common.Caching;
using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarMakes.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace CarShowroom.Application.Features.CarMakes;

public class CarMakeService(
    IApplicationDbContext context,
    HybridCache cache) : ICarMakeService
{
    public async Task<CustomApiResponse<PagedListModel<CarMakeDto>>> GetCarMakesAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.CarMakes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(m => m.Name.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            m => new CarMakeDto(m.Id, m.Name, m.Models.Count),
            query,
            nameof(CarMake.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<CarMakeDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<CarMakeDto>> GetCarMakeByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var make = await context.CarMakes
            .AsNoTracking()
            .Select(m => new CarMakeDto(m.Id, m.Name, m.Models.Count))
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (make is null)
        {
            return CustomApiResponse<CarMakeDto>.NotFound(Msg.NotFound(Msg.CarMake));
        }

        return CustomApiResponse<CarMakeDto>.Success(make);
    }

    public async Task<CustomApiResponse<CarMakeDto>> CreateCarMakeAsync(
        CreateCarMakeRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var exists = await context.CarMakes.AnyAsync(m => m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarMakeDto>.Conflict(Msg.AlreadyExists(Msg.CarMake));
        }

        var make = new CarMake { Name = name };
        await context.CarMakes.AddAsync(make, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarMakes, cancellationToken);

        return CustomApiResponse<CarMakeDto>.Created(new CarMakeDto(make.Id, make.Name, 0), Msg.Saved);
    }

    public async Task<CustomApiResponse<CarMakeDto>> UpdateCarMakeAsync(
        int id, UpdateCarMakeRequest request, CancellationToken cancellationToken = default)
    {
        var make = await context.CarMakes.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (make is null)
        {
            return CustomApiResponse<CarMakeDto>.NotFound(Msg.NotFound(Msg.CarMake));
        }

        var name = request.Name.Trim();
        var exists = await context.CarMakes.AnyAsync(
            m => m.Id != id && m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarMakeDto>.Conflict(Msg.AlreadyExists(Msg.CarMake));
        }

        make.Name = name;
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarMakes, cancellationToken);

        var count = await context.CarModels.CountAsync(mo => mo.MakeId == id, cancellationToken);
        return CustomApiResponse<CarMakeDto>.Success(new CarMakeDto(make.Id, make.Name, count), Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteCarMakeAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var make = await context.CarMakes.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (make is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.CarMake));
        }

        var hasCars = await context.Cars.AnyAsync(c => c.MakeId == id, cancellationToken);
        if (hasCars)
        {
            return CustomApiResponse<Nothing>.Conflict(Msg.InUse(Msg.CarMake));
        }

        var hasModels = await context.CarModels.AnyAsync(mo => mo.MakeId == id, cancellationToken);
        if (hasModels)
        {
            return CustomApiResponse<Nothing>.Conflict("لا يمكن حذف نوع السيارة لوجود موديلات مرتبطة به، يرجى حذف الموديلات أولاً");
        }

        context.CarMakes.Remove(make);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarMakes, cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }
}
