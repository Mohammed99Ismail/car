using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.Auth.Dtos;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .EmailAddress().WithMessage(Msg.ValidationFailed)
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MinimumLength(6);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .EmailAddress().WithMessage(Msg.ValidationFailed);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(Msg.ValidationFailed);
    }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .EmailAddress().WithMessage(Msg.ValidationFailed);
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(Msg.ValidationFailed);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage(Msg.ValidationFailed)
            .MinimumLength(6);
    }
}
