using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;

namespace Demo.MiniProducts.Api.Core;

[ExcludeFromCodeCoverage]
public class ModelValidatorBase<T> : AbstractValidator<T> where T : notnull
{
    protected ModelValidatorBase() => RuleLevelCascadeMode = CascadeMode.Stop;

    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
    {
        if (context.InstanceToValidate != null)
            return true;
        result.Errors.Add(new ValidationFailure("", "instance cannot be null"));
        return false;
    }
    
    protected void NotNullOrEmpty(
        params Expression<Func<T, object>>[] properties
    )
    {
        (properties?.ToList() ?? new List<Expression<Func<T, object>>>()).ForEach(x =>
        {
            RuleFor(x)
                .NotNull()
                .WithMessage("cannot be null")
                .NotEmpty()
                .WithMessage("cannot be empty");
        });
    }
}