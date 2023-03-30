using Demo.MiniProducts.Api.Models;
using FluentValidation;

namespace Demo.MiniProducts.Api;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("cannot be null")
            .NotEmpty().WithMessage("cannot be empty");
    }
}