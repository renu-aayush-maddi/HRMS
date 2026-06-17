namespace HRMS.API.Constants;

public static class ResignationStatuses
{
    public const string Pending = "Pending";

    public const string Approved = "Approved";

    public const string Rejected = "Rejected";

    public const string Withdrawn = "Withdrawn";

    public static readonly string[] All =
    [
        Pending,
        Approved,
        Rejected,
        Withdrawn
    ];
}