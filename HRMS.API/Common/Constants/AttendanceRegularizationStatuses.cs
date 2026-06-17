namespace HRMS.API.Common.Constants;

public static class AttendanceRegularizationStatuses
{
    public const string Pending = "Pending";

    public const string Approved = "Approved";

    public const string Rejected = "Rejected";

    public static readonly string[] All =
    [
        Pending,
        Approved,
        Rejected
    ];
}