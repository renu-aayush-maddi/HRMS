namespace HRMS.API.Constants;

public static class DocumentTypes
{
    public const string Aadhaar = "Aadhaar";
    public const string Pan = "PAN";
    public const string Passport = "Passport";
    public const string Resume = "Resume";
    public const string DrivingLicense = "DrivingLicense";


    public static readonly string[] All =
    [
        Aadhaar,
        Pan,
        Passport,
        Resume,
        DrivingLicense
    ];
}