using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.CarMakes.Dtos;

public record CarMakeDto(
    int Id,
    string Name,
    int ModelsCount);

public record CreateCarMakeRequest(string Name);

public record UpdateCarMakeRequest(string Name);

public class CreateCarMakeRequestValidator : AbstractValidator<CreateCarMakeRequest>
{
    public CreateCarMakeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);
    }
}

public class UpdateCarMakeRequestValidator : AbstractValidator<UpdateCarMakeRequest>
{
    public UpdateCarMakeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(100).WithMessage(Msg.ValidationFailed);
    }
}
