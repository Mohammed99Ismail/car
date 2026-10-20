using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using FluentValidation;

namespace CarShowroom.Application.Features.CarModels.Dtos;

public record CarModelDto(
    int Id,
    int MakeId,
    string MakeName,
    string Name);

public class CarModelQueryParameters : QueryParameters
{
    public int? MakeId { get; set; }
}

public record CreateCarModelRequest(
    int MakeId,
    string Name);

public record UpdateCarModelRequest(
    int MakeId,
    string Name);

public class CreateCarModelRequestValidator : AbstractValidator<CreateCarModelRequest>
{
    public CreateCarModelRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);
    }
}

public class UpdateCarModelRequestValidator : AbstractValidator<UpdateCarModelRequest>
{
    public UpdateCarModelRequestValidator()
    {
        RuleFor(x => x.MakeId).GreaterThan(0).WithMessage(Msg.ValidationFailed);
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);
    }
}
