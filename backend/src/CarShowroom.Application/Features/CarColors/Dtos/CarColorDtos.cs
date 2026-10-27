using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.CarColors.Dtos;

public record CarColorDto(
    int Id,
    string Name,
    string? HexCode,
    int CarsCount = 0);

public record CreateCarColorRequest(
    string Name,
    string? HexCode = null);

public record UpdateCarColorRequest(
    string Name,
    string? HexCode = null);

public class CreateCarColorRequestValidator : AbstractValidator<CreateCarColorRequest>
{
    public CreateCarColorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);

        RuleFor(x => x.HexCode)
            .MaximumLength(20).WithMessage(Msg.ValidationFailed);
    }
}

public class UpdateCarColorRequestValidator : AbstractValidator<UpdateCarColorRequest>
{
    public UpdateCarColorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);

        RuleFor(x => x.HexCode)
            .MaximumLength(20).WithMessage(Msg.ValidationFailed);
    }
}
