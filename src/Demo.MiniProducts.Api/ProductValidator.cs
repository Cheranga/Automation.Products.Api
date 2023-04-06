using Demo.MiniProducts.Api.Requests;
using FluentValidation;

namespace Demo.MiniProducts.Api;

public class RegisterProductRequestValidator : AbstractValidator<RegisterProductRequest>
{
    public RegisterProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");

        RuleFor(x => x.Category)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");
    }
}

public class ChangeLocationRequestValidator : AbstractValidator<ChangeLocationRequest>
{
    public ChangeLocationRequestValidator()
    {
        RuleFor(x => x.LocationCode)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");

        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");
    }
}
