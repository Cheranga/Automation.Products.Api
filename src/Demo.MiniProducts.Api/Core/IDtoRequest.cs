using Swashbuckle.AspNetCore.Filters;

namespace Demo.MiniProducts.Api.Core;

public interface IDtoRequest<T> where T : class, IDtoRequest<T>, IExamplesProvider<T>, new() { }