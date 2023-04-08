namespace Storage.Table.Helper;

public class ErrorCodes
{
    public const int UnregisteredTableService = 500;
    public const int TableUnavailable = 501;
    public const int CannotUpsert = 502;
}

public class ErrorMessages
{
    public const string UnregisteredTableService = "table service is unregistered for the storage account.";
    public const string TableUnavailable = "table is unavailable";
    public const string CannotUpsert = "data cannot be upserted into the storage table";
}