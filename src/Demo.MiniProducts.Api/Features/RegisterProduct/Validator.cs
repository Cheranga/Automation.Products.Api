using Demo.MiniProducts.Api.Core;
using FluentValidation;

namespace Demo.MiniProducts.Api.Features.RegisterProduct;

public class Validator : ModelValidatorBase<RegisterProductRequest>
{
    public Validator()
    {
        RuleFor(x => x.ProductId)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");

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

        RuleFor(x => x.LocationCode)
            .NotNull()
            .WithMessage("cannot be null")
            .NotEmpty()
            .WithMessage("cannot be empty");
    }
}
