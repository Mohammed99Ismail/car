using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using FluentValidation;

namespace CarShowroom.Application.Features.Cars.Dtos;

public record CarDto(
    int Id,
    int MakeId,
    string MakeName,
    int ModelId,
    string ModelName,
    int ManufactureYear,
    string Color,
    string PlateNumber,
    int BranchId,
    string BranchName,
    DateTime PurchaseDate,
    decimal PurchasePrice,
    string? ImagePath,
    string ChassisNumber,
    string EngineNumber,
    string? Notes,
    DateTime CreatedDate);

public record CreateCarRequest(
    int MakeId,
    int ModelId,
    int ManufactureYear,
    string Color,
    string PlateNumber,
    int BranchId,
    DateTime PurchaseDate,
    decimal PurchasePrice,
    string ChassisNumber,
    string EngineNumber,
    string? Notes);

public record UpdateCarRequest(
    int MakeId,
    int ModelId,
    int ManufactureYear,
    string Color,
    string PlateNumber,
    int BranchId,
    DateTime PurchaseDate,
    decimal PurchasePrice,
    string ChassisNumber,
    string EngineNumber,
    string? Notes);

public class CarQueryParameters : QueryParameters
{
    public int? BranchId { get; set; }
    public int? MakeId { get; set; }
    public int? ModelId { get; set; }
    public int? ManufactureYear { get; set; }
}

public class CreateCarRequestValidator : AbstractValidator<CreateCarRequest>
{
    public CreateCarRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ModelId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ManufactureYear).InclusiveBetween(1980, 2100).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Color).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.BranchId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ChassisNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.EngineNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}

public class UpdateCarRequestValidator : AbstractValidator<UpdateCarRequest>
{
    public UpdateCarRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ModelId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ManufactureYear).InclusiveBetween(1980, 2100).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Color).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.PlateNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.BranchId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.ChassisNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.EngineNumber).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(60);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
