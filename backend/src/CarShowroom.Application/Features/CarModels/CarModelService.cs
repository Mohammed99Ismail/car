using CarShowroom.Application.Common.Caching;
using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.CarModels.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace CarShowroom.Application.Features.CarModels;

public class CarModelService(
    IApplicationDbContext context,
    HybridCache cache) : ICarModelService
{
    public async Task<CustomApiResponse<PagedListModel<CarModelDto>>> GetCarModelsAsync(
        CarModelQueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.CarModels.Include(m => m.Make).AsNoTracking();

        if (query.MakeId.HasValue && query.MakeId.Value > 0)
        {
            source = source.Where(m => m.MakeId == query.MakeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(m => m.Name.ToLower().Contains(search) || m.Make.Name.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            m => new CarModelDto(m.Id, m.MakeId, m.Make.Name, m.Name),
            query,
            nameof(CarModel.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<CarModelDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<CarModelDto>> GetCarModelByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var model = await context.CarModels
            .Include(m => m.Make)
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new CarModelDto(m.Id, m.MakeId, m.Make.Name, m.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (model is null)
        {
            return CustomApiResponse<CarModelDto>.NotFound(Msg.NotFound(Msg.CarModel));
        }

        return CustomApiResponse<CarModelDto>.Success(model);
    }

    public async Task<CustomApiResponse<CarModelDto>> CreateCarModelAsync(
        CreateCarModelRequest request, CancellationToken cancellationToken = default)
    {
        var make = await context.CarMakes.FirstOrDefaultAsync(m => m.Id == request.MakeId, cancellationToken);
        if (make is null)
        {
            return CustomApiResponse<CarModelDto>.NotFound(Msg.NotFound(Msg.CarMake));
        }

        var name = request.Name.Trim();
        var exists = await context.CarModels.AnyAsync(
            m => m.MakeId == request.MakeId && m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarModelDto>.Conflict(Msg.AlreadyExists(Msg.CarModel));
        }

        var model = new CarModel
        {
            MakeId = request.MakeId,
            Name = name
        };

        await context.CarModels.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarModels(request.MakeId), cancellationToken);

        var dto = new CarModelDto(model.Id, model.MakeId, make.Name, model.Name);
        return CustomApiResponse<CarModelDto>.Created(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<CarModelDto>> UpdateCarModelAsync(
        int id, UpdateCarModelRequest request, CancellationToken cancellationToken = default)
    {
        var model = await context.CarModels.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (model is null)
        {
            return CustomApiResponse<CarModelDto>.NotFound(Msg.NotFound(Msg.CarModel));
        }

        var make = await context.CarMakes.FirstOrDefaultAsync(m => m.Id == request.MakeId, cancellationToken);
        if (make is null)
        {
            return CustomApiResponse<CarModelDto>.NotFound(Msg.NotFound(Msg.CarMake));
        }

        var name = request.Name.Trim();
        var exists = await context.CarModels.AnyAsync(
            m => m.Id != id && m.MakeId == request.MakeId && m.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CarModelDto>.Conflict(Msg.AlreadyExists(Msg.CarModel));
        }

        var oldMakeId = model.MakeId;
        model.MakeId = request.MakeId;
        model.Name = name;
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarModels(oldMakeId), cancellationToken);
        if (request.MakeId != oldMakeId)
        {
            await cache.RemoveAsync(CacheKey.CarModels(request.MakeId), cancellationToken);
        }

        var dto = new CarModelDto(model.Id, model.MakeId, make.Name, model.Name);
        return CustomApiResponse<CarModelDto>.Success(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteCarModelAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var model = await context.CarModels.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (model is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.CarModel));
        }

        var hasCars = await context.Cars.AnyAsync(c => c.ModelId == id, cancellationToken);
        if (hasCars)
        {
            return CustomApiResponse<Nothing>.Conflict(Msg.InUse(Msg.CarModel));
        }

        var makeId = model.MakeId;
        context.CarModels.Remove(model);
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CacheKey.CarModels(makeId), cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }
}
