using Microsoft.AspNetCore.Mvc;
using MHS.Service.Interfaces;

namespace MobileHomeServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapboxController : ControllerBase
    {
        private readonly IMapboxService _mapboxService;

        public MapboxController(IMapboxService mapboxService)
        {
            _mapboxService = mapboxService;
        }

        /// <summary>
        /// Geocode: Địa chỉ -> Tọa độ
        /// </summary>
        [HttpGet("geocode")]
        public async Task<IActionResult> Geocode([FromQuery] string address)
        {
            var result = await _mapboxService.GeocodeAsync(address);
            if (result == null) return NotFound("Address not found");
            return Ok(new { latitude = result.Value.lat, longitude = result.Value.lng });
        }

        /// <summary>
        /// Reverse geocode: Tọa độ -> Địa chỉ
        /// </summary>
        [HttpGet("reverse-geocode")]
        public async Task<IActionResult> ReverseGeocode([FromQuery] double lat, [FromQuery] double lng)
        {
            var result = await _mapboxService.ReverseGeocodeAsync(lat, lng);
            if (result == null) return NotFound("Location not found");
            return Ok(new { address = result });
        }

        /// <summary>
        /// Directions: Lấy đường đi giữa 2 điểm
        /// </summary>
        [HttpGet("directions")]
        public async Task<IActionResult> GetDirections(
            [FromQuery] double fromLat,
            [FromQuery] double fromLng,
            [FromQuery] double toLat,
            [FromQuery] double toLng)
        {
            var result = await _mapboxService.GetDirectionsAsync(fromLat, fromLng, toLat, toLng);
            if (result == null) return NotFound("Route not found");
            return Ok(result);
        }
    }
}
