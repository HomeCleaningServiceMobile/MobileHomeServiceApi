using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MHS.Service.Implementations
{
    public class MapboxService: IMapboxService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public MapboxService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _accessToken = configuration["Mapbox:AccessToken"] ?? throw new Exception("Mapbox access token not configured");
        }

        public async Task<(double lat, double lng)?> GeocodeAsync(string address, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{Uri.EscapeDataString(address)}.json?access_token={_accessToken}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("features", out var features) || features.GetArrayLength() == 0)
                    return null;

                var firstFeature = features[0];
                if (!firstFeature.TryGetProperty("center", out var coords) || coords.GetArrayLength() < 2)
                    return null;

                double lng = coords[0].GetDouble();
                double lat = coords[1].GetDouble();
                return (lat, lng);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MapboxService] GeocodeAsync error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> ReverseGeocodeAsync(double lat, double lng)
        {
            var url = $"https://api.mapbox.com/geocoding/v5/mapbox.places/{lng},{lat}.json?access_token={_accessToken}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var features = doc.RootElement.GetProperty("features");
            if (features.GetArrayLength() == 0) return null;

            return features[0].GetProperty("place_name").GetString();
        }

        public async Task<MapboxDirectionsResult?> GetDirectionsAsync(double fromLat, double fromLng, double toLat, double toLng)
        {
            var url = $"https://api.mapbox.com/directions/v5/mapbox/driving/{fromLng},{fromLat};{toLng},{toLat}?access_token={_accessToken}&geometries=polyline";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var routes = doc.RootElement.GetProperty("routes");
            if (routes.GetArrayLength() == 0) return null;

            var route = routes[0];
            return new MapboxDirectionsResult
            {
                Distance = route.GetProperty("distance").GetDouble(),
                Duration = route.GetProperty("duration").GetDouble(),
                Geometry = route.GetProperty("geometry").GetString(),
                Summary = route.TryGetProperty("summary", out var summaryProp) ? summaryProp.GetString() : null
            };
        }
    }
}
