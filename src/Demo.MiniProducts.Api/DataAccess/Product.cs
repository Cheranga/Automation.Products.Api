namespace Demo.MiniProducts.Api.DataAccess;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public bool IsOnPromotion { get; set; }
}