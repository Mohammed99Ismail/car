using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.Users.Dtos;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().WithMessage(Msg.ValidationFailed).EmailAddress().WithMessage(Msg.ValidationFailed).MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().WithMessage(Msg.ValidationFailed).MinimumLength(6);
    }
}
