namespace Demo.MiniProducts.Api.Responses;

public class ApiResponse<T>
{
    public T Data { get; }

    private ApiResponse(T data)
    {
        Data = data;
    }
    
    public static ApiResponse<T> New(T data)=> new (data);
}