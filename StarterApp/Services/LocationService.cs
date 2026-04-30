using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;

namespace StarterApp.Services;

public class LocationService : ILocationService
{
    private static readonly LocationCoordinates DefaultLocation = new(55.9533, -3.1883);

    public async Task<LocationCoordinates> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                return DefaultLocation;

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);

            return location == null
                ? DefaultLocation
                : new LocationCoordinates(location.Latitude, location.Longitude);
        }
        catch
        {
            return DefaultLocation;
        }
    }
}
