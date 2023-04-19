using FluentValidation;

namespace Demo.MiniProducts.Api.Core;

public interface IValidatable<T, TValidator>
    where T : IValidatable<T, TValidator>
    where TValidator : IValidator<T> { }