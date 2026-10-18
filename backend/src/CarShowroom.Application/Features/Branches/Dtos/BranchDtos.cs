using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.Branches.Dtos;

public record BranchDto(
    int Id,
    string Name,
    string Address,
    string Mobile,
    DateTime CreatedDate);

public record CreateBranchRequest(
    string Name,
    string Address,
    string Mobile);

public record UpdateBranchRequest(
    string Name,
    string Address,
    string Mobile);

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(150);
        RuleFor(x => x.Address).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(300);
        RuleFor(x => x.Mobile).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
    }
}

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(150);
        RuleFor(x => x.Address).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(300);
        RuleFor(x => x.Mobile).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
    }
}
