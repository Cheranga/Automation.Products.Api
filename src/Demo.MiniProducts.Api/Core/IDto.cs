using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Core;

public interface IDto<T> where T : class, IDto<T>, IExamplesProvider<T>, new() { }