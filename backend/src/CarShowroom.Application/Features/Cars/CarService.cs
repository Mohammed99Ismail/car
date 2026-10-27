using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Cars.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using CarShowroom.Application.Features.Contracts.Services;

namespace CarShowroom.Application.Features.Cars;

public class CarService(
    IApplicationDbContext context,
    IFileStorage fileStorage,
    IContractCalculationService calculationService) : ICarService
{
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private CarDto MapCarToDto(Car c)
    {
        var (calculatedBookValue, accumulatedDepr, productiveAge) = calculationService.CalculateCarBookValue(
            c.PurchasePrice, c.PurchaseDate);

        var effectiveBookValue = c.BookValue > 0 ? c.BookValue : calculatedBookValue;

        return new CarDto(
            c.Id,
            c.MakeId,
            c.Make?.Name ?? string.Empty,
            c.ModelId,
            c.Model?.Name ?? string.Empty,
            c.ManufactureYear,
            c.CarColor?.Name ?? c.Color,
            c.PlateNumber,
            c.BranchId,
            c.Branch?.Name ?? string.Empty,
            c.PurchaseDate,
            c.PurchasePrice,
            effectiveBookValue,
            accumulatedDepr,
            productiveAge,
            c.ImagePath,
            c.ChassisNumber,
            c.EngineNumber,
            c.Notes,
            c.CreatedDate,
            c.ColorId,
            c.CarColor?.HexCode);
    }

    public async Task<CustomApiResponse<PagedListModel<CarDto>>> GetCarsAsync(
        CarQueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Cars
            .Include(c => c.Make)
            .Include(c => c.Model)
            .Include(c => c.Branch)
            .Include(c => c.CarColor)
            .AsNoTracking();

        if (query.BranchId.HasValue)
            source = source.Where(c => c.BranchId == query.BranchId.Value);

        if (query.MakeId.HasValue)
            source = source.Where(c => c.MakeId == query.MakeId.Value);

        if (query.ModelId.HasValue)
            source = source.Where(c => c.ModelId == query.ModelId.Value);

        if (query.ManufactureYear.HasValue)
            source = source.Where(c => c.ManufactureYear == query.ManufactureYear.Value);

        if (query.ColorId.HasValue)
            source = source.Where(c => c.ColorId == query.ColorId.Value);

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
                (c.CarColor != null && c.CarColor.Name.ToLower().Contains(search)) ||
                c.Branch.Name.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            query,
            nameof(Car.Id),
            cancellationToken);

        var dtos = paged.Data.Select(MapCarToDto).ToList();

        return CustomApiResponse<PagedListModel<CarDto>>.Success(new PagedListModel<CarDto>
        {
            Data = dtos,
            Length = paged.Length,
            Page = paged.Page,
            Limit = paged.Limit
        });
    }

    public async Task<CustomApiResponse<CarDto>> GetCarByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var car = await context.Cars
            .Include(c => c.Make)
            .Include(c => c.Model)
            .Include(c => c.Branch)
            .Include(c => c.CarColor)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (car is null)
        {
            return CustomApiResponse<CarDto>.NotFound(Msg.NotFound(Msg.Car));
        }

        return CustomApiResponse<CarDto>.Success(MapCarToDto(car));
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

        var (calcBookValue, _, _) = calculationService.CalculateCarBookValue(request.PurchasePrice, request.PurchaseDate);

        var colorId = request.ColorId;
        var colorName = request.Color?.Trim() ?? string.Empty;

        if (colorId.HasValue)
        {
            var colorEntity = await context.CarColors.FindAsync([colorId.Value], cancellationToken);
            if (colorEntity != null && string.IsNullOrWhiteSpace(colorName))
            {
                colorName = colorEntity.Name;
            }
        }
        else if (!string.IsNullOrWhiteSpace(colorName))
        {
            var existingColor = await context.CarColors.FirstOrDefaultAsync(c => c.Name.ToLower() == colorName.ToLower(), cancellationToken);
            if (existingColor != null)
            {
                colorId = existingColor.Id;
            }
        }

        var car = new Car
        {
            MakeId = request.MakeId,
            ModelId = request.ModelId,
            ManufactureYear = request.ManufactureYear,
            Color = colorName,
            ColorId = colorId,
            PlateNumber = plate,
            BranchId = request.BranchId,
            PurchaseDate = request.PurchaseDate.ToUniversalTime(),
            PurchasePrice = request.PurchasePrice,
            BookValue = (request.BookValue.HasValue && request.BookValue.Value > 0) ? request.BookValue.Value : calcBookValue,
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

        var updateColorId = request.ColorId;
        var updateColorName = request.Color?.Trim() ?? string.Empty;

        if (updateColorId.HasValue)
        {
            var colorEntity = await context.CarColors.FindAsync([updateColorId.Value], cancellationToken);
            if (colorEntity != null && string.IsNullOrWhiteSpace(updateColorName))
            {
                updateColorName = colorEntity.Name;
            }
        }
        else if (!string.IsNullOrWhiteSpace(updateColorName))
        {
            var existingColor = await context.CarColors.FirstOrDefaultAsync(c => c.Name.ToLower() == updateColorName.ToLower(), cancellationToken);
            if (existingColor != null)
            {
                updateColorId = existingColor.Id;
            }
        }

        car.MakeId = request.MakeId;
        car.ModelId = request.ModelId;
        car.ManufactureYear = request.ManufactureYear;
        car.Color = updateColorName;
        car.ColorId = updateColorId;
        car.PlateNumber = plate;
        car.BranchId = request.BranchId;
        car.PurchaseDate = request.PurchaseDate.ToUniversalTime();
        car.PurchasePrice = request.PurchasePrice;
        var (calcBookValue, _, _) = calculationService.CalculateCarBookValue(request.PurchasePrice, request.PurchaseDate);
        car.BookValue = (request.BookValue.HasValue && request.BookValue.Value > 0) ? request.BookValue.Value : calcBookValue;
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
