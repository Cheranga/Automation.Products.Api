using FluentValidation;
using FluentValidation.Results;

namespace Demo.MiniProducts.Api.Core;

public class ModelValidatorBase<T> : AbstractValidator<T> where T : class
{
    protected ModelValidatorBase() => RuleLevelCascadeMode = CascadeMode.Stop;

    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
    {
        if (context.InstanceToValidate != null)
            return true;
        result.Errors.Add(new ValidationFailure("", "instance cannot be null"));
        return false;
    }
}
