using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.Roles.Dtos;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(100);
    }
}

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(100);
    }
}
