using Demo.MiniProducts.Api;

var app = Bootstrapper.Setup(args);
var productsApi = app.MapGroup($"/{ProductApi.Route}/");

productsApi.MapGet("", ProductApi.GetAllProducts);
productsApi.MapGet("/{id}", ProductApi.GetProduct);
productsApi.MapPost("/", ProductApi.Create);
productsApi.MapPut("/{id}", ProductApi.Update);

app.Run();
