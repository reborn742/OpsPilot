namespace OpsPilot.Infrastructure.Identity;

public static class RoleNames
{
    public const string SuperAdmin = "SuperAdmin";
    public const string ITManager = "ITManager";
    public const string Technician = "Technician";
    public const string Employee = "Employee";

    public static readonly string[] All =
    [
        SuperAdmin,
        ITManager,
        Technician,
        Employee
    ];
}
