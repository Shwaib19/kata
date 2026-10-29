namespace SharedKernel;

/// <summary>
/// Chemins des hubs SignalR, centralises pour que le mapping (MapHub) et
/// l'authentification par query string (JwtBearerEvents.OnMessageReceived)
/// ne puissent jamais diverger.
/// </summary>
public static class HubRoutes
{
    public const string DriverTrack = "/DeliveryTrack";
}
