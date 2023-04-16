global using GetProductResponse = Azure.Storage.Table.Wrapper.Queries.QueryResponse<
    Azure.Storage.Table.Wrapper.Queries.QueryResult.QueryFailedResult,
    Azure.Storage.Table.Wrapper.Queries.QueryResult.EmptyResult,
    Azure.Storage.Table.Wrapper.Queries.QueryResult.SingleResult<Demo.MiniProducts.Api.DataAccess.ProductDataModel>
>;