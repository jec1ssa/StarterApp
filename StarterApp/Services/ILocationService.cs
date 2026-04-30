namespace StarterApp.Services;

public interface ILocationService
{
    Task<LocationCoordinates> GetCurrentLocationAsync();
}

public record LocationCoordinates(double Latitude, double Longitude);
