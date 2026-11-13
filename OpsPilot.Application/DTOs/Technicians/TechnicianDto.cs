namespace OpsPilot.Application.DTOs.Technicians;

public sealed record TechnicianDto(
    Guid Id,
    string DisplayName,
    string Email);
