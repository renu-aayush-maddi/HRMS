namespace HRMS.API.Common.Constants;

public static class AttendanceConstants
{
    public const string Present = "Present";

    public const string Late = "Late";

    public const string HalfDay = "HalfDay";

    public const string Absent = "Absent";

    public const string WorkFromHome = "WorkFromHome";

    public static readonly string[] All =
    [
        Present,
        Late,
        HalfDay,
        Absent,
        WorkFromHome
    ];

    public static readonly TimeOnly LateThreshold =
        new(9, 30);

    public const decimal HalfDayThresholdHours = 4m;
}