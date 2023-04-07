namespace Storage.Blob.Helper;

public static class FunkyExtensions
{
    public static Operation<TB> Select<TA, TB>(this Operation<TA> operation, Func<TA, TB> select)
    {
        if (operation.IsFail)
            return Operation<TB>.Failure(operation.Error);

        return Operation<TB>.Success(select(operation.Data));
    }

    public static Operation<TC> SelectMany<TA, TB, TC>(
        this Operation<TA> model,
        Func<TA, Operation<TB>> select,
        Func<TA, TB, TC> projection
    )
    {
        if (model.IsFail)
            return Operation<TC>.Failure(model.Error);

        var selection = select(model.Data);
        if (selection.IsFail)
            return Operation<TC>.Failure(selection.Error);

        return Operation<TC>.Success(projection(model.Data, selection.Data));
    }

    public static Operation<TA> ToOperation<TA>(Func<TA> func)
    {
        try
        {
            var op = Operation<TA>.Success(func());
            return op;
        }
        catch (Exception exception)
        {
            return Operation<TA>.Failure(ErrorInfo.New(exception));
        }
    }

    public static async Task<Operation<TA>> ToOperation<TA>(Func<Task<TA>> func)
    {
        return Operation<TA>.Success(await func());
    }

    public static Operation<TA> MapFail<TA>(
        this Operation<TA> operation,
        Func<ErrorInfo, ErrorInfo> error
    ) => operation.IsFail ? Operation<TA>.Failure(error(operation.Error)) : operation;
}
