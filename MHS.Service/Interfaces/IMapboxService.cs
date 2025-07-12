using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MHS.Service.DTOs;

namespace MHS.Service.Interfaces
{
    public interface IMapboxService
    {
        Task<(double lat, double lng)?> GeocodeAsync (string address, CancellationToken cancellationToken = default);
        Task<string?> ReverseGeocodeAsync(double lat, double lng);
        Task<MapboxDirectionsResult?> GetDirectionsAsync(double fromLat, double fromLng, double toLat, double toLng);
    }
}
