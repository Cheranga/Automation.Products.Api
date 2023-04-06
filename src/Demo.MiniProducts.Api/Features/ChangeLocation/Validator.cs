using Demo.MiniProducts.Api.Core;
using FluentValidation;

namespace Demo.MiniProducts.Api.Features.ChangeLocation;

public class Validator : ModelValidatorBase<ChangeLocationRequest>
{
    public Validator()
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