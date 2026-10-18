using CarShowroom.Application.Common.Localization;
using FluentValidation;

namespace CarShowroom.Application.Features.Customers.Dtos;

public record CustomerDto(
    int Id,
    string Name,
    string NationalId,
    string Mobile,
    string Address,
    DateTime CreatedDate);

public record CreateCustomerRequest(
    string Name,
    string NationalId,
    string Mobile,
    string Address);

public record UpdateCustomerRequest(
    string Name,
    string NationalId,
    string Mobile,
    string Address);

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(150);
        RuleFor(x => x.NationalId).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.Mobile).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.Address).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(300);
    }
}

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(150);
        RuleFor(x => x.NationalId).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(50);
        RuleFor(x => x.Mobile).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(30);
        RuleFor(x => x.Address).NotEmpty().WithMessage(Msg.ValidationFailed).MaximumLength(300);
    }
}
