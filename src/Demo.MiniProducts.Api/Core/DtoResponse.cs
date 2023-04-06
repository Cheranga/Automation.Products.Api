namespace Demo.MiniProducts.Api.Core;

public abstract record ResponseDtoBase<T>(T Data) where T : notnull;




