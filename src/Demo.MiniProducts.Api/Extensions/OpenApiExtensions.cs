using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Models;

namespace Demo.MiniProducts.Api.Extensions;

public static class OpenApiExtensions
{
    public static OpenApiOperation SetParamInfo<T>(this OpenApiOperation operation, Expression<Func<T, object>> expression, Action<OpenApiParameter> parameterFunc)
    where T:class
    {
        var propertyName = ((expression.Body as MemberExpression)!).Member.Name;
        var parameter = operation.Parameters.FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        if (parameter != null)
        {
            parameterFunc(parameter);
        }

        return operation;
    }
}