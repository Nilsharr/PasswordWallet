namespace PasswordWallet.Client.Models;

public class Alert
{
    public string? Id { get; set; }
    public AlertType Type { get; init; }
    public string? Message { get; init; }
    public bool AutoClose { get; init; }
    public bool KeepAfterRouteChange { get; set; }
    public bool Fade { get; set; }
}

public enum AlertType
{
    Success,
    Error,
    Info,
    Warning
}