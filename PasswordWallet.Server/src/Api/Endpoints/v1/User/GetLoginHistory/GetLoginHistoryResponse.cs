namespace Api.Endpoints.v1.User.GetLoginHistory;

public record GetLoginHistoryResponse(long Id, DateTimeOffset Date, bool Correct, string? IpAddress);