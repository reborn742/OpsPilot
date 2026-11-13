namespace OpsPilot.Application.DTOs.Dashboard;

public sealed record DashboardSummary(
    int TotalAssets,
    int ActiveAssets,
    int OpenIncidents,
    int CriticalIncidents,
    int PendingIncidents);
