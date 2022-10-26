using PasswordWallet.Client.Models;

namespace PasswordWallet.Client.Services;

public interface IAlertService
{
    event Action<Alert> OnAlert;
    void Success(string message, bool autoClose = true, bool keepAfterRouteChange = false);
    void Error(string message, bool autoClose = false, bool keepAfterRouteChange = false);
    void Info(string message, bool autoClose = true, bool keepAfterRouteChange = false);
    void Warn(string message, bool autoClose = true, bool keepAfterRouteChange = false);
    void Alert(Alert alert);
    void Clear(string? id = null);
}

public class AlertService : IAlertService
{
    private const string DefaultId = "default-alert";
    public event Action<Alert>? OnAlert;

    public void Success(string message, bool autoClose = true, bool keepAfterRouteChange = false)
    {
        Alert(new Alert
        {
            Type = AlertType.Success,
            Message = message,
            KeepAfterRouteChange = keepAfterRouteChange,
            AutoClose = autoClose
        });
    }

    public void Error(string message, bool autoClose = false, bool keepAfterRouteChange = false)
    {
        Alert(new Alert
        {
            Type = AlertType.Error,
            Message = message,
            KeepAfterRouteChange = keepAfterRouteChange,
            AutoClose = autoClose
        });
    }

    public void Info(string message, bool autoClose = true, bool keepAfterRouteChange = false)
    {
        Alert(new Alert
        {
            Type = AlertType.Info,
            Message = message,
            KeepAfterRouteChange = keepAfterRouteChange,
            AutoClose = autoClose
        });
    }

    public void Warn(string message, bool autoClose = true, bool keepAfterRouteChange = false)
    {
        Alert(new Alert
        {
            Type = AlertType.Warning,
            Message = message,
            KeepAfterRouteChange = keepAfterRouteChange,
            AutoClose = autoClose
        });
    }

    public void Alert(Alert alert)
    {
        alert.Id ??= DefaultId;
        OnAlert?.Invoke(alert);
    }

    public void Clear(string? id = DefaultId)
    {
        OnAlert?.Invoke(new Alert {Id = id});
    }
}