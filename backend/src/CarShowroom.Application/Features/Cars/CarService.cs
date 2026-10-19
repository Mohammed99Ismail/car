using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Cars.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Cars;

public class CarService(
    IApplicationDbContext context,
    IFileStorage fileStorage) : ICarService
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public async Task<CustomApiResponse<PagedListModel<CarDto>>> GetCarsAsync(
        CarQueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Cars
            .Include(c => c.Make)
            .Include(c => c.Model)
            .Include(c => c.Branch)
            .AsNoTracking();

        if (query.BranchId.HasValue)
            source = source.Where(c => c.BranchId == query.BranchId.Value);

        if (query.MakeId.HasValue)
            source = source.Where(c => c.MakeId == query.MakeId.Value);

        if (query.ModelId.HasValue)
            source = source.Where(c => c.ModelId == query.ModelId.Value);

        if (query.ManufactureYear.HasValue)
            source = source.Where(c => c.ManufactureYear == query.ManufactureYear.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(c =>
                c.Make.Name.ToLower().Contains(search) ||
                c.Model.Name.ToLower().Contains(search) ||
                c.PlateNumber.ToLower().Contains(search) ||
                c.ChassisNumber.ToLower().Contains(search) ||
                c.EngineNumber.ToLower().Contains(search) ||
                c.Color.ToLower().Contains(search) ||
                c.Branch.Name.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            c => new CarDto(
                c.Id,
                c.MakeId,
                c.Make.Name,
                c.ModelId,
                c.Model.Name,
                c.ManufactureYear,
                c.Color,
                c.PlateNumber,
                c.BranchId,
                c.Branch.Name,
                c.PurchaseDate,
                c.PurchasePrice,
                c.ImagePath,
                c.ChassisNumber,
                c.EngineNumber,
                c.Notes,
                c.CreatedDate),
            query,
            nameof(Car.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<CarDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<CarDto>> GetCarByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await context.Cars
            .Include(c => c.Make)
            .Include(c => c.Model)
            .Include(c => c.Branch)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car is null)
        {
            return CustomApiResponse<CarDto>.NotFound(Msg.NotFound(Msg.Car));
        }

        var dto = new CarDto(
            car.Id,
            car.MakeId,
            car.Make.Name,
            car.ModelId,
            car.Model.Name,
            car.ManufactureYear,
            car.Color,
            car.PlateNumber,
            car.BranchId,
            car.Branch.Name,
            car.PurchaseDate,
            car.PurchasePrice,
            car.ImagePath,
            car.ChassisNumber,
            car.EngineNumber,
            car.Notes,
            car.CreatedDate);

        return CustomApiResponse<CarDto>.Success(dto);
    }

    public async Task<CustomApiResponse<CarDto>> CreateCarAsync(
        CreateCarRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Verify model belongs to make
        var validModel = await context.CarModels
            .AnyAsync(m => m.Id == request.ModelId && m.MakeId == request.MakeId, cancellationToken);

        if (!validModel)
        {
            return CustomApiResponse<CarDto>.BadRequest(Msg.ModelNotOfMake);
        }

        // 2. Verify branch exists
        var branchExists = await context.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            return CustomApiResponse<CarDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        // 3. Unique checks
        var plate = request.PlateNumber.Trim();
        var chassis = request.ChassisNumber.Trim();
        var engine = request.EngineNumber.Trim();

        if (await context.Cars.AnyAsync(c => c.PlateNumber.ToLower() == plate.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.PlateNumber));

        if (await context.Cars.AnyAsync(c => c.ChassisNumber.ToLower() == chassis.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.ChassisNumber));

        if (await context.Cars.AnyAsync(c => c.EngineNumber.ToLower() == engine.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.EngineNumber));

        var car = new Car
        {
            MakeId = request.MakeId,
            ModelId = request.ModelId,
            ManufactureYear = request.ManufactureYear,
            Color = request.Color.Trim(),
            PlateNumber = plate,
            BranchId = request.BranchId,
            PurchaseDate = request.PurchaseDate.ToUniversalTime(),
            PurchasePrice = request.PurchasePrice,
            ChassisNumber = chassis,
            EngineNumber = engine,
            Notes = request.Notes?.Trim(),
            ImagePath = request.ImagePath
        };

        await context.Cars.AddAsync(car, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await GetCarByIdAsync(car.Id, cancellationToken);
    }

    public async Task<CustomApiResponse<CarDto>> UpdateCarAsync(
        int id, UpdateCarRequest request, CancellationToken cancellationToken = default)
    {
        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (car is null)
        {
            return CustomApiResponse<CarDto>.NotFound(Msg.NotFound(Msg.Car));
        }

        // 1. Verify model belongs to make
        var validModel = await context.CarModels
            .AnyAsync(m => m.Id == request.ModelId && m.MakeId == request.MakeId, cancellationToken);

        if (!validModel)
        {
            return CustomApiResponse<CarDto>.BadRequest(Msg.ModelNotOfMake);
        }

        // 2. Verify branch exists
        var branchExists = await context.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            return CustomApiResponse<CarDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        // 3. Unique checks (excluding current car)
        var plate = request.PlateNumber.Trim();
        var chassis = request.ChassisNumber.Trim();
        var engine = request.EngineNumber.Trim();

        if (await context.Cars.AnyAsync(c => c.Id != id && c.PlateNumber.ToLower() == plate.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.PlateNumber));

        if (await context.Cars.AnyAsync(c => c.Id != id && c.ChassisNumber.ToLower() == chassis.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.ChassisNumber));

        if (await context.Cars.AnyAsync(c => c.Id != id && c.EngineNumber.ToLower() == engine.ToLower(), cancellationToken))
            return CustomApiResponse<CarDto>.Conflict(Msg.AlreadyExists(Msg.EngineNumber));

        car.MakeId = request.MakeId;
        car.ModelId = request.ModelId;
        car.ManufactureYear = request.ManufactureYear;
        car.Color = request.Color.Trim();
        car.PlateNumber = plate;
        car.BranchId = request.BranchId;
        car.PurchaseDate = request.PurchaseDate.ToUniversalTime();
        car.PurchasePrice = request.PurchasePrice;
        car.ChassisNumber = chassis;
        car.EngineNumber = engine;
        car.Notes = request.Notes?.Trim();
        if (request.ImagePath is not null)
        {
            car.ImagePath = request.ImagePath;
        }

        await context.SaveChangesAsync(cancellationToken);

        return await GetCarByIdAsync(car.Id, cancellationToken);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteCarAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (car is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.Car));
        }

        if (!string.IsNullOrWhiteSpace(car.ImagePath))
        {
            fileStorage.Delete(car.ImagePath);
        }

        context.Cars.Remove(car);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }

    public async Task<CustomApiResponse<string>> UploadCarImageAsync(
        int carId, Stream imageStream, string extension, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension))
        {
            return CustomApiResponse<string>.BadRequest(Msg.InvalidImage);
        }

        var car = await context.Cars.FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);
        if (car is null)
        {
            return CustomApiResponse<string>.NotFound(Msg.NotFound(Msg.Car));
        }

        // Delete previous image if exists
        if (!string.IsNullOrWhiteSpace(car.ImagePath))
        {
            fileStorage.Delete(car.ImagePath);
        }

        var webPath = await fileStorage.SaveAsync(imageStream, extension, "cars", cancellationToken);
        car.ImagePath = webPath;
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<string>.Success(webPath, Msg.Saved);
    }

    public async Task<CustomApiResponse<string>> UploadStandaloneImageAsync(
        Stream imageStream, string extension, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension.ToLowerInvariant()))
        {
            return CustomApiResponse<string>.BadRequest(Msg.InvalidImage);
        }

        var webPath = await fileStorage.SaveAsync(imageStream, extension, "cars", cancellationToken);
        return CustomApiResponse<string>.Success(webPath, Msg.Saved);
    }
}
