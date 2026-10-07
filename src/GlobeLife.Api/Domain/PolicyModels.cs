namespace GlobeLife.Api.Domain;

public sealed record PolicySummary(string PolicyNumber, string ProductName, string Status, decimal CoverageAmount, DateOnly RenewalDate);
public sealed record SupportRequest(string Category, string Message);
