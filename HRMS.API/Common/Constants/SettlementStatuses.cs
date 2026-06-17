namespace HRMS.API.Constants;

public static class SettlementStatuses
{
    public const string Pending = "Pending";

    public const string Processing = "Processing";

    public const string Completed = "Completed";

    public static readonly string[] All =
    [
        Pending,
        Processing,
        Completed
    ];
}