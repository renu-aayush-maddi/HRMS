namespace HRMS.API.Common.Constants;

public static class PerformanceCycleStatuses
{
    public const string Draft = "Draft";
    public const string Active = "Active";
    public const string Closed = "Closed";

    public static readonly string[] All =
    [
        Draft,
        Active,
        Closed
    ];
}